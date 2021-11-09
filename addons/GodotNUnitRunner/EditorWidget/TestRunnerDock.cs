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
        private Tree _resultTree;

        public override void _Ready()
        {
            InitializeNUnitIfNeeded();

            // Connect controls
            _refreshButton = (Button)FindNode("RefreshButton");
            _refreshButton.Connect("pressed", this, nameof(RefreshButton_Click));

            _resultTree = (Tree)FindNode("ResultTree");
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
    }

}
