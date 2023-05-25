using Ramsey.Board;
using System.Collections.Generic;
using Ramsey.Utilities;
using System.Linq;

namespace Ramsey.UI
{
    public static class UserModeHandler
    {
        public static void Create(BoardManager board)
        {
            UserModeHandler.board = board;
            currentModes = new();
        }

        static BoardManager board;
        static List<IUserMode> currentModes = new();
        static List<bool> activationStatuses = new();

        public static void Update(InputData input)
            => currentModes.ForEachIndex((m, i) => { if (activationStatuses[i]) m.Update(input, board); });

        public static void AddMode(IUserMode mode)
        {
            currentModes.Add(mode);
            activationStatuses.Add(true);

            mode.Init(board);
        }

        public static void DelMode(IUserMode mode)
        {
            int i = currentModes.FindIndex(m => m == mode);
            currentModes.RemoveAt(i);
            activationStatuses.RemoveAt(i);

            mode.End(board);
        }

        public static void SetStatus(IUserMode mode, bool status)
        { var i = currentModes.FindIndex(m => m == mode); if (i != -1) activationStatuses[i] = status; }

    }

    public interface IUserMode
    {
        void Init(BoardManager board);
        void Update(InputData input, BoardManager board);
        void End(BoardManager board);
    }
}