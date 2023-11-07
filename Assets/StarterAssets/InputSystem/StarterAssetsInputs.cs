using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class StarterAssetsInputs : MonoBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public float scroll;
        public bool jump;
        public bool sprint;
        public bool confirm;
        public bool rotate;
        public bool demolish;
        public bool rightClick;
        public bool save;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }
        public void OnScroll(InputValue value)
        {
            ScrollInput(value.Get<float>());
        }

        public void OnLook(InputValue value)
        {
            if (cursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
        public void OnConfirm(InputValue value)
        {
            ConfirmInput(value.isPressed);
        }
        public void OnRotate(InputValue value)
        {
            RotateInput(value.isPressed);
        }
        public void OnDemolish(InputValue value)
        {
            DemolishInput(value.isPressed);
        }
        public void OnRightClick(InputValue value)
        {
            RightClickInput(value.isPressed);
        }
        public void OnSave(InputValue value)
        {
            SaveInput(value.isPressed);
        }
#endif


        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }
        public void ScrollInput(float newScrollValue)
        {
            scroll = newScrollValue;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            sprint = newSprintState;
        }
        public void ConfirmInput(bool newConfirmState)
        {
            confirm = newConfirmState;
        }
        public void RotateInput(bool newRotateState)
        {
            rotate = newRotateState;
        }
        public void DemolishInput(bool newDemolishState)
        {
            demolish = newDemolishState;
        }
        public void RightClickInput(bool newRightClickState)
        {
            rightClick = newRightClickState;
        }
        public void SaveInput(bool newSaveState)
        {
            save = newSaveState;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            //SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

}