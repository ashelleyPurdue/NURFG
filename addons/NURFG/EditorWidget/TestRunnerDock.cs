using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace NURFG
{
    [Tool]
    public class TestRunnerDock : Control
    {
        private FrameworkController _nunit;

        private Dictionary<ITest, TreeItem> _testTreeItems = new Dictionary<ITest, TreeItem>();
        private Dictionary<ITest, ITestResult> _testResults = new Dictionary<ITest, ITestResult>();

        private Button _refreshButton;
        private Button _runButton;
        private Tree _resultTree;
        private RichTextLabel _testOutputLabel;

        public override void _Ready()
        {
            InitializeNUnitIfNeeded();

            // Connect controls
            _refreshButton = (Button)FindNode("RefreshButton");
            _refreshButton.Connect("pressed", this, nameof(RefreshButton_Click));

            _runButton = (Button)FindNode("RunButton");
            _runButton.Connect("pressed", this, nameof(RunButton_Click));

            _resultTree = (Tree)FindNode("ResultTree");
            _resultTree.Connect("item_selected", this, nameof(TestResultTree_ItemSelected));
            _resultTree.Connect("item_activated", this, nameof(TestResultTree_ItemActivated));

            _testOutputLabel = (RichTextLabel)FindNode("TestOutputLabel");
        }

        public override void _Process(float delta)
        {
            base._Process(delta);

            InitializeNUnitIfNeeded();
            EnableButtons(!_nunit.Runner.IsTestRunning);
        }

        private void InitializeNUnitIfNeeded()
        {
            if (_nunit != null)
                return;

            GD.Print("Initializing NUnit");

            _nunit = new FrameworkController(
                Assembly.GetExecutingAssembly(),
                "gnur",
                new Dictionary<string, object>()
            );
            _nunit.LoadTests();
        }


        private void RefreshButton_Click()
        {
            _testTreeItems.Clear();
            _resultTree.Clear();
            
            CreateTreeItemForTest(_nunit.Runner.LoadedTest);

            foreach (var test in _testTreeItems.Keys)
                UpdateTestTreeItem(test);
        }

        private void RunButton_Click()
        {
            RefreshButton_Click();
            StartTestRun(new MatchEverythingTestFilter());
        }

        private void TestResultTree_ItemSelected()
        {
            var selectedItem = _resultTree.GetSelected();
            ITest selectedTest = GetTestFromTreeItem(selectedItem);
            DisplayTestOutput(selectedTest);
        }

        private void TestResultTree_ItemActivated()
        {
            // Run the selected test
            var selectedItem = _resultTree.GetSelected();
            ITest selectedTest = GetTestFromTreeItem(selectedItem);
            StartTestRun(new MatchDescendantsOfFilter(selectedTest));
        }


        private void StartTestRun(ITestFilter filter)
        {
            // Mark all of the tests matched by the filter as "not run".
            var testsToClear = _testResults
                .Keys
                .Where(test => filter.Pass(test))
                .ToArray();

            foreach (var test in testsToClear)
            {
                _testResults.Remove(test);
                UpdateTestTreeItem(test);
            }

            // Whenever a test starts or finishes, update its results and its
            // tree item.
            var testListener = new LambdaListener
            {
                TestStartedCallback = (test) =>
                {
                    CreateTreeItemForTest(test);
                    _testResults[test] = null;
                    UpdateTestTreeItem(test);
                },

                TestFinishedCallback = (result) =>
                {
                    _testResults[result.Test] = result;
                    UpdateTestTreeItem(result.Test);
                }
            };

            // Start running the tests in the background.
            _nunit.Runner.RunAsync(testListener, filter);
        }

        private void EnableButtons(bool enabled)
        {
            // Instead of an "Enabled" property, Godot uses a "Disabled"
            // property.  Why?  No clue.
            // Thanks, Godot!
            bool disabled = !enabled;

            _refreshButton.Disabled = disabled;
            _runButton.Disabled = disabled;
        }


        private void UpdateTestTreeItem(ITest test)
        {
            var treeItem = _testTreeItems[test];
            treeItem.SetText(0, GetTestLabel(test));

            // Recursively update all ancestor items
            if (test.Parent != null)
                UpdateTestTreeItem(test.Parent);
        }

        private string GetTestLabel(ITest test)
        {
            var state = GetTestState(test);
            return $"{TestStateToIcon(state)} {test.Name}";
        }

        /// <summary>
        /// Gets a value corresponding to the "icon" that should be displayed
        /// next to a test's name
        /// 
        /// If there are children, it examines the results of all of them and
        /// returns the "worst" of them.
        /// </summary>
        /// <param name="test"></param>
        /// <returns></returns>
        private TestState GetTestState(ITest test)
        {
            // Recursive case: find the worst of the children.
            if (test.HasChildren)
            {
                var worstState = TestState.Passed;

                foreach (var child in test.Tests)
                {
                    var childState = GetTestState(child);
                    if (childState < worstState)
                        worstState = childState;
                }

                return worstState;
            }

            // Tests that haven't been run do not have an entry in _testResults.
            if (!_testResults.ContainsKey(test))
                return TestState.NotRun;
            
            // Tests that are in progress have a null entry in _testResults
            else if (_testResults[test] == null)
                return TestState.InProgress;

            // All others are self-explanatory.
            switch (_testResults[test].ResultState.Status)
            {
                case TestStatus.Failed: return TestState.Failed;
                case TestStatus.Inconclusive: return TestState.Inconclusive;
                case TestStatus.Passed: return TestState.Passed;
                case TestStatus.Skipped: return TestState.Skipped;
                case TestStatus.Warning: return TestState.Warning;
            }

            throw new Exception("Unexpected TestStatus " + _testResults[test].ResultState.Status);
        }

        private enum TestState
        {
            InProgress = 0,
            Failed = 1,
            Warning = 2,
            Inconclusive = 3,
            Skipped = 4,
            NotRun = 5,
            Passed = 6
        }

        private string TestStateToIcon(TestState state)
        {
            switch (state)
            {
                case TestState.NotRun: return "?";
                case TestState.InProgress: return "(...)";
                case TestState.Passed: return "✔";
                case TestState.Failed: return "[FAILED]";
                case TestState.Inconclusive: return "?";
                case TestState.Warning: return "[WARN]";
                
                default: return $"[{state.ToString().ToUpper()}]";
            }
        }


        private void DisplayTestOutput(ITest test)
        {
            if (test == null)
            {
                _testOutputLabel.Text = "";
                return;
            }

            if (!_testResults.ContainsKey(test))
            {
                _testOutputLabel.Text = "Test not run.";
                return;
            }

            if (_testResults[test] == null)
            {
                _testOutputLabel.Text = "Test in progress...";
                return;
            }

            var builder = new System.Text.StringBuilder();
            var testResult = _testResults[test];

            builder.AppendLine(testResult.Name);
            PrintIfNotEmpty(testResult.Message);
            PrintIfNotEmpty(testResult.Output);
            PrintIfNotEmpty(testResult.StackTrace);

            _testOutputLabel.Text = builder.ToString();

            void PrintIfNotEmpty(string msg)
            {
                if (!string.IsNullOrWhiteSpace(msg))
                    builder.AppendLine(msg);
            }
        }

        private void CreateTreeItemForTest(ITest test)
        {
            if (_testTreeItems.ContainsKey(test))
                return;
            
            // Create a tree item for this test
            var parentTreeItem = test.Parent == null
                ? null
                : _testTreeItems[test.Parent];
                    
            var treeItem = _resultTree.CreateItem(parentTreeItem);
            _testTreeItems[test] = treeItem;

            // Create tree items for all child tests
            foreach (var child in test.Tests)
                CreateTreeItemForTest(child);
        }
    
        private ITest GetTestFromTreeItem(TreeItem treeItem)
        {
            return _testTreeItems
                .Where(kvp => kvp.Value == treeItem)
                .Select(kvp => kvp.Key)
                .FirstOrDefault();
        }
    }

}
