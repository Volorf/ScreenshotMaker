using System.Collections;
using System.IO;
using UnityEngine;

namespace Volorf.ScreenshotMaker
{
    public class ScreenshotMaker: MonoBehaviour
    {
        [SerializeField] private Camera coverShotCamera;
        [SerializeField] private string defaultCoverName = "cover";
        [SerializeField] private int coverWidth = 300;
        [SerializeField] private int coverHeight = 200;

        public void MakeCover(string filePath)
        {
            StartCoroutine(CameraCapture(filePath));
        }

        IEnumerator CameraCapture(string filePath)
        {
            yield return new WaitForEndOfFrame();
            CamCapture(filePath);
        }
    
        private void CamCapture(string filePath)
        {
            RenderTexture tempRenderTexture = new RenderTexture(coverWidth, coverHeight, 24, RenderTextureFormat.ARGB32)
            {
                antiAliasing = 2
            };
        
            coverShotCamera.targetTexture = tempRenderTexture;
            coverShotCamera.Render();
        
            RenderTexture.active = tempRenderTexture;
        
            Texture2D tex = new Texture2D(coverWidth, coverHeight);
            tex.ReadPixels(new Rect(0, 0, coverWidth, coverHeight), 0, 0);
            tex.Apply();
            RenderTexture.active = null;
 
            var Bytes = tex.EncodeToPNG();
            Destroy(tex);
            // File.WriteAllBytes(Application.dataPath + "/Backgrounds/" + defaultCoverName + ".png", Bytes);
            File.WriteAllBytes(filePath + "/" + defaultCoverName + ".png", Bytes);
        }
    }
}
