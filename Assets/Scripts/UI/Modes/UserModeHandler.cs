using Ramsey.Board;
using System.Collections.Generic;

public class UserModeHandler
{
    public static void Create(BoardManager board)
    {
        UserModeHandler.board = board;
        UserModeHandler.currentModes = new();
    }

    static BoardManager board;
    static List<IUserMode> currentModes;

    public static void Update(InputData input)
        => currentModes.ForEach(m => m.Update(input, board));

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

}

public interface IUserMode
{
    void Init(BoardManager board);
    void Update(InputData input, BoardManager board);
    void End(BoardManager board);
}
