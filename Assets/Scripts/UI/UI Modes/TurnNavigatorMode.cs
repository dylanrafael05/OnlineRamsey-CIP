using Ramsey.Board;
using Ramsey.Utilities;
using Ramsey.Gameplayer;
using UnityEngine;
using System.Collections.Generic;
using Ramsey.Drawing;
using System.Collections;

namespace Ramsey.UI
{
    public class TurnNavigatorMode : IUserMode
    {
        public const float RecorderOffset = 3f;
        public const float RecorderForce = 1f;
        public const float RecorderTolerance = 0.05f;

        private static float RecorderY
        {
            get => UnityReferences.RecordingTransform.position.y;
            set => UnityReferences.RecordingTransform.position 
                = new Vector3(UnityReferences.RecordingTransform.position.x, value, UnityReferences.RecordingTransform.position.z);
        }
        
        private IEnumerator Slide(float target) 
        {
            sliding = true;
            var dir = Mathf.Sign(target - RecorderY);
            
            while(Mathf.Abs(RecorderY - target) > RecorderTolerance)
            {
                RecorderY += dir * RecorderForce * Time.deltaTime;
                RecorderY = Mathf.LerpUnclamped(RecorderY, (RecorderY * 39 + target) / 40, Time.deltaTime * 60f);
                yield return null;
            }
            
            RecorderY = target;
            sliding = false;
        }

        private float upperpos;
        private float lowerpos;
        private bool sliding;
        
        private bool recorderOffScreen = true;
        bool IUserMode.AltersBoard => false;

        public void Init(BoardManager board) 
        { 
            lowerpos = RecorderY;
            upperpos = RecorderY + RecorderOffset;

            RecorderY = upperpos;
        }

        public void Update(InputData input, BoardManager board)
        {
            if(!sliding && input.recorderToggled)
            {
                Coroutines.StartCoroutine(Slide(recorderOffScreen ? lowerpos : upperpos));
                recorderOffScreen = !recorderOffScreen;
            }
            
            if(!recorderOffScreen)
            {
                int d = -1 * input.lkd.ToInt() + 1 * input.rkd.ToInt();
                board.OffsetTurn(d);

                IMove.Enable = board.IsCurrentTurn;
                foreach(var mode in UserModeHandler.BoardAlteringModes)
                {
                    UserModeHandler.SetStatus(mode, board.IsCurrentTurn);
                }
            }
        }
        public void End(BoardManager board) { }

    }
}