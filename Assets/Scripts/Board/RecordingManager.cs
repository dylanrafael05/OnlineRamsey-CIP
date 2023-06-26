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
        #if HEADLESS
            public void AddCurrentTurn() {}
            public void LoadTurn(int _) {}
            public void OffsetTurn() {}
            public RecordingManager(BoardManager _) {}
        #else
            List<BoardState> boardStates;
            int selectedID = -1;
            
            BoardManager board;

            public RecordingManager(BoardManager board)
            {
                this.board = board; 
                boardStates = new();

                AddCurrentTurn();
            }

            //
            void UpdateRecorder() => board.RenderIO.UpdateRecorder(boardStates.Count, selectedID);

            //
            public bool IsCurrentTurn => selectedID == boardStates.Count - 1;

            public void AddCurrentTurn()
            {
                boardStates.Add((new BoardState(board.RenderIO.CreateDrawState())));

                if (selectedID != boardStates.Count-2) return;

                selectedID++;
                UpdateRecorder();
            }
            public void LoadTurn(int i)
            {
                if(!(0 <= i && i < boardStates.Count) || i == selectedID) return;
                selectedID = i;

                if (selectedID != boardStates.Count - 1) board.RenderIO.LoadDrawState(boardStates[i].DrawState);
                else board.RenderIO.LoadDrawState();
                board.RenderIO.UpdateRecorder(boardStates.Count, selectedID);
            }

            public void OffsetTurn(int d) 
                => LoadTurn(selectedID + d);
        #endif
    }

    internal struct BoardState
    {

        public BoardState(DrawState DrawState) => this.DrawState = DrawState;
        public DrawState DrawState { get; private set; }

    }
}