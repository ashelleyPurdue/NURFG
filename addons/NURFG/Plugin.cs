#if TOOLS
using Godot;
using System;

namespace NURFG
{
    [Tool]
    public partial class Plugin : EditorPlugin
    {
        private Control _dock;

        public override void _EnterTree()
        {
            _dock = (Control)GD.Load<PackedScene>("addons/NURFG/EditorWidget/TestRunnerDock.tscn").Instantiate();
            AddControlToDock(DockSlot.RightBl, _dock);
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
#endif