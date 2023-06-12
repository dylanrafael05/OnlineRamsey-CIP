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
        static Camera insBoardCamera;
        static Camera insScreenCamera;

        public static float2 BoardSize => insBoardCamera.pixelRect.size;
        public static float2 ScreenSize => insScreenCamera.pixelRect.size;

        Camera screenCamera;
        Camera boardCamera;

        public CameraManager(Camera screenCamera, Camera boardCamera)
        {
            this.screenCamera = screenCamera;
            this.boardCamera = boardCamera;

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
            var scl = UnityReferences.BackgroundRenderer.transform.localScale;
            UnityReferences.BackgroundRenderer.transform.localPosition = new Vector3(
                x: MathUtils.Round(BoardCamera.transform.localPosition.x, scl.x * 0.01f * 20), //TODO: this 0.01f should be a uniform!,
                y: MathUtils.Round(BoardCamera.transform.localPosition.y, scl.y * 0.01f * 20), //TODO: this 0.01f should be a uniform!
                z: UnityReferences.BackgroundRenderer.transform.localPosition.z
            );
        }

    }

}
