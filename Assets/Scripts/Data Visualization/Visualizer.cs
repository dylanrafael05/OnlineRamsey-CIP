using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Ramsey.Utilities;

namespace Ramsey.Visualization
{
    public struct GraphPreferences
    {

        public float2 sizeBounds; //could add some dithering on the curve as it leaves
        public float2 scale;
        public float2 position;
        //rotation, tick stuff..

        public Matrix4x4 GetMatrix()
            => Matrix4x4.TRS(position.xyz(Visualizer.Depth), Quaternion.identity, Vector3.one);

    }

    public struct CurvePreferences
    {
        static Shader shader = Shader.Find("Unlit/DataGraph");

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
            m.SetVector("_Scale", graphPrefs.scale.xyz().xyzw());
            m.SetVector("_SizeBounds", graphPrefs.sizeBounds.xyz().xyzw());
            return m;
        }
        static Material GetCurveMaterial(Material m, GraphPreferences graphPrefs)
        {
            m.SetVector("_Scale", graphPrefs.scale.xyz().xyzw());
            m.SetVector("_SizeBounds", graphPrefs.sizeBounds.xyz().xyzw());
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

            Matrix4x4 matrix = graphPrefs.GetMatrix();

            //Draw Graph

            //Draw Curves
            graphs.ForEach(tup =>
            {
                Graphics.DrawMesh(tup.Item1, matrix, tup.Item2, layer, camera);
            });
        }

    }
}