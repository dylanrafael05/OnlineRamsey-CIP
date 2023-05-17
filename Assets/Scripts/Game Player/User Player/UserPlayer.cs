using Ramsey.Core;
using Unity.Mathematics;
using static Unity.Mathematics.math;

public static class UserPlayer
{

    static Graph currentGraph;
    static Node selectedNode;

    public static void DoInput(float2 mouse)
    {

    }

}

public class UserInteraction
{

    public static UserInteraction Ins { get; private set; }

    public UserInteraction(BoardManager board, EngineManager drawer)
    {
        this.board = board;

        Ins ??= this;
    }

    BoardManager board;

    bool CollideNode(float2 mouse, Node n)
        => length(mouse - n.Position) <= board.Preferences.NodeRadius;

    public void DoInput(float2 mouse, bool lmbp)
    {

        bool hit = false;

        //board.((Node n) => hit = hit || CollideNode(mouse, n));


    }

}