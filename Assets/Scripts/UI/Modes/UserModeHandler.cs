using Ramsey.Board;
using System.Collections.Generic;

public class UserModeHandler
{

    static List<IUserMode> currentModes;

    public static void Update(InputData input, BoardManager board)
        => currentModes.ForEach(m => m.Update(input, board));

    public static void AddMode(IUserMode mode)
    {
        currentModes.Add(mode);
        mode.Init();
    }

    public static void DelMode(IUserMode mode)
    {
        currentModes.Remove(mode);
        mode.End();
    }

}

public interface IUserMode
{

    void Init();
    void Update(InputData input, BoardManager board);
    void End();

}