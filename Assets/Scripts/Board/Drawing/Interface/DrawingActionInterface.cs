using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine.Profiling;

namespace Ramsey.Drawing
{
    public class DrawingActionInterface
    {

        //
        readonly Drawer drawer;
        readonly DrawingStorage data;

        internal DrawingActionInterface(Drawer drawer, DrawingStorage data)
        { this.drawer = drawer; this.data = data; }

        //
        public void Update()
        {
            drawer.Draw();
        }

        public void Cleanup()
            => drawer.Cleanup();

    }
}