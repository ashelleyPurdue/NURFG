using Godot;
using System;

namespace GodotNUnitRunner
{
    [Tool]
    public class Plugin : EditorPlugin
    {
        private Control _dock;
        private Button _refreshButton;

        public override void _EnterTree()
        {
            _dock = (Control)GD.Load<PackedScene>("addons/GodotNUnitRunner/EditorWidget/TestRunnerDock.tscn").Instance();
            AddControlToDock(DockSlot.LeftUl, _dock);
        }

        public override void _ExitTree()
        {
            // Clean-up of the plugin goes here.
            // Remove the dock.
            RemoveControlFromDocks(_dock);

            // Erase the control from the memory.
            _dock?.Free();
        }
    }
}
