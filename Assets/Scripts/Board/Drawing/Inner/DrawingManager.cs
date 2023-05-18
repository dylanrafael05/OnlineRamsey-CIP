using UnityEngine;

namespace Ramsey.Drawing
{
    public class DrawingManager
    {

        public DrawingActionInterface ReadingInterface { get; private set; }
        public DrawingWritingInterface WritingInterface { get; private set; }

        DrawingData data;
        Drawer drawer;

        public DrawingManager(Camera camera, DrawingPreferences preferences)
        {

            data = new();
            drawer = new(data, preferences, camera);

            ReadingInterface = new(drawer);
            WritingInterface = new(data, preferences, drawer);

        }

    }
}