using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Volorf.ScreenshotMaker
{
    public class ScreenshotMaker : MonoBehaviour
    {
        [Header("Camera")]
        [SerializeField] Camera screenshotCamera;
        [SerializeField] int cameraDepth = -1000;

        [Space(10)] [Header("Screenshot")]
        [SerializeField] string defaultScreenshotName = "screenshot";
        [SerializeField] bool useNameCounter;
        [SerializeField] int antiAliasing = 2;
        [SerializeField] int screenshotWidth = 300;
        [SerializeField] int screenshotHeight = 200;

        [Space(10)] [Header("Events")]
        [SerializeField] UnityEvent<Texture2D> _onScreenshotTaken;
        [SerializeField] UnityEvent<Sprite> _onSpriteCreated;

        [Space(10)] [Header("Debugging")]
        [SerializeField] bool _printPath;

        const string COUNTER_KEY = "Volorf.ScreenshotMaker.Counter";
        int _currentCounter;

        private void Start()
        {
            if (screenshotCamera == null)
            {
                Debug.LogError("[ScreenshotMaker] No camera assigned.", this);
                return;
            }

            screenshotCamera.depth = cameraDepth;

            if (useNameCounter)
                _currentCounter = PlayerPrefs.GetInt(COUNTER_KEY);
        }

        [ContextMenu("Make Screenshot")]
        public void MakeScreenshot()
        {
            if (screenshotCamera == null)
            {
                Debug.LogError("[ScreenshotMaker] No camera assigned.", this);
                return;
            }

            string path = Path.Combine(Application.persistentDataPath, "Screenshots");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            _ = CaptureAndSaveAsync(path);

            if (_printPath)
                Debug.Log("[ScreenshotMaker] Screenshot saved to: " + path);
        }

        public async Task CaptureAndSaveAsync(string folderPath)
        {
            try
            {
                await SaveScreenshotAsync(folderPath);
            }
            catch (Exception e)
            {
                Debug.LogError("[ScreenshotMaker] Failed to save screenshot: " + e);
            }
        }

        public (byte[] pngData, Texture2D texture, Sprite sprite) CaptureScreenshot()
        {
            RenderTexture rt = new RenderTexture(screenshotWidth, screenshotHeight, 24, RenderTextureFormat.ARGB32)
            {
                antiAliasing = antiAliasing
            };

            RenderTexture previousTarget = screenshotCamera.targetTexture;

            screenshotCamera.targetTexture = rt;
            screenshotCamera.Render();

            RenderTexture.active = rt;
            Texture2D texture = new Texture2D(screenshotWidth, screenshotHeight);
            texture.ReadPixels(new Rect(0, 0, screenshotWidth, screenshotHeight), 0, 0);
            texture.Apply();
            RenderTexture.active = null;

            screenshotCamera.targetTexture = previousTarget;
            rt.Release();
            Destroy(rt);

            byte[] pngData = texture.EncodeToPNG();
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, screenshotWidth, screenshotHeight), Vector2.zero);

            _onScreenshotTaken?.Invoke(texture);
            _onSpriteCreated?.Invoke(sprite);

            return (pngData, texture, sprite);
        }

        private async Task SaveScreenshotAsync(string folderPath)
        {
            string finalName;

            if (useNameCounter)
            {
                _currentCounter++;
                PlayerPrefs.SetInt(COUNTER_KEY, _currentCounter);
                finalName = defaultScreenshotName + _currentCounter;
            }
            else
            {
                finalName = defaultScreenshotName;
            }

            var (pngData, _, _) = CaptureScreenshot();
            await File.WriteAllBytesAsync(Path.Combine(folderPath, finalName + ".png"), pngData);
        }
    }
}
