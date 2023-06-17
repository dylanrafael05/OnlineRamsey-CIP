using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Ramsey.Drawing;
using Ramsey.Utilities;

namespace Ramsey.Screen
{
    public class CameraManager
    {
        public static Camera BoardCamera => insBoardCamera;
        public static Camera ScreenCamera => insScreenCamera;
        static Camera insBoardCamera;
        static Camera insScreenCamera;

        public static float2 BoardSize => insBoardCamera.pixelRect.size;
        public static float2 ScreenSize => insScreenCamera.pixelRect.size;

        public static void Create(Camera screenCamera, Camera boardCamera)
        {
            //
            insBoardCamera ??= boardCamera;
            insScreenCamera ??= screenCamera;

            //
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32) { name = "Board Texture" };
            renderTexture.Create();
            boardCamera.targetTexture = renderTexture;

            //
            GameObject.Find("Board Plane").GetComponent<MeshRenderer>().material = UnityReferences.ScreenMaterial;
            UnityReferences.ScreenMaterial.SetTexture(Shader.PropertyToID("_ScreenTexture"), renderTexture);
        }

        public static void Update() 
        {
            UnityReferences.BackgroundRenderer.transform.localPosition = new Vector3(
                x: BoardCamera.transform.localPosition.x,
                y: BoardCamera.transform.localPosition.y,
                z: UnityReferences.BackgroundRenderer.transform.localPosition.z
            );
        }

    }

}
