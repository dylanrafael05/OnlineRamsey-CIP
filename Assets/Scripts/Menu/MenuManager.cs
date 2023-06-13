using Ramsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;

namespace Ramsey.Visualization
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
        readonly float thickness;
        readonly int ticks;

        readonly float knobSize;

        // Current
        int currentTick;
        bool hasNode;

        bool CollideKnob(float2 mouse)
        {
            float2 pos = float2(radius, (PI * 2f * ((float)currentTick) / ((float)ticks))).ToCartesian();
            return (length(mouse - pos) - radius) <= 0f;
        }

        /*public void Update(float2 mouse, bool isDown
        {
            *//*hasNode &= mouse.
            hasNode = hasNode *//*
        }*/

    }
}
