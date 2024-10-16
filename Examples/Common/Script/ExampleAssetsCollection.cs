using System.Collections.Generic;
using UnityEngine;
using RCore.Common;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using RCore.Editor;
using UnityEditor;
#endif

namespace RCore.Example
{
    [CreateAssetMenu(fileName = "ExampleAssetsCollection", menuName = "RCore/Assets Collection Example")]
    public class ExampleAssetsCollection : ScriptableObject
    {
        public AssetsList<Sprite> icons;
        [FormerlySerializedAs("gameobjects")] public List<GameObject> gameObjects;
        public AssetsList<Sprite> prefabs;

#if UNITY_EDITOR
        [CustomEditor(typeof(ExampleAssetsCollection))]
        public class ExampleAssetsCollectionEditor : UnityEditor.Editor
        {
            private ExampleAssetsCollection m_script;

            private void OnEnable()
            {
                m_script = target as ExampleAssetsCollection;
            }

            public override void OnInspectorGUI()
            {
                var currentTab = EditorHelper.Tabs(m_script.name, "Default", "Custom");
                switch (currentTab)
                {
                    case "Default":
                        base.OnInspectorGUI();
                        break;
                    case "Custom":
                        EditorHelper.DrawAssetsList(m_script.icons, "Icons");
                        EditorHelper.ListObjects("Prefabs 1", ref m_script.gameObjects, null);
                        EditorHelper.DrawAssetsList(m_script.prefabs, "Prefabs 2");
                        break;
                }
            }
        }
#endif
    }
}