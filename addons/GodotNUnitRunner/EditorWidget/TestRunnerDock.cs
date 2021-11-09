using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace GodotNUnitRunner
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

        public override void _Ready()
        {
            InitializeNUnitIfNeeded();

            // Connect controls
            _refreshButton = (Button)FindNode("RefreshButton");
            _refreshButton.Connect("pressed", this, nameof(RefreshButton_Click));

            _runButton = (Button)FindNode("RunButton");
            _runButton.Connect("pressed", this, nameof(RunButton_Click));

            _resultTree = (Tree)FindNode("ResultTree");
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
            _testResults.Clear();
            RefreshButton_Click();

            var testFilter = new MatchEverythingTestFilter();
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

            _nunit.Runner.RunAsync(testListener, testFilter);
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

            if (!_testResults.ContainsKey(test))
            {
                treeItem.SetText(0, $"? {test.Name}");
            }
            else if (_testResults[test] == null)
            {
                treeItem.SetText(0, $"(...) {test.Name}");
            }
            else
            {
                var status = _testResults[test].ResultState.Status;
                treeItem.SetText(0, $"{GetStatusIcon(status)} {test.Name}");
            }
        }

        private string GetStatusIcon(TestStatus status)
        {
            switch (status)
            {
                case TestStatus.Passed: return "✔";
                case TestStatus.Failed: return "[FAILED]";
                case TestStatus.Inconclusive: return "?";
                case TestStatus.Warning: return "[WARN]";
                
                default: return status.ToString();
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
    }

}
