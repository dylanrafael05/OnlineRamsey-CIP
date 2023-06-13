using Ramsey.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

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
