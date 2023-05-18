using Unity.Mathematics;
using UnityEngine;
using Ramsey.Utilities;

public static class InputManager
{

    //Later change to input struct?
    public static (float2 mouse, bool lmbp, bool rmbp) GetInput()
        => (Camera.current.ScreenToWorldPoint(Input.mousePosition).xy(), Input.GetMouseButtonDown(0), Input.GetMouseButtonDown(1));
    

}