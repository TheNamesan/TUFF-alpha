using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TUFF
{
    [ExecuteInEditMode]
    public class AdjustToPreferredTextSize : MonoBehaviour
    {
        public TMP_Text tmpText;
        public RectTransform rt;

        public float minWidth = 0;
        public float minHeight = 0;

        public float widthPadding = 0;
        public float heightPadding = 0;

        public bool disableWidthAdjust = false;
        public bool disableHeightAdjust = false;

        private void OnEnable()
        {
            Adjust();
        }
        private void Update()
        {
            Adjust();
        }

        public void Adjust()
        {
            if (tmpText == null || rt == null) return;
            if (!tmpText.enabled) return;
            if (tmpText.text.Length <= 0) return;
            float maxWidth = float.PositiveInfinity;
            float maxHeight = float.PositiveInfinity;
            if (disableWidthAdjust) maxWidth = rt.sizeDelta.x;
            if (disableHeightAdjust) maxHeight = rt.sizeDelta.y;
            Vector2 prefferedSize = tmpText.GetPreferredValues(tmpText.text, maxWidth, maxHeight);
            float sizeX = Mathf.Max(minWidth, prefferedSize.x) + widthPadding;
            float sizeY = Mathf.Max(minHeight, prefferedSize.y) + heightPadding;
            if (disableWidthAdjust) sizeX = rt.sizeDelta.x;
            if (disableHeightAdjust) sizeY = rt.sizeDelta.y;
            rt.sizeDelta = new Vector2(sizeX, sizeY);
        }
    }
}

