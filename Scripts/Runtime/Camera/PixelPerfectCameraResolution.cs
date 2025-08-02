using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;

namespace TUFF
{
    public class PixelPerfectCameraResolution : MonoBehaviour
    {
        public float targetSize = 6f;
        public UnityEngine.Rendering.Universal.PixelPerfectCamera pixelPerfectCamera;
        public Camera cam;

        private void Awake()
        {
            if (!pixelPerfectCamera) pixelPerfectCamera = GetComponent<UnityEngine.Rendering.Universal.PixelPerfectCamera>();
            if (!cam) cam = GetComponent<Camera>();
        }
        private void OnEnable()
        {
            if (cam)
            {
                cam.orthographicSize = targetSize;
            }
            AdjustToResolution();
        }
        private void Start()
        {
            if (cam)
            {
                cam.orthographicSize = targetSize;
            }
            AdjustToResolution();
        }
        private void Update()
        {
            AdjustToResolution();
        }

        private void AdjustToResolution()
        {
            if (!pixelPerfectCamera) return;
            if (!pixelPerfectCamera.enabled) return;
            int height = Screen.height;
            pixelPerfectCamera.refResolutionY = height;
            pixelPerfectCamera.refResolutionX = Mathf.RoundToInt(height / 9 * 16f);

            float size = Mathf.Max(targetSize, 0.00001f);
            pixelPerfectCamera.assetsPPU = LISAUtility.Truncate((pixelPerfectCamera.refResolutionY * 0.5f) / size);
            //(float)nativePixelResHeight / (float)PPU / 2f; //544x416
        }
    }
}

