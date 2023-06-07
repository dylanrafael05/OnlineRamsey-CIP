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

        static Material material = new(Shader.Find("Unlit/DataGraph"));

        public float2 sizeBounds; //could add some dithering on the curve as it leaves ok i did but barely visible since the line is so thin
        public float2 scale;
        public float2 position;

        public Color color;
        public float thickness;

        public float drawSize;

        public Matrix4x4 GetCurveMatrix()
            => Matrix4x4.TRS(position.xyz(Visualizer.Depth), Quaternion.identity, Vector3.one);

        public Matrix4x4 GetGraphMatrix()
            => Matrix4x4.TRS((position + drawSize * .5f).xyz(Visualizer.Depth), Quaternion.identity, drawSize * Vector3.one);

        public Material GetMaterial(int2 tickCount) //not necessary to update all uniforms everytime but for now
        {

            material.SetColor("_Color", color);
            material.SetVector("_TickCount", ((float2) tickCount).xyzw());
            material.SetVector("_Scale", scale.xyzw());
            material.SetFloat("_Thickness", thickness);
            material.SetFloat("_UVScale", drawSize);

            return material;
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
        List<(Mesh, Material)> graphs = new();

        static int layer = LayerMask.NameToLayer("Visualization");

        //
        static Material GetCurveMaterial(GraphPreferences graphPrefs, CurvePreferences curvePrefs)
        {
            var m = curvePrefs.GetMaterial();
            m.SetVector("_Scale", graphPrefs.scale.xyzw());
            m.SetVector("_SizeBounds", graphPrefs.sizeBounds.xyzw());
            return m;
        }
        static Material GetCurveMaterial(Material m, GraphPreferences graphPrefs)
        {
            m.SetVector("_Scale", graphPrefs.scale.xyzw());
            m.SetVector("_SizeBounds", graphPrefs.sizeBounds.xyzw());
            return m;
        }
        static Material GetCurveMaterial(Material m, CurvePreferences curvePrefs)
            => curvePrefs.GetMaterial(m);

        //
        public void AddCurve(MatchupData data, CurvePreferences curvePrefs, float k = 1f)
            => graphs.Add((MeshGenerator.GenerateCurveMesh(data, k), GetCurveMaterial(graphPrefs, curvePrefs)));

        public void SetPreferences(int i, CurvePreferences curvePrefs)
            => graphs[i] = (graphs[i].Item1, GetCurveMaterial(graphs[i].Item2, curvePrefs));

        public void SetPreferences(GraphPreferences graphPrefs)
            => graphs.ForEachIndex((g, i) => graphs[i] = (g.Item1, GetCurveMaterial(g.Item2, graphPrefs)));

        public void Draw()
        {

            Matrix4x4 curveMatrix = graphPrefs.GetCurveMatrix();

            //Draw Graph (ima fix shader later)
            Graphics.DrawMesh(MeshUtils.QuadMesh, graphPrefs.GetGraphMatrix(), graphPrefs.GetMaterial(new(20)), layer); //need to make tick count consistent with meshes and stuff.. uniforming curve scale and like ye

            //Draw Curves
            graphs.ForEach(tup =>
            {
                Graphics.DrawMesh(tup.Item1, curveMatrix, tup.Item2, layer, camera);
            });
        }

    }
}