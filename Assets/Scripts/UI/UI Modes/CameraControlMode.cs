using Ramsey.Board;
using Ramsey.Screen;
using Ramsey.Utilities;
using UnityEngine;

namespace Ramsey.UI
{
    public class CameraControlMode : IUserMode<BoardManager>
    {
        public const float TranslationAmount = 0.1f;
        public const float BaseSize = 5f;
        public const float ScrollMultiplier = 1.025f;
        public const float MinSize = 0.5f;
        public const float MaxSize = 20f;

        bool IUserMode.AltersBoard => false;

        public void Init(BoardManager board) {}

        public void Update(InputData input, BoardManager board)
        {
            var scl = Input.GetKey(KeyCode.LeftShift).ToInt();
            var tscl = Time.deltaTime * 60f;

            var mscl = tscl * (scl * 2 + 1) * CameraManager.BoardCamera.orthographicSize / BaseSize;
            var sscl = tscl * scl * 0.05f + 1;

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
                CameraManager.BoardCamera.orthographicSize *= sscl * ScrollMultiplier;
            }
            if (Input.mouseScrollDelta.y > 0f)
            {
                CameraManager.BoardCamera.orthographicSize /= sscl * ScrollMultiplier;
            }

            CameraManager.BoardCamera.orthographicSize = Mathf.Clamp(CameraManager.BoardCamera.orthographicSize, MinSize, MaxSize);
        }

        public void End(BoardManager board)
        {}

        public bool IsGameplayMode => false;
    }
}