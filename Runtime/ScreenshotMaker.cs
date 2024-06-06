using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace Volorf.ScreenshotMaker
{
    public class ScreenshotMaker: MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] Camera coverShotCamera;
        [SerializeField] int cameraDepth = -1000;
        
        [Space(10)]
        [Header("Screenshot")]
        [SerializeField] string defaultCoverName = "cover";
        [SerializeField] int coverAntiAliasing = 2;
        [SerializeField] int coverWidth = 300;
        [SerializeField] int coverHeight = 200;
        
        [Space(10)]
        [Header("Events")]
        [SerializeField] UnityEvent<Texture2D> _onScreenshotTaken;
        [SerializeField] UnityEvent<Sprite> _onSpriteCreated;

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
            Sprite sprite = Sprite.Create(tempTexture, new Rect(0f, 0f, coverWidth, coverHeight), Vector2.zero);
            RenderTexture.active = null;
            byte[] imageDataFromCamera = tempTexture.EncodeToPNG();
        
            _onScreenshotTaken?.Invoke(tempTexture);
            _onSpriteCreated.Invoke(sprite);
            
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
