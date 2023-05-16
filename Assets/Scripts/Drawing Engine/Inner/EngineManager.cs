using UnityEngine;

public class EngineManager
{

    public ReadingInterface Interface { get; private set; }

    EngineData data;
    EngineDrawer drawer;

    public EngineManager(Camera camera, EnginePreferences preferences)
    {

        data = new();
        drawer = new(data, preferences, camera);

        Interface = new(drawer);

    }

}