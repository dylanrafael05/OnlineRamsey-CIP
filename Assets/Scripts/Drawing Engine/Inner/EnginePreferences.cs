using UnityEngine;

public struct EnginePreferences
{

    public Color blackColor;
    public Color redColor;
    public Color blueColor;

    public float edgeThickness;
    public float nodeRadius;

    public Color TypeToColor(int type)
        => type == -1 ? blackColor : type == 0 ? blueColor : redColor;

    public void UniformPreferences()
    {
        NodeMaterial.SetFloat(Shader.PropertyToID("_Radius"), nodeRadius);
    }

    public static readonly Material EdgeMaterial = new(Shader.Find("Unlit/GraphShaders/EdgeShader"));
    public static readonly Material NodeMaterial = new(Shader.Find("Unlit/GraphShaders/NodeShader"));

}