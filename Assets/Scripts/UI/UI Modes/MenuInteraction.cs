﻿using Ramsey.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using Ramsey.UI;

namespace Ramsey.UI
{
    public class MenuInteraction : IUserMode<MenuManager>
    {

        public void Init(MenuManager m)
        {

        }

        public void Update(InputData input, MenuManager m)
        {
            float2 mouse = input.mouse;
        }

        public void End(MenuManager m)
        {

        }

        public bool IsGameplayMode => false;

    }
}
