using Ramsey.Drawing;
using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

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
        readonly float thetaNormalizedOffset;

        readonly float tickCollisionSize;

        readonly float smallKnobSize;
        readonly float largeKnobSize;

        // Current
        float currentKnobSize;
        public int CurrentTick { get; private set; }
        float currentContinuousTick;
        public float2 KnobPos
            => UnityReferences.WheelSelectTransform.position.xy()
                + UnityReferences.WheelSelectTransform.lossyScale.xy() * float2(radius, (CurrentTick + .5f) * MathUtils.TAU / tickCount).ToCartesian();
        bool hasNode;

        //
        Material material;

        public WheelSelect(float radius, float wheelThickness, int tickCount, float nodeSize, float tickCollisionSize = 0.3f, float thetaNormalizedOffset = 0.5f)
        {
            this.radius = radius;
            this.wheelThickness = wheelThickness;
            this.tickCount = tickCount;
            this.tickCollisionSize = tickCollisionSize;
            this.smallKnobSize = nodeSize*1.25f;
            this.largeKnobSize = nodeSize * 1.5f;
            this.currentKnobSize = smallKnobSize;
            this.thetaNormalizedOffset = thetaNormalizedOffset;

            //
            material = new(UnityReferences.WheelShader);

            material.SetColor("_BaseColor", Color.black);
            material.SetColor("_NodeColor", Color.white);
            material.SetFloat("_WheelRadius", radius);
            material.SetFloat("_WheelThickness", wheelThickness);
            material.SetFloat("_ThetaNormalizedOffset", thetaNormalizedOffset);

            material.SetInteger("_TickCount", tickCount);

            material.SetFloat("_NodeRadius", nodeSize);
        }

        bool CollideKnob(float2 mouse)
        {
            //mouse -= UnityReferences.WheelSelectTransform.position.xy(); Debug.Log(mouse);
            //mouse /= UnityReferences.WheelSelectTransform.localScale.xy();

            float partitionSize = MathUtils.TAU / tickCount;
            float r = radius * cos(.5f * partitionSize) / cos(MathUtils.amod((currentContinuousTick+0.001f) * partitionSize, partitionSize) - .5f * partitionSize);

            mouse = UnityReferences.WheelSelectTransform.InverseTransformPoint(mouse.xyz()).xy(); //(UnityReferences.WheelSelectTransform.worldToLocalMatrix * (Vector4) mouse.xyzw(0f, 1f)).xy();//
            float2 pos = float2(r, (MathUtils.TAU * (currentContinuousTick+thetaNormalizedOffset) / tickCount)).ToCartesian();
            return ((length(mouse - pos) - currentKnobSize) > 0f);
        }
        public int Update(float2 mouse, bool isDown, bool isPress, float dt)
        {
            //
            hasNode |= CollideKnob(mouse) && isPress;
            hasNode &= isDown;

            //
            currentKnobSize = lerp(currentKnobSize, hasNode ? largeKnobSize : smallKnobSize, dt * 30.0f); //technically inaccurate dt should be exponential but whatever

            if (!hasNode)
            {
                currentContinuousTick -= fmod(currentContinuousTick + .5f, 1.0f) - .5f;
                CurrentTick = (int)(currentContinuousTick + 0.001f);
                return CurrentTick;
            }

            //
            float2 polar = UnityReferences.WheelSelectTransform.InverseTransformPoint(mouse.xyz()).xy().ToPolar();
            float partitionSize = MathUtils.TAU / tickCount;
            polar.y -= thetaNormalizedOffset;
            float rtheta = fmod(polar.y+partitionSize*.5f, partitionSize) - partitionSize * .5f;
            currentContinuousTick = (polar.y) / partitionSize;
            if (abs(rtheta) > tickCollisionSize*.5f) return CurrentTick;

            //
            float id = ((polar.y - rtheta + 0.001f) / partitionSize) % tickCount;
            CurrentTick = (int)id;
            currentContinuousTick = CurrentTick;

            return CurrentTick;
        }

        public void Draw()
        {

            material.SetFloat("_KnobLocation", currentContinuousTick);
            material.SetFloat("_KnobRadius", currentKnobSize);

            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.WheelSelectTransform.localToWorldMatrix, material, UnityReferences.ScreenLayer);

        }

    }
}
