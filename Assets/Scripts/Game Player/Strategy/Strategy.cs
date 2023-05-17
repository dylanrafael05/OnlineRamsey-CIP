using Ramsey.Core;
using UnityEngine.Assertions;

public interface IBuilder
{
    BuilderMove GetMove(IGraphView gameState);
}

public interface IPainter
{
    PainterMove GetMove(IGraphView gameState);
}

public class BuilderMove
{

    Node n1; Node n2;
    public BuilderMove(Node n1, Node n2)
    { this.n1 = n1; this.n2 = n2; }

    public void MakeMove(BoardManager board)
        => board.CreateEdge(n1, n2, -1);

}

public class PainterMove
{
    Edge edge; int type;
    public PainterMove(Edge edge, int type)
    { this.edge = edge; this.type = type; }

    public void MakeMove(BoardManager board)
        => board.PaintEdge(edge, type);
}

public static class GameAlgorithms
{

    //Methods take in IGraphView return a bool or something

}