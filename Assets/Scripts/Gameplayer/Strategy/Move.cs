using Ramsey.Board;
using Ramsey.Graph;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Ramsey.Gameplayer
{
    public interface IMove
    {
        static bool Enable = true;
        bool MakeMove(BoardManager board, bool synchronous = false);
    }

    public readonly struct BuilderMove : IMove
    {
        private readonly Node n1;
        private readonly Node n2;

        public BuilderMove(Node n1, Node n2)
        {
            this.n1 = n1;
            this.n2 = n2;
        }

        public BuilderMove(Node n1, BoardManager board) : this(n1, board.CreateNode()) { } //Later make it create using the NodeSmoother which we'll make

        bool IsValid(BoardManager board)
            => IMove.Enable && n1 is not null && n2 is not null && board.IsValidEdge(n1, n2);

        public bool MakeMove(BoardManager board, bool synchronous = false)
        {
            if (!IsValid(board))
                return false;

            board.CreateEdge(n1, n2);
            return true;
        }

    }

    public readonly struct PainterMove : IMove
    {
        private readonly Edge edge;
        private readonly int type;

        public PainterMove(Edge edge, int type)
        {
            this.edge = edge;
            this.type = type;
        }

        bool IsValid(BoardManager board)
            => IMove.Enable && edge.Type == Edge.NullType && type != Edge.NullType;

        public bool MakeMove(BoardManager board, bool synchronous = false)
        {
            if (!IsValid(board))
                return false;

            board.PaintEdge(edge, type, synchronous);
            return true;
        }
    }
}