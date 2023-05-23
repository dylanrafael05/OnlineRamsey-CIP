using Ramsey.Board;
using System.Collections.Generic;
using Ramsey.Utilities;
using System.Linq;

public static class UserModeHandler
{
    public static void Create(BoardManager board)
    {
        UserModeHandler.board = board;
        UserModeHandler.currentModes = new();
    }

    static BoardManager board;
    static List<IUserMode> currentModes;
    static List<bool> activationStatuses;

    public static void Update(InputData input)
        => currentModes.ForEachIndex((m, i) => { if (activationStatuses[i]) m.Update(input, board); });

    public static void AddMode(IUserMode mode)
    {
        currentModes.Add(mode);
        mode.Init(board);
    }

    public static void DelMode(IUserMode mode)
    {
        currentModes.Remove(mode);
        mode.End(board);
    }

    public static void SetStatus(IUserMode mode, bool status)
        => activationStatuses[currentModes.FindIndex(m => m == mode)] = status;

}

public interface IUserMode
{
    void Init(BoardManager board);
    void Update(InputData input, BoardManager board);
    void End(BoardManager board);
}
