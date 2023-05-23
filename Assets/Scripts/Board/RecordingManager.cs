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
        int selectedID;
        public bool IsCurrentTurn => selectedID == boardStates.Count - 1;

        public void Add(BoardState state)
        {
            boardStates.Add(state);
        }
        public void LoadTurn(int i, DrawingIOInterface drawingWritingInterface)
        {
            Assert.IsTrue(0 <= i && i < boardStates.Count);
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