using Unity.Mathematics;
using UnityEngine;
using Ramsey.Utilities;

public static class InputManager
{

    //Later change to input struct?
    public static (float2 mouse, bool lmbp, bool rmbp) GetInput()
        => (((float3)Camera.main.ScreenToWorldPoint(Input.mousePosition)).xy, Input.GetMouseButtonDown(0), Input.GetMouseButtonDown(1));
    

}