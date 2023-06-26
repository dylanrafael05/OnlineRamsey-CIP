using UnityEngine;

namespace Ramsey.Drawing
{
    public class DrawingManager
    {
        public DrawingActionInterface ActionInterface { get; private set; }
        public DrawingIOInterface IOInterface { get; private set; }

        #if HEADLESS
            public DrawingManager(Camera _, DrawingPreferences _)
            {
                ActionInterface = new();
                IOInterface = new();
            }
        #else
            DrawingStorage data;
            Drawer drawer;

            public DrawingManager(Camera camera, DrawingPreferences preferences)
            {

                data = new();
                drawer = new(data, preferences, camera);

                ActionInterface = new(drawer, data);
                IOInterface = new(data, preferences, drawer);

            }
        #endif
    }
}