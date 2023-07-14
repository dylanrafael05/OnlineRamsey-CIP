using Ramsey.Board;
using Ramsey.Utilities;
using Ramsey.Gameplayer;
using UnityEngine;
using System.Collections.Generic;
using Ramsey.Drawing;
using System.Collections;

namespace Ramsey.UI
{
    public class TurnNavigatorMode : IUserMode<BoardManager>
    {
        public const float RecorderOffset = 1.5f;
        public const float RecorderForce = 0.2f;
        public const float RecorderTolerance = 0.01f;

        private static float RecorderY
        {
            get => UnityReferences.RecordingTransform.localPosition.y;
            set => UnityReferences.RecordingTransform.localPosition 
                = new Vector3(UnityReferences.RecordingTransform.localPosition.x, value, UnityReferences.RecordingTransform.localPosition.z);
        }
        
        private IEnumerator Slide(float target) 
        {
            var dir = Mathf.Sign(target - RecorderY);
            
            while(Mathf.Abs(RecorderY - target) > RecorderTolerance && dir == Mathf.Sign(target - RecorderY))
            {
                RecorderY += dir * RecorderForce * Time.deltaTime;
                RecorderY = Mathf.LerpUnclamped(RecorderY, (RecorderY * 39 + target) / 40, Time.deltaTime * 60f);
                yield return null;
            }
            
            RecorderY = target;
        }

        private float upperpos;
        private float lowerpos;
        private Coroutine sliding;
        
        private bool recorderOffScreen = true;

        public void Init(BoardManager board) 
        { 
            lowerpos = RecorderY;
            upperpos = RecorderY + RecorderOffset;

            RecorderY = upperpos;
        }

        public void Update(InputData input, BoardManager board)
        {
            if(input.recorderToggled)
            {
                Coroutines.Kill(sliding);
                sliding = Coroutines.Start(Slide(recorderOffScreen ? lowerpos : upperpos));
                recorderOffScreen = !recorderOffScreen;
            }

            if (!recorderOffScreen)
            {
                int d = -1 * input.lkd.ToInt() + 1 * input.rkd.ToInt();
                board.OffsetTurn(d);
            }

            IMove.Enable = board.IsCurrentTurn;
            UserModeHandler<BoardManager>.GameplayModes.Foreach(m => UserModeHandler<BoardManager>.SetStatus(m, board.IsCurrentTurn));

        }
        public void End(BoardManager board) 
        { 
            RecorderY = lowerpos;
            recorderOffScreen = true;
        }

        public bool IsGameplayMode => false;
    }
}