using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public static class EngineTransformGenerator
{

    public static Matrix4x4 GenerateEdgeTransform(float2 start, float2 end, float thickness)
    {

        float2 pos = (start + end) * .5f;
        float2 scale = new float2(length(start - end) * .5f, thickness * .5f);
        float theta = atan2((end - start).y, (end-start).x);

        return Matrix4x4.TRS(new Vector3(pos.x, pos.y, 0f), Quaternion.Euler(0f, 0f, Mathf.Rad2Deg*theta), new Vector3(scale.x, scale.y, 1f));

    }

}