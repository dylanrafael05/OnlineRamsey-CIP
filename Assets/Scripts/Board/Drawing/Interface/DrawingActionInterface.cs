using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine.Profiling;

namespace Ramsey.Drawing
{
    public class DrawingActionInterface
    {
        #if HEADLESS
            public void RenderBoard() {}
            public void RenderUI() {}
            public void Cleanup() {}
        #else
            readonly Drawer drawer;
            readonly DrawingStorage data;

            internal DrawingActionInterface(Drawer drawer, DrawingStorage data)
            { this.drawer = drawer; this.data = data; }

            public void RenderBoard()
                => drawer.DrawBoard();
            public void RenderUI()
                => drawer.DrawUI();
            public void RenderLoadingDirect()
                => drawer.DrawLoadingDirect();
            public void Cleanup()
                => drawer.Cleanup();
        #endif

    }
}