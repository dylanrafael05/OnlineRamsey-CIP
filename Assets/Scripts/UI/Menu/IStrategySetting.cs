using System.Collections.Generic;
using Ramsey.Gameplayer;
using Ramsey.Utilities.UI;

namespace Ramsey.UI
{
    public interface IStrategySetting<out T> where T : IPlayer
    {
        IStrategyInitializer<T> Initializer { get; }

        IReadOnlyList<InputBox> Inputs { get; }
        
        void ShowTextInputs();
        void HideTextInputs();
        
        bool InputIsValid();
        
        T Initialize();
    }
}
