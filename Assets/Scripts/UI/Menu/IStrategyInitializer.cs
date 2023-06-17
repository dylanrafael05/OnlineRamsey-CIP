using System.Collections.Generic;
using Unity.Mathematics;
using Ramsey.Gameplayer;
using Ramsey.Utilities.UI;

namespace Ramsey.UI
{
    public interface IStrategyInitializer<out T> where T : IPlayer
    {
        IReadOnlyList<TextParameter> Parameters { get; }
        IReadOnlyList<InputBox> Inputs { get; }

        void ShowTextInputs();
        void HideTextInputs();

        T Initialize();

        bool InputIsValid();

        string Name { get; }

        bool IsDeterministic { get; }
        bool IsAutomated { get; }
    }
}
