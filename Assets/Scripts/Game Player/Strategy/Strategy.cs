using Ramsey.Core;
using UnityEngine.Assertions;

public interface IBuilder
{
    BuilderMove GetMove(GameState gameState);
    void MakeMove(BoardManager board);
}

public interface IPainter
{
    PainterMove GetMove(GameState gameState);
    void MakeMove(BoardManager board);
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

    public void MakeMove(BoardManager board)
        => board.CreateEdge(n1, n2, -1);

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

    public void MakeMove(BoardManager board)
        => board.PaintEdge(edge, type);
}