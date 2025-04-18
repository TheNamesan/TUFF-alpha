using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace TUFF.TUFFEditor
{
    [CustomPropertyDrawer(typeof(MoveCameraAction))]
    public class MoveCameraActionPD : EventActionPD
    {
        public override void InspectorGUIContent()
        {
            EditorGUILayout.BeginHorizontal();
            var targetCamera = targetProperty.FindPropertyRelative("targetCamera");
            EditorGUILayout.PropertyField(targetCamera);
            if (GUILayout.Button(new GUIContent("Assign Main Camera"), GUILayout.MaxWidth(200)))
            {
                Camera camera = Camera.main;
                if (camera != null)
                {
                    if (camera.TryGetComponent<CameraFollow>(out var follow))
                    {
                        targetCamera.objectReferenceValue = follow;
                    }
                    else Debug.LogWarning($"Main Camera ({camera}) does not contain Camera Follow component!");
                } 
                else Debug.LogWarning("No Main Camera found!");
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.PropertyField(targetProperty.FindPropertyRelative("cameraMove"));
        }
        public override void SummaryGUI(Rect position)
        {
            EditorGUI.LabelField(position, GetSummaryText());
        }

        private string GetSummaryText()
        {
            var action = targetObject as MoveCameraAction;
            if (action.targetCamera == null) return "No target set";
            CameraMove cameraMove = action.cameraMove;
            if (cameraMove == null) return "No Camera Move set";
            string targetText = "";
            string cameraName = action.targetCamera.gameObject.name;
            if (cameraMove.moveCameraType == MoveCameraType.MoveDelta)
            {
                targetText = $"Move {cameraName} Delta {action.cameraMove.moveDelta}";
            }
            else if (cameraMove.moveCameraType == MoveCameraType.MoveToWorldPosition)
            {
                targetText = $"Move {cameraName} to World Position {action.cameraMove.targetWorldPosition}";
            }
            else if (cameraMove.moveCameraType == MoveCameraType.MoveToTransformPosition)
            {
                string transformName = "null";
                if (action.cameraMove.targetTransform) transformName = action.cameraMove.targetTransform.gameObject.name;
                targetText = $"Move {cameraName} to Transform position ({transformName})";
            }
            else if (cameraMove.moveCameraType == MoveCameraType.ReturnToPlayer)
            {
                targetText = $"Return {cameraName} to Player";
            }

            string ignoreAxisText = "";
            bool ignoreX = cameraMove.ignoreX;
            bool ignoreY = cameraMove.ignoreY;
            bool hasIgnores = ignoreX || ignoreY;
            bool hasBothIgnores = ignoreX && ignoreY;
            if (hasIgnores) ignoreAxisText = $"(Ignore {(ignoreX ? "X" : "")}{(hasBothIgnores ? ", " : "")}{(ignoreY ? "Y" : "")})";
            return $"{targetText}. " +
                $"Ease {action.cameraMove.easeType}, in {action.cameraMove.timeDuration} second{(action.cameraMove.timeDuration == 1 ? "" : "s")} {ignoreAxisText}";
        }
    }
}