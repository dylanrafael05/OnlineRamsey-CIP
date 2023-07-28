using UnityEngine;

namespace Ramsey.Drawing
{
    public class DrawingManager
    {
        public DrawingActionInterface ActionInterface { get; private set; }
        public DrawingIOInterface IOInterface { get; private set; }

        #if HEADLESS
            public DrawingManager(Camera _, Camera _, DrawingPreferences _)
            {
                ActionInterface = new();
                IOInterface = new();
            }
        #else
            DrawingStorage data;
            Drawer drawer;

            public DrawingManager(Camera boardCamera, Camera screenCamera, DrawingPreferences preferences)
            {

                data = new();
                drawer = new(data, preferences, boardCamera, screenCamera);

                ActionInterface = new(drawer, data);
                IOInterface = new(data, preferences, drawer);

            }
        #endif
    }
}