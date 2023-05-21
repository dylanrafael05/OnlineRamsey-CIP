using Unity.Mathematics;
using UnityEngine;
using Ramsey.Utilities;
using Ramsey.Screen;

public static class InputManager
{
    public static float3 GetScreenMousePosition()
        => Input.mousePosition;

    public static float2 GetWorldMousePosition()
        => (Vector2)CameraManager.BoardCamera.ScreenToWorldPoint(GetScreenMousePosition().rescale(CameraManager.ScreenSize, CameraManager.BoardSize));

    //Later change to input struct?
    // public static (float2 mouse, bool lmbp, bool rmbp) GetInput()
    //     => (GetWorldMousePosition(), Input.GetMouseButtonDown(0), Input.GetMouseButtonDown(1));

    private static readonly InputData data = new();
    public static InputData GetInput()
    {
        data.mouse = GetWorldMousePosition();

        data.lmb  = Input.GetMouseButton(0);
        data.lmbp = Input.GetMouseButtonDown(0);
        
        data.mmb  = Input.GetMouseButton(2);
        data.mmbp = Input.GetMouseButtonDown(2);
        
        data.rmb  = Input.GetMouseButton(1);
        data.rmbp = Input.GetMouseButtonDown(1);


        return data;
    }   
}

public class InputData
{

    public float2 mouse;
    public bool lmb; public bool rmb; public bool mmb;
    public bool lmbp; public bool rmbp; public bool mmbp;

}