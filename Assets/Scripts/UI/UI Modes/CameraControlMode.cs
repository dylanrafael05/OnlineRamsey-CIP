using Ramsey.Board;
using Ramsey.Screen;
using Ramsey.Utilities;
using UnityEngine;

namespace Ramsey.UI
{
    public class CameraControlMode : IUserMode 
    {
        public const float TranslationAmount = 0.05f;
        public const float BaseSize = 5f;
        public const float ScrollMultiplier = 1.04f;
        public const float MinSize = 0.5f;
        public const float MaxSize = 20f;

        public void Init(BoardManager board) {}

        public void Update(InputData input, BoardManager board)
        {
            var scl = Input.GetKey(KeyCode.LeftShift).ToInt() * 2f + 1;

            var mscl = scl * CameraManager.BoardCamera.orthographicSize / BaseSize;

            if (input.wk)
            {
                CameraManager.BoardCamera.transform.position += mscl * TranslationAmount * Vector3.up;
            }
            if (input.sk)
            {
                CameraManager.BoardCamera.transform.position += mscl * TranslationAmount * Vector3.down;
            }
            if (input.dk)
            {
                CameraManager.BoardCamera.transform.position += mscl * TranslationAmount * Vector3.right;
            }
            if (input.ak)
            {
                CameraManager.BoardCamera.transform.position += mscl * TranslationAmount * Vector3.left;
            }
            if (Input.mouseScrollDelta.y < 0f)
            {
                CameraManager.BoardCamera.orthographicSize *= scl * ScrollMultiplier;
            }
            if (Input.mouseScrollDelta.y > 0f)
            {
                CameraManager.BoardCamera.orthographicSize *= scl / ScrollMultiplier;
            }

            CameraManager.BoardCamera.orthographicSize = Mathf.Clamp(CameraManager.BoardCamera.orthographicSize, MinSize, MaxSize);
        }

        public void End(BoardManager board)
        {}
    }
}