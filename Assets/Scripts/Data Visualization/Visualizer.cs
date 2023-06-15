using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Ramsey.Utilities;
using Ramsey.Drawing;

namespace Ramsey.Visualization
{
    public struct GraphPreferences
    {

        /// <summary>
        /// Size Bounds are where the curves on the graph start to get dithered. 
        /// Axis Scale is the scale of the actual graph geometry. 
        /// Draw Size is how large the quad will be drawn in space, shouldn't be touched unless you encounter clipping.
        /// </summary>
        /// <param name="sizeBounds"></param>
        /// <param name="axisScale"></param>
        /// <param name="position"></param>
        /// <param name="tickCount"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        /// <param name="drawSize"></param>
        public GraphPreferences(float2 sizeBounds, float2 axisScale, float2 position, int2 tickCount, Color color, float thickness, float drawSize = 5f)
        { this.sizeBounds= sizeBounds; this.axisScale = axisScale; this.position = position; this.tickCount= tickCount; this.color = color; this.thickness = thickness; this.drawSize= drawSize; }

        static Material material = new(Shader.Find("Unlit/DataGraph"));

        public float2 sizeBounds; //could add some dithering on the curve as it leaves ok i did but barely visible since the line is so thin
        public float2 axisScale;
        public float2 position;

        public int2 tickCount;

        public Color color;
        public float thickness;
        
        public float drawSize;

        /*public Matrix4x4 GetCurveMatrix()
            => Matrix4x4.TRS((position+.5f*thickness).xyz(Visualizer.Depth), Quaternion.identity, Vector3.one);

        public Matrix4x4 GetGraphMatrix()
            => Matrix4x4.TRS((position).xyz(Visualizer.Depth), Quaternion.identity, drawSize * Vector3.one);*/

        public Matrix4x4 GetCurveOffsetMatrix()
            => Matrix4x4.Translate(new float3(.5f * thickness));

        public float2 PartitionSize
            => 2f * (axisScale) / (float2) tickCount;

        public Material GetMaterial() //not necessary to update all uniforms everytime but for now
        {

            material.SetColor("_Color", color);
            material.SetVector("_TickCount", ((float2) tickCount).xyzw());
            material.SetVector("_Scale", axisScale.xyzw());
            material.SetFloat("_Thickness", thickness);
            material.SetFloat("_UVScale", .5f*drawSize);

            return material;
        }

        public void FillCurveMaterial(Material m)
        {
            m.SetVector("_Scale", PartitionSize.xyzw());
            m.SetVector("_SizeBounds", sizeBounds.xyzw());
        }

    }

    public struct CurvePreferences
    {
        static Shader shader = Shader.Find("Unlit/DataCurve");

        public float lineThickness;
        public Color color;

        public Material GetMaterial()
            => GetMaterial(new(shader));
        public Material GetMaterial(Material m)
        {
            m.SetFloat("_Thickness", lineThickness);
            m.SetColor("_Color", color);
            return m;
        }
    }

    public class Visualizer
    {
        //
        public static float Depth = 1f;

        public Visualizer(Camera camera, GraphPreferences prefs)
        { this.camera = camera; this.graphPrefs = prefs; }

        Camera camera;

        GraphPreferences graphPrefs;
        List<(Mesh, Material)> curves = new();

        static readonly int layer = LayerMask.NameToLayer("Visualization");

        //
        static Material GetCurveMaterial(GraphPreferences graphPrefs, CurvePreferences curvePrefs)
        {
            var m = curvePrefs.GetMaterial();
            graphPrefs.FillCurveMaterial(m);
            return m;
        }
        static Material GetCurveMaterial(Material m, GraphPreferences graphPrefs)
        {
            graphPrefs.FillCurveMaterial(m);
            return m;
        }
        static Material GetCurveMaterial(Material m, CurvePreferences curvePrefs)
            => curvePrefs.GetMaterial(m);

        //
        public void AddCurve(MatchupData data, CurvePreferences curvePrefs, float k = 0f)
            => curves.Add((MeshGenerator.GenerateCurveMesh(data, k), GetCurveMaterial(graphPrefs, curvePrefs)));

        void SetPreferences(int i, CurvePreferences curvePrefs)
            => curves[i] = (curves[i].Item1, GetCurveMaterial(curves[i].Item2, curvePrefs));

        void SetPreferences(GraphPreferences graphPrefs)
        { this.graphPrefs = graphPrefs; Utils.ForLength(curves.Count, (i) => curves[i] = (curves[i].Item1, GetCurveMaterial(curves[i].Item2, graphPrefs))); }

        float zoom = 8f;
        public void UpdateInput(float dt, float change, float2 mouse) //TODO: use the new matrix system, inverse mat with z = same plane or smth
        {
            zoom += math.step(math.abs(mouse - (graphPrefs.position + graphPrefs.drawSize * .5f)), graphPrefs.drawSize*.5f).mul() != 0 ? change * dt : 0f;
            graphPrefs.tickCount = (int)zoom;
            SetPreferences(graphPrefs);
        }

        public void Draw()
        {

            Matrix4x4 curveMatrix = UnityReferences.VisualizerGraphTransform.localToWorldMatrix * graphPrefs.GetCurveOffsetMatrix();

            //Draw Graph
            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.VisualizerGraphTransform.localToWorldMatrix, graphPrefs.GetMaterial(), layer); 

            //Draw Curves
            curves.ForEach(tup =>
            {
                Graphics.DrawMesh(tup.Item1, curveMatrix, tup.Item2, layer, camera);
            });
        }

    }
}