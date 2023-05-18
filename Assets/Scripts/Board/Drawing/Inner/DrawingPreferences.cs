using UnityEngine;

namespace Ramsey.Drawing
{
    public struct DrawingPreferences
    {

        public Color nullColor;
        public Color[] colors;

        public float edgeThickness;
        public float nodeRadius;

        public Color TypeToColor(int type)
            => type == -1 ? nullColor : colors[type];

        public void UniformPreferences()
        {
            NodeMaterial.SetFloat(Shader.PropertyToID("_Radius"), nodeRadius);
        }

        public static readonly Material EdgeMaterial = new(Shader.Find("Unlit/GraphShaders/EdgeShader"));
        public static readonly Material NodeMaterial = new(Shader.Find("Unlit/GraphShaders/NodeShader"));

    }
}