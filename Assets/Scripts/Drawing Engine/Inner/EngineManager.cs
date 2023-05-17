using UnityEngine;

public class EngineManager
{

    public ReadingInterface ReadingInterface { get; private set; }
    public WritingInterface WritingInterface { get; private set; }

    EngineData data;
    EngineDrawer drawer;

    public EngineManager(Camera camera, DrawingPreferences preferences)
    {

        data = new();
        drawer = new(data, preferences, camera);

        ReadingInterface = new(drawer);
        WritingInterface = new(data, preferences, drawer);

    }

}