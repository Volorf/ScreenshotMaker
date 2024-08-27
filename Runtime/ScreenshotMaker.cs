using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Volorf.ScreenshotMaker
{
    public class ScreenshotMaker: MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] Camera coverShotCamera;
        [SerializeField] int cameraDepth = -1000;
        
        [Space(10)] [Header("Screenshot")]
        [SerializeField] string defaultCoverName = "cover";
        [SerializeField] bool useNameCounter;
        [SerializeField] int coverAntiAliasing = 2;
        [SerializeField] int coverWidth = 300;
        [SerializeField] int coverHeight = 200;
        
        [Space(10)] [Header("Events")]
        [SerializeField] UnityEvent<Texture2D> _onScreenshotTaken;
        [SerializeField] UnityEvent<Sprite> _onSpriteCreated;

        [Space(10)] [Header("Debugging")] 
        [SerializeField] bool _printPath;

        const string COUNTER_NAME = "Counter Name";
        int _currentCounter;

        private void Start()
        {
            coverShotCamera.depth = cameraDepth;
            if (useNameCounter)
                _currentCounter = PlayerPrefs.GetInt(COUNTER_NAME);
        }

        public string GetImagePreviewName()
        {
            return defaultCoverName;
        }
    
        [ContextMenu("Make Screenshot")]
        public void MakeScreenshot()
        {
            string path = Path.Combine(Application.persistentDataPath, "Screenshots");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            MakeCover(path);
            
            if (_printPath)
                Debug.Log("Screenshot has been save there: " + path);
            
        }

        public async void MakeCover(string filePath)
        {
            try
            {
                await CamCaptureAsync(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
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
    
        private async Task CamCaptureAsync(string filePath)
        {
            string finalName;
            
            if (useNameCounter)
            {
                _currentCounter++;
                PlayerPrefs.SetInt(COUNTER_NAME, _currentCounter);
                 finalName = defaultCoverName + _currentCounter;
            }
            else
            {
                finalName = defaultCoverName;
            }
            
            await File.WriteAllBytesAsync(filePath + "/" + finalName + ".png", GetImageDataFromCamera());
            // Debug.Log(filePath);
        }
    }
}
