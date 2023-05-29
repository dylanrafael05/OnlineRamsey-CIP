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

        public float loadingCircleOuterRadius;
        public float loadingCircleInnerRadius;

        public Color nodeColor;
        public Color highlightColor;

        public Color recorderColor;

        public Color TypeToColor(int type)
            => type == -1 ? nullColor : colors[type];

        public Matrix4x4 RecorderTransform;
        public Matrix4x4 LoadingCircleTransform;

        public void UniformPreferences()
        {
            Values.NodeMaterial.SetFloat("_Radius", nodeRadius);
            Values.NodeMaterial.SetFloat("_HighlightRadius", highlightRadius);

            Values.NodeMaterial.SetFloat("_HighlightThickness", highlightThickness);

            Values.NodeMaterial.SetColor("_NodeColor", nodeColor);
            Values.NodeMaterial.SetColor("_HighlightColor", highlightColor);

            Values.EdgeMaterial.SetColor("_HighlightColor", nodeColor); //think setting it to be nodecolor would be cool

            Values.RecorderMaterial.SetColor("_Color", recorderColor);

            Values.LoadingMaterial.SetFloat("_InnerRadius", loadingCircleInnerRadius);
            Values.LoadingMaterial.SetFloat("_OuterRadius", loadingCircleOuterRadius);
        }
    }
}