using Ramsey.Board;
using System.Collections.Generic;
using Ramsey.Utilities;
using System.Linq;

namespace Ramsey.UI
{
    public static class UserModeHandler<D>
    {
        public static void Create(D data)
        {
            UserModeHandler<D>.data = data;
            currentModes = new();
        }

        static D data;
        static List<IUserMode<D>> currentModes = new();
        static List<bool> activationStatuses = new();

        public static IEnumerable<IUserMode<D>> Modes => currentModes;

        public static void Update(InputData input)
            => currentModes.ForEachIndex((m, i) => { if (activationStatuses[i]) m.Update(input, data); });

        public static void AddMode(IUserMode<D> mode)
        {
            mode.Init(data);

            currentModes.Add(mode);
            activationStatuses.Add(true);
        }

        public static void DelMode(IUserMode<D> mode)
        {
            mode.End(data);

            int i = currentModes.FindIndex(m => m == mode);
            currentModes.RemoveAt(i);
            activationStatuses.RemoveAt(i);
        }

        public static void SetStatus(IUserMode<D> mode, bool status)
        { var i = currentModes.FindIndex(m => m == mode); if (i != -1) activationStatuses[i] = status; }

        public static void SetAllStatuses(bool status)
        {
            foreach(var mode in Modes)
                SetStatus(mode, status);
        }

        public static IEnumerable<IUserMode<D>> GameplayModes => Modes.Where(m => m.IsGameplayMode);

    }

    public interface IUserMode<D>
    {
        void Init(D data);
        void Update(InputData input, D data);
        void End(D data);

        bool IsGameplayMode { get; }

    }
}