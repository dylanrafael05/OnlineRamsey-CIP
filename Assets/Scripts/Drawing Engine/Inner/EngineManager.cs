using UnityEngine;

public class EngineManager
{

    public EngineInterface Interface { get; private set; }

    public EngineData data { get; private set; } //UNEXPOSE
    EngineDrawer drawer;

    public EngineManager(Camera camera, EnginePreferences preferences)
    {

        data = new();
        drawer = new(data, preferences, camera);

        Interface = new(drawer);

    }

}