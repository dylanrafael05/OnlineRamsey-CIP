using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using Ramsey.Drawing;

using static Unity.Mathematics.math;

namespace Ramsey.UI
{

    /*public interface IStrategyParameter
    {

    }*/

    public interface IPainterInitializer
    {
        int ParameterCount { get; }

        Painter 
    }

    public class MenuManager
    {

        
        public MenuManager(float wheelRadius, float wheelThickness, )
        {

        }


    }

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
        bool hasNode;

        //
        Material material;

        public WheelSelect(float radius, float wheelThickness, float2 tickDim, int tickCount, float tickCollisionSize, float knobSize)
        {
            this.radius = radius;
            this.wheelThickness = wheelThickness;
            this.tickCount = tickCount;
            this.tickCollisionSize = tickCollisionSize;
            this.knobSize = knobSize;

            //
            material = new(Shader.Find("Unlit/UIShaders/WheelShader"));

            material.SetVector("_Color", Color.white);
            material.SetFloat("_Radius", radius);

            material.SetInt("_TickCount", tickCount);
            material.SetVector("_TickDim", tickDim.xyzw());

            material.SetFloat("_NodeRadius", knobSize);
        }

        bool CollideKnob(float2 mouse)
        {
            mouse -= UnityReferences.WheelSelectTransform.position.xy();
            float2 pos = float2(radius, (PI * 2f * CurrentTick / tickCount)).ToCartesian();
            return (length(mouse - pos) - radius) <= 0f;
        }
        public void Update(float2 mouse, bool isDown)
        {
            //
            hasNode |= CollideKnob(mouse) && hasNode;
            hasNode |= CollideKnob(mouse) && isDown;

            if (!hasNode) return;

            //
            float2 polar = mouse.ToPolar();
            float partitionSize = 2f * PI / tickCount;
            float rtheta = fmod(polar.y, partitionSize)-partitionSize*.5f;

            if (abs(rtheta) > tickCollisionSize * .5f) return;

            //
            float id = (polar.y - rtheta - partitionSize * .5f) / partitionSize;
            CurrentTick = (int) id;
        }

        public void Draw()
        {

            material.SetInt("_NodeLocation", CurrentTick);
            Graphics.DrawMesh(MeshUtils.QuadMesh, UnityReferences.WheelSelectTransform.localToWorldMatrix, UnityReferences.WheelMaterial, UnityReferences.BoardLayer);

        }

    }
}
