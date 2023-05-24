using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Ramsey.Graph;
using System.Collections.Generic;
using UnityEngine.Assertions;
using Ramsey.Drawing;

namespace Ramsey.Board
{
    internal class RecordingManager //Partial class of BoardManager
    {

        //
        List<BoardState> boardStates;
        int selectedID = -1;
        public bool IsCurrentTurn => selectedID == boardStates.Count - 1;

        public RecordingManager()
            => boardStates = new();

        public void Add(BoardState state, bool present)
        {
            boardStates.Add(state);
            selectedID++;
        }
        public void Add(BoardState state) => Add(state, true);
        public void LoadTurn(int i, DrawingIOInterface drawingWritingInterface)
        {
            Assert.IsTrue(0 <= i && i < boardStates.Count); if (i == selectedID) return;
            selectedID = i;

            drawingWritingInterface.LoadDrawState(boardStates[i].DrawState);
            drawingWritingInterface.UpdateRecorder(boardStates.Count, selectedID);
        }
        public void OffsetTurn(int d, DrawingIOInterface drawingWritingInterface) 
            => LoadTurn(selectedID + d, drawingWritingInterface);

    }

    internal struct BoardState
    {

        public BoardState(DrawState DrawState) => this.DrawState = DrawState;
        public DrawState DrawState { get; private set; }

    }
}