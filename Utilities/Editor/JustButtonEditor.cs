using RCore.Common.Editor;
using UnityEditor;
using UnityEditor.UI;

namespace RCore.Components.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(JustButton), true)]
    public class JustButtonEditor : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.BeginVertical("box");
            {
                EditorHelper.SerializeField(serializedObject, "mImg");
                EditorHelper.SerializeField(serializedObject, "mPivotForFX");
                EditorHelper.SerializeField(serializedObject, "mEnabledFX");
                EditorHelper.SerializeField(serializedObject, "mGreyMatEnabled");
                EditorHelper.SerializeField(serializedObject, "m_SfxClip");
                EditorHelper.SerializeField(serializedObject, "m_PerfectRatio");
                var imgSwapEnabled = EditorHelper.SerializeField(serializedObject, "mImgSwapEnabled");
                if (imgSwapEnabled.boolValue)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.BeginVertical("box");
                    EditorHelper.SerializeField(serializedObject, "mImgActive");
                    EditorHelper.SerializeField(serializedObject, "mImgInactive");
                    EditorGUILayout.EndVertical();
                    EditorGUI.indentLevel--;
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("RCore/UI/Replace Button By JustButton")]
        private static void ReplaceButton()
        {
            var gameObjects = Selection.gameObjects;
            for (int i = 0; i < gameObjects.Length; i++)
            {
                var buttons = gameObjects[i].GetComponentsInChildren<UnityEngine.UI.Button>(true);
                for (int j = 0; j < buttons.Length; j++)
                {
                    var btn = buttons[j];
                    if (btn is not JustButton)
                    {
                        var obj = btn.gameObject;
                        DestroyImmediate(btn);
                        obj.AddComponent<JustButton>();
                    }
                }
            }
        }
    }
}