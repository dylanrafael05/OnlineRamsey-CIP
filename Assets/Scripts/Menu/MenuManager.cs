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

namespace Ramsey.Menu
{
    public class MenuManager
    {

        MenuDrawer drawer;
        public MenuManager(Camera camera)
            => drawer = new(camera);

        public void UpdateWheel(float2 mouse)
        {

        }

    }

    internal class WheelSelect
    {

        // Prefs
        readonly float2 pos;
        readonly float radius;
        readonly float wheelThickness;
        readonly int ticks;

        readonly float tickCollisionSize;

        readonly float knobSize;

        // Current
        int currentTick;
        bool hasNode;

        bool CollideKnob(float2 mouse)
        {
            float2 pos = float2(radius, (PI * 2f * ((float)currentTick) / ((float)ticks))).ToCartesian();
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
            float partitionSize = 2f * PI / ticks;
            float rtheta = fmod(polar.y, partitionSize)-partitionSize*.5f;

            if (abs(rtheta) > tickCollisionSize * .5f) return;

            //
            float id = (polar.y - rtheta - partitionSize * .5f) / partitionSize;
            currentTick = (int) id;
        }

        public void Draw()
        {

            // Graphics.DrawMesh(Ramsey.Drawing.)

        }

    }
}
