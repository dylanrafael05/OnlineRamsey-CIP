using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ramsey.Screen
{
    
    public class CameraManager
    {

        public static Camera BoardCamera => insBoardCamera;
        static Camera insBoardCamera;

        Camera screenCamera;
        Camera boardCamera;

        public CameraManager(Camera screenCamera, Camera boardCamera)
        {
            this.screenCamera = screenCamera;
            this.boardCamera = boardCamera;

            //
            insBoardCamera ??= boardCamera;

            //
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 16, RenderTextureFormat.ARGB32) { name = "Board Texture" };
            renderTexture.Create();
            boardCamera.targetTexture = renderTexture;

            //
            Material m = new Material(Shader.Find("Unlit/Fullscreen/Vignette"));
            GameObject.Find("Board Plane").GetComponent<MeshRenderer>().material = m;
            m.SetTexture(Shader.PropertyToID("_ScreenTexture"), renderTexture);
        }

    }

}
