using UnityEngine;

public struct EnginePreferences
{

    public Color redColor;
    public Color blueColor;

    public float edgeThickness;
    public float nodeRadius;

    public void UniformPreferences()
    {
        EdgeMaterial.SetColor(Shader.PropertyToID("_RedColor"), redColor);
        EdgeMaterial.SetColor(Shader.PropertyToID("_BlueColor"), blueColor);
        EdgeMaterial.SetFloat(Shader.PropertyToID("_Thickness"), edgeThickness);

        NodeMaterial.SetFloat(Shader.PropertyToID("_Radius"), nodeRadius);
    }

    public static readonly Material EdgeMaterial = new(Shader.Find("put edge shade rpath here"));
    public static readonly Material NodeMaterial = new(Shader.Find("put node shade rpath here"));

}