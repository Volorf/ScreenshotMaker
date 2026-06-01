using UnityEditor;
using UnityEngine;

namespace Volorf.ScreenshotMaker.Editor
{
    [CustomEditor(typeof(ScreenshotMaker))]
    public class ScreenshotMakerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(10);

            ScreenshotMaker screenshotMaker = (ScreenshotMaker)target;

            GUI.enabled = EditorApplication.isPlaying;

            if (GUILayout.Button("Make Screenshot", GUILayout.Height(24)))
            {
                screenshotMaker.MakeScreenshot();
            }
        }
    }
}
