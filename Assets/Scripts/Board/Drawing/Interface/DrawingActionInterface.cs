using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine.Profiling;

namespace Ramsey.Drawing
{
    public class DrawingActionInterface
    {

        //
        readonly Drawer drawer;
        readonly DrawingData data;

        internal DrawingActionInterface(Drawer drawer, DrawingData data)
        { this.drawer = drawer; this.data = data; }

        //
        public void Draw()
        {
            drawer.Draw();
        }

        public void Cleanup()
            => drawer.Cleanup();

    }
}