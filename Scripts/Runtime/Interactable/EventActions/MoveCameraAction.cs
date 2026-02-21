using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TUFF
{
    [System.Serializable]
    public class MoveCameraAction : EventAction
    {
        [Tooltip("Reference to the camera to move.")]
        public CameraFollow targetCamera;
        public CameraMove cameraMove = new CameraMove();
        public bool waitForCompletion = true;

        public MoveCameraAction()
        {
            eventName = "Move Camera";
            eventColor = EventGUIColors.movement;
        }
        public override void Invoke()
        {
            if (cameraMove == null) EndEvent();
            if (targetCamera == null) EndEvent();

            if (waitForCompletion)
            {
                AddWaitForCompletionEvent();
            }
            
            CameraMove.InvokeMovement(cameraMove, targetCamera);
            if (!waitForCompletion) EndEvent();
        }
        protected void AddWaitForCompletionEvent()
        {
            UnityAction action = null;
            action = () => 
            {
                EndEvent();
                cameraMove.onMovementEnd.RemoveListener(action);
            };
            cameraMove.onMovementEnd.AddListener(action);
        }
    }
}