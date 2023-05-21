using UnityEngine;

namespace Ramsey.Drawing
{
    public struct DrawingPreferences
    {

        public Color nullColor;
        public Color[] colors;

        public float edgeThickness;
        public float nodeRadius;
        public float highlightRadius;

        public float highlightThickness;

        public Color nodeColor;
        public Color highlightColor;

        public Color TypeToColor(int type)
            => type == -1 ? nullColor : colors[type];

        public void UniformPreferences()
        {
            NodeMaterial.SetFloat(Shader.PropertyToID("_Radius"), nodeRadius);
            NodeMaterial.SetFloat(Shader.PropertyToID("_HighlightRadius"), highlightRadius);

            NodeMaterial.SetFloat(Shader.PropertyToID("_HighlightThickness"), highlightThickness);

            NodeMaterial.SetColor(Shader.PropertyToID("_NodeColor"), nodeColor);
            NodeMaterial.SetColor(Shader.PropertyToID("_HighlightColor"), highlightColor);

            EdgeMaterial.SetColor(Shader.PropertyToID("_HighlightColor"), nodeColor); //think setting it to be nodecolor would be cool

            RecordingMaterial.SetColor(Shader.PropertyToID("_Color"), Color.white);
        }

        public static readonly Material EdgeMaterial = new(Shader.Find("Unlit/GraphShaders/EdgeShader"));
        public static readonly Material NodeMaterial = new(Shader.Find("Unlit/GraphShaders/NodeShader"));
        public static readonly Material RecordingMaterial = new(Shader.Find("Unlit/GraphShaders/RecordingShader"));

    }
}