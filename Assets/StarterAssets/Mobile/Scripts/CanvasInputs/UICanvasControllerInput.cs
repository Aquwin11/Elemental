using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool virtualSprintState)
        {
            starterAssetsInputs.SprintInput(virtualSprintState);
        }

        public void VirtualDashInput(bool virtualDashState)
        {
            starterAssetsInputs.DashInput(virtualDashState);
        }
        public void VirtualInteractInput(bool virtualInteractState)
        {
            starterAssetsInputs.InteractInput(virtualInteractState);
        }
        public void VirtualAimInput(bool virtualAimState)
        {
            starterAssetsInputs.AimInput(virtualAimState);
        }
        public void VirtualShootInput(bool virtualShootState)
        {
            starterAssetsInputs.ShootInput(virtualShootState);
        }
        public void VirtualEscapeInput(bool virtualEscapeState)
        {
            starterAssetsInputs.EscapeInput(virtualEscapeState);
        }

    }

}
