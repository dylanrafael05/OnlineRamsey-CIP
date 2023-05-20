using Ramsey.Board;
using Ramsey.Graph;
using System.Collections.Generic;
using Unity.Mathematics;

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

public class NodeEditingMode : IUserMode
{
    private Node selection;

    public void Init(BoardManager board)
    {
        selection = null;
    }
    
    bool CollideNode(float2 mouse, Node n, BoardManager board)
        => math.length(mouse - n.Position) <= board.Preferences.drawingPreferences.nodeRadius;

    public void Update(InputData input, BoardManager board)
    {
    }

    public void End(BoardManager board)
    {
        throw new System.NotImplementedException();
    }
}