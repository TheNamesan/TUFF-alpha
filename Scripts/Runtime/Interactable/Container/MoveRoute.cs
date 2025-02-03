using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TUFF
{
    public enum MoveRouteInstruction
    {
        WaitForSeconds = 0,
        MoveHorizontal = 1,
        ChangeFacing = 2,
        TryVerticalJump = 3,
        ForceJump = 4,
        ChangeSpeed = 5
    }
    [System.Serializable]
    public class MoveRoute
    {
        public List<MoveRouteElement> elements = new List<MoveRouteElement>();
    }
    [System.Serializable]
    public class MoveRouteElement
    {
        public MoveRouteInstruction instruction = MoveRouteInstruction.WaitForSeconds;
        public float duration = 0;
        public CharacterHorizontalDirection moveDirectionH = CharacterHorizontalDirection.Right;
        public CharacterVerticalJumpDirection tryJumpDirection = CharacterVerticalJumpDirection.Up;
        public FaceDirections facing = FaceDirections.East;
        public Vector2 jumpForceDirection = new Vector2();
        public HardFallBehaviour hardFallBehaviour = HardFallBehaviour.Default;
        public float newSpeed = 0;

        public IEnumerator PlayElement(OverworldCharacterController controller)
        {
            if (controller == null) yield break;
            switch (instruction)
            {
                case MoveRouteInstruction.WaitForSeconds:
                    yield return new WaitForSeconds(duration);
                    break;
                case MoveRouteInstruction.MoveHorizontal:
                    controller.nextInput.horizontalInput = (moveDirectionH == CharacterHorizontalDirection.Right ? 1 : -1);
                    yield return new WaitForSeconds(duration);
                    controller.nextInput.horizontalInput = 0;
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
                    break;
                case MoveRouteInstruction.ChangeFacing:
                    controller.ChangeFaceDirection(facing);
                    break;
                case MoveRouteInstruction.TryVerticalJump:
                    var tryJumpDir = (tryJumpDirection == CharacterVerticalJumpDirection.Up ? 
                        OverworldCharacterController.QueuedAction.JumpUp : OverworldCharacterController.QueuedAction.JumpDown);
                    controller.QueueAction(tryJumpDir);
                    yield return new WaitForFixedUpdate();
                    yield return new WaitForFixedUpdate();
                    break;
                case MoveRouteInstruction.ForceJump:
                    controller.ForceFall(jumpForceDirection, hardFallBehaviour);
                    break;
                case MoveRouteInstruction.ChangeSpeed:
                    controller.ChangeMoveSpeed(newSpeed);
                    break;
            }
            yield break;
        }
    }
}