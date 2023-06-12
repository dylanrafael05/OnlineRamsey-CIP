using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

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

        bool CollideKnob()
        {
            return false;
        }

        public void Update(float2 mouse)
        {

        }

    }
}
