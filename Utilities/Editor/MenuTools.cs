﻿/***
 * Author RadBear - Nguyen Ba Hung - nbhung71711@gmail.com 
 **/

using RCore.Common;
using RCore.Common.Editor;
using UnityEditor;
using UnityEngine;

namespace RCore.Editor
{
    public class MenuTools : UnityEditor.Editor
    {
        private const string ALT = "&";
        private const string SHIFT = "#";
        private const string CTRL = "%";

        [MenuItem("RCore/Save Assets " + SHIFT + "_1", priority = 1)]
        private static void SaveAssets()
        {
            var objs = Selection.objects;
            if (objs != null)
                foreach (var obj in objs)
                    EditorUtility.SetDirty(obj);

            AssetDatabase.SaveAssets();
        }
        
        [MenuItem("RCore/Refresh Prefabs in folder", priority = 1)]
        private static void RefreshPrefabs()
        {
            RefreshAssets("t:GameObject");
        }
        
        [MenuItem("RCore/Refresh ScriptableObjects in folder", priority = 1)]
        private static void RefreshScriptableObjects()
        {
            RefreshAssets("t:ScriptableObject");
        }
        
        [MenuItem("RCore/Refresh Assets in folder", priority = 1)]
        private static void RefreshAll()
        {
            RefreshAssets("t:GameObject t:ScriptableObject");
        }

        private static void RefreshAssets(string filter)
        {
            string folderPath = EditorHelper.OpenFolderPanel();
            folderPath = EditorHelper.FormatPathToUnityPath(folderPath);
            var assetGUIDs = AssetDatabase.FindAssets(filter, new[] { folderPath });
            foreach (string guid in assetGUIDs)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset != null)
                    EditorUtility.SetDirty(asset);
            }
            AssetDatabase.SaveAssets();
        }

        //==========================================================

        [MenuItem("RCore/Group Scene Objects " + ALT + "_F1", priority = 31)]
        private static void GroupSceneObjects()
        {
            var objs = Selection.gameObjects;
            if (objs.Length > 1)
            {
                var group = new GameObject();
                for (int i = 0; i < objs.Length; i++)
                {
                    objs[i].transform.SetParent(group.transform);
                }
                Selection.activeObject = group;
            }
        }

        [MenuItem("RCore/Ungroup Scene Objects " + ALT + "_F2", priority = 32)]
        private static void UngroupSceneObjects()
        {
            var objs = Selection.gameObjects;
            if (objs.Length > 1)
            {
                for (int i = 0; i < objs.Length; i++)
                    objs[i].transform.SetParent(null);
            }
        }

        //==========================================================

        [MenuItem("RCore/Run _F5", priority = 61)]
        private static void Run()
        {
            EditorApplication.isPlaying = true;
        }

        [MenuItem("RCore/Stop #_F5", priority = 62)]
        private static void Stop()
        {
            EditorApplication.isPlaying = false;
        }

        //==========================================================

        [MenuItem("CONTEXT/Collider/Create a child object with this collider")]
        public static void Menu_AttachBeam(MenuCommand menuCommand)
        {
            var collider = menuCommand.context as Collider;
            if (collider)
            {
                var obj = Instantiate(collider);
                obj.transform.SetParent(collider.transform);
            }
        }
    }
}