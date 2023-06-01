using Unity.Mathematics;
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

        public void UniformPreferences()
        {
            UnityReferences.NodeMaterial.SetFloat("_Radius", nodeRadius);
            UnityReferences.NodeMaterial.SetFloat("_HighlightRadius", highlightRadius);

            UnityReferences.NodeMaterial.SetFloat("_HighlightThickness", highlightThickness);

            UnityReferences.NodeMaterial.SetColor("_NodeColor", nodeColor);
            UnityReferences.NodeMaterial.SetColor("_HighlightColor", highlightColor);

            UnityReferences.EdgeMaterial.SetColor("_HighlightColor", nodeColor); //think setting it to be nodecolor would be cool
            UnityReferences.EdgeMaterial.SetFloat("_Thickness", edgeThickness);

            UnityReferences.RecorderMaterial.SetColor("_Color", recorderColor);
            UnityReferences.RecorderMaterial.SetFloat("_xScale", 2.0f);

            UnityReferences.LoadingMaterial.SetFloat("_InnerRadius", loadingCircleInnerRadius);
            UnityReferences.LoadingMaterial.SetFloat("_OuterRadius", loadingCircleOuterRadius);
        }
    }
}