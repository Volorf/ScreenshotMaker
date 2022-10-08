using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Volorf.ScreenshotMaker
{
    public class ScreenshotMaker: MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] private Camera coverShotCamera;
        [SerializeField] private int cameraDepth = -1000;
        
        [Space(16)]
        [Header("Screenshot")]
        [SerializeField] private string defaultCoverName = "cover";
        [SerializeField] private int coverAntiAliasing = 2;
        [SerializeField] private int coverWidth = 300;
        [SerializeField] private int coverHeight = 200;

        private void Start()
        {
            coverShotCamera.depth = cameraDepth;
        }

        public string GetImagePreviewName()
        {
            return defaultCoverName;
        }

        public void MakeCover(string filePath)
        {
            StartCoroutine(CameraCapture(filePath));
        }

        public byte[] GetImageDataFromCamera()
        {
            RenderTexture tempRenderTexture = new RenderTexture(coverWidth, coverHeight, 24, RenderTextureFormat.ARGB32)
            {
                antiAliasing = coverAntiAliasing
            };
        
            coverShotCamera.targetTexture = tempRenderTexture;
            coverShotCamera.Render();
            RenderTexture.active = tempRenderTexture;
            Texture2D tempTexture = new Texture2D(coverWidth, coverHeight);
            tempTexture.ReadPixels(new Rect(0, 0, coverWidth, coverHeight), 0, 0);
            tempTexture.Apply();
            RenderTexture.active = null;
            byte[] imageDataFromCamera = tempTexture.EncodeToPNG();
            Destroy(tempTexture);

            return imageDataFromCamera;
        }

        IEnumerator CameraCapture(string filePath)
        {
            yield return new WaitForEndOfFrame();
            CamCapture(filePath);
        }
    
        private void CamCapture(string filePath)
        {
            File.WriteAllBytes(filePath + "/" + defaultCoverName + ".png", GetImageDataFromCamera());
        }
    }
}
