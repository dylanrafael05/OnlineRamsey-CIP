using Unity.Mathematics;
using UnityEngine;
using Ramsey.Utilities;
using Ramsey.Screen;

public static class InputManager
{

    //Later change to input struct?
    public static (float2 mouse, bool lmbp, bool rmbp) GetInput()
        => (CameraManager.BoardCamera.ScreenToWorldPoint(Input.mousePosition).xy(), Input.GetMouseButtonDown(0), Input.GetMouseButtonDown(1));
    

}

public struct InputStruct
{



}