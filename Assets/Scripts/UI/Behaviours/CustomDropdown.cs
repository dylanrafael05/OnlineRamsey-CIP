using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

using Dropdown = TMPro.TMP_Dropdown;

namespace Ramsey.UI 
{
    public class DropdownWrapper<T>
    {
        public Dropdown UI { get; private set; }
        
        private Dictionary<string, Func<T>> items;

        public DropdownWrapper(Dropdown dropdown, params (string name, Func<T> fn)[] items)
        {
            this.UI = dropdown;
            this.items = items.ToDictionary(i => i.name, i => i.fn);

            dropdown.options = items.Select(i => new Dropdown.OptionData(i.name)).ToList();
        }

        public T Selected => items[UI.options[UI.value].text]();
    }
}