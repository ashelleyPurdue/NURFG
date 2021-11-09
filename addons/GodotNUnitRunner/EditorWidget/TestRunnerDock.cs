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

        private Node _refreshButton;

        public override void _Ready()
        {
            InitializeNUnitIfNeeded();

            // Connect buttons
            _refreshButton = FindNode("RefreshButton");
            _refreshButton.Connect("pressed", this, nameof(RefreshButton_Click));
        }

        private void InitializeNUnitIfNeeded()
        {
            if (_nunit != null)
                return;

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
            PrintTest(rootTest);
        }

        private void PrintTest(ITest test, int indentLevel = 0)
        {
            Print($"* {test.Name}");

            foreach (var childTest in test.Tests)
                PrintTest(childTest, indentLevel + 1);

            void Print(string msg)
            {
                var builder = new System.Text.StringBuilder();

                for (int i = 0; i < indentLevel; i++)
                    builder.Append("  ");
                
                builder.Append(msg);
                GD.Print(builder.ToString());
            }
        }
    }

}
