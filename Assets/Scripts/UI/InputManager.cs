using Unity.Mathematics;
using UnityEngine;
using Ramsey.Utilities;
using Ramsey.Screen;

public static class InputManager
{

    //Later change to input struct?
    public static (float2 mouse, bool lmbp, bool rmbp) GetInput()
        => (((float3) CameraManager.BoardCamera.ScreenToWorldPoint(Input.mousePosition)).xy, Input.GetMouseButtonDown(0), Input.GetMouseButtonDown(1));
    

}

public class InputData
{

    public float2 mouse;
    public bool lmb; public bool rmb; public bool mmb;
    public bool lmbp; public bool rmbp; public bool mmbp;

}