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
            InitializeNUnitIfNeeded();

            var rootTest = _nunit.Runner.ExploreTests(new MatchEverythingTestFilter());
            DisplayTests(rootTest);
        }

        private void RunButton_Click()
        {
            var testFilter = new MatchEverythingTestFilter();
            var testListener = new LambdaListener
            {
                TestFinishedCallback = result =>
                {
                    if (_nunit.Runner.Result == null)
                        return;
                    DisplayResults(_nunit.Runner.Result);
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

        private void DisplayTests(ITest rootTest)
        {
            _resultTree.Clear();
            DisplayTestsRecursive(rootTest);

            void DisplayTestsRecursive(ITest test, TreeItem parentTree = null)
            {
                var treeItem = _resultTree.CreateItem(parentTree);
                treeItem.SetText(0, test.Name);

                foreach (var childTest in test.Tests)
                    DisplayTestsRecursive(childTest, treeItem);
            }
        }

        private void DisplayResults(ITestResult rootTest)
        {
            _resultTree.Clear();
            DisplayResultsRecursive(rootTest);

            void DisplayResultsRecursive(ITestResult test, TreeItem parentTree = null)
            {
                var treeItem = _resultTree.CreateItem(parentTree);
                treeItem.SetText(0, $"({test.ResultState}){test.Name}");

                foreach (var childTest in test.Children)
                    DisplayResultsRecursive(childTest, treeItem);
            }
        }
    }

}
