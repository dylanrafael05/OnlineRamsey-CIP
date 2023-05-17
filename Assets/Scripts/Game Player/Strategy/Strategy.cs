using Ramsey.Core;
using System.Threading.Tasks;
using UnityEngine.Assertions;

public interface IBuilder
{
    Task<BuilderMove> GetMove(GameState gameState);
}

public interface IPainter
{
    Task<PainterMove> GetMove(GameState gameState);
}


public readonly struct BuilderMove
{
    private readonly Node n1;
    private readonly Node n2;

    public BuilderMove(Node n1, Node n2)
    {
        this.n1 = n1;
        this.n2 = n2;
    }

    public bool MakeMove(BoardManager board)
    {
        if (n1 is null || n2 is null || !board.IsValidEdge(n1, n2))
            return false;

        board.CreateEdge(n1, n2, -1);
        return true;
    }

}

public readonly struct PainterMove
{
    private readonly Edge edge; 
    private readonly int type;

    public PainterMove(Edge edge, int type)
    { 
        this.edge = edge; 
        this.type = type; 
    }

    public bool MakeMove(BoardManager board)
    {
        if (edge.Type != Edge.NullType) 
            return false;

        board.PaintEdge(edge, type);
        return true;
    }
}