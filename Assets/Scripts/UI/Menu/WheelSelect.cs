using Ramsey.Utilities;
using Unity.Mathematics;
using UnityEngine;
using Ramsey.Drawing;

using static Unity.Mathematics.math;

namespace Ramsey.UI
{
    internal class WheelSelect
    {

        // Prefs
        readonly float radius;
        readonly float wheelThickness;
        readonly float2 tickDim;
        readonly int tickCount;

        readonly float tickCollisionSize;

        readonly float knobSize;

        // Current
        public int CurrentTick { get; private set; }
        public float2 KnobPos 
            => UnityReferences.WheelSelectTransform.position.xy() 
                + UnityReferences.WheelSelectTransform.lossyScale.xy() * float2(radius, (CurrentTick + .5f) * MathUtils.TAU / tickCount).ToCartesian();
        bool hasNode;

        //
        Material material;

        public WheelSelect(float radius, float wheelThickness, float2? tickDim, int tickCount, float knobSize, float tickCollisionSize = 0.3f)
        {
            this.radius = radius;
            this.wheelThickness = wheelThickness;
            this.tickCount = tickCount;
            this.tickCollisionSize = tickCollisionSize;
            this.knobSize = knobSize;

            //
            material = new(Shader.Find("Unlit/UIShaders/WheelSelect"));

            material.SetColor("_Color", Color.white);
            material.SetFloat("_WheelRadius", radius);
            material.SetFloat("_WheelThickness", wheelThickness);

            material.SetInteger("_TickCount", tickCount);
            if(tickDim != null) material.SetVector("_TickDim", ((float2) tickDim).xyzw());

            material.SetFloat("_NodeRadius", knobSize);
        }

        bool CollideKnob(float2 mouse)
        {
            //mouse -= UnityReferences.WheelSelectTransform.position.xy(); Debug.Log(mouse);
            //mouse /= UnityReferences.WheelSelectTransform.localScale.xy();
            mouse = UnityReferences.WheelSelectTransform.InverseTransformPoint(mouse.xyz()).xy(); //(UnityReferences.WheelSelectTransform.worldToLocalMatrix * (Vector4) mouse.xyzw(0f, 1f)).xy();//
            float2 pos = float2(radius, (MathUtils.TAU * (CurrentTick*1f+0.5f) / tickCount)).ToCartesian();
            return (length(mouse - pos) - knobSize) <= 0f;
        }
        public int Update(float2 mouse, bool isDown, bool isPress)
        {
            //
            hasNode |= CollideKnob(mouse) && isPress;
            hasNode &= isDown;

            if (!hasNode) return CurrentTick;

            //
            float2 polar = UnityReferences.WheelSelectTransform.InverseTransformPoint(mouse.xyz()).xy().ToPolar();
            float partitionSize = MathUtils.TAU / tickCount;
            float rtheta = fmod(polar.y, partitionSize)-partitionSize*.5f;
            if (abs(rtheta) > tickCollisionSize * .5f) return CurrentTick;

            //
            float id = (polar.y - rtheta - partitionSize * .5f) / partitionSize;
            CurrentTick = (int) id;

            return CurrentTick;
        }

        public void Draw()
        {

            material.SetInteger("_NodeLocation", CurrentTick);

            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.WheelSelectTransform.localToWorldMatrix, material, UnityReferences.BoardLayer);

        }

    }
}
