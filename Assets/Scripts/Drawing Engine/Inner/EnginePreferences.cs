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

        NodeMaterial.SetFloat(Shader.PropertyToID("_Radius"), nodeRadius);
    }

    public static readonly Material EdgeMaterial = new(Shader.Find("Unlit/GraphShaders/EdgeShader"));
    public static readonly Material NodeMaterial = new(Shader.Find("Unlit/GraphShaders/NodeShader"));

}