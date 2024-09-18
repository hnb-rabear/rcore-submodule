using UnityEngine;
using RCore.Common;
using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
using RCore.Common.Editor;
using UnityEditor;
#endif

namespace RCore.Components
{
    /// <summary>
    /// Simple audio collection
    /// Simple enough for small game which does not require many sounds
    /// </summary>
    [CreateAssetMenu(fileName = "AudioCollection", menuName = "RCore/Audio Collection")]
    public class AudioCollection : ScriptableObject
    {
        [SerializeField] private bool m_ImportFromFolder = true;
        [SerializeField] private string m_Namespace;
        [SerializeField] private string m_NameClassMusic = "MusicIDs";
        [SerializeField] private string m_NameClassSFX = "SfxIDs";
        [SerializeField] private string m_MusicsPath;
        [SerializeField] private string m_SfxsPath;
        [SerializeField] private string m_ConfigPath;

        [SerializeField] private AudioClip[] sfxClips;
        [SerializeField] private AudioClip[] musicClips;
        [SerializeField] private AssetReferenceT<AudioClip>[] m_abSfxClips;
        [SerializeField] private AssetReferenceT<AudioClip>[] m_abMusicClips;

        public AudioClip GetMusicClip(int pKey)
        {
            if (pKey < musicClips.Length)
                return musicClips[pKey];
            return null;
        }

        public AudioClip GetMusicClip(string pKey)
        {
            for (int i = 0; i < musicClips.Length; i++)
            {
                if (musicClips[i].name == pKey)
                    return musicClips[i];
            }
            return null;
        }

        public AudioClip GetMusicClip(string pKey, ref int pIndex)
        {
            for (int i = 0; i < musicClips.Length; i++)
            {
                if (musicClips[i].name == pKey)
                {
                    pIndex = i;
                    return musicClips[i];
                }
            }
            return null;
        }

        public AudioClip GetSFXClip(int pIndex)
        {
            if (pIndex < sfxClips.Length)
                return sfxClips[pIndex];
            return null;
        }

        public AudioClip GetSFXClip(string pName)
        {
            for (int i = 0; i < sfxClips.Length; i++)
            {
                if (sfxClips[i].name == pName)
                    return sfxClips[i];
            }
            return null;
        }

        public AudioClip GetSFXClip(string pKey, out int pIndex)
        {
            pIndex = -1;
            for (int i = 0; i < sfxClips.Length; i++)
            {
                if (sfxClips[i].name.ToLower() == pKey.ToLower())
                {
                    pIndex = i;
                    return sfxClips[i];
                }
            }
            return null;
        }

        public string[] GetSFXNames()
        {
            var names = new string[sfxClips.Length];
            for (var i = 0; i < sfxClips.Length; i++)
                names[i] = sfxClips[i].name;
            return names;
        }

#if UNITY_EDITOR
        [CustomEditor(typeof(AudioCollection))]
        private class AudioCollectionEditor : UnityEditor.Editor
        {
            private AudioCollection m_Script;
            private bool m_isAssetBundle;

            private void OnEnable()
            {
                m_Script = target as AudioCollection;
                UnityEngine.Debug.Log(m_Script.name);
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                EditorHelper.BoxVertical(() =>
                {
                    string musicSourcePath = EditorHelper.FolderSelector("Musics Sources Path", $"{m_Script.name}musicsSourcePath", m_Script.m_MusicsPath, true);
                    string sfxSourcePath = EditorHelper.FolderSelector("SFX Sources Path", $"{m_Script.name}sfxsSourcePath", m_Script.m_SfxsPath, true);
                    string exportConfigPath = EditorHelper.FolderSelector("Export Config Path", $"{m_Script.name}exportConfigPath", m_Script.m_ConfigPath, true);

                    if (EditorHelper.Button("Build"))
                    {
                        musicSourcePath = Application.dataPath + musicSourcePath;
                        sfxSourcePath = Application.dataPath + sfxSourcePath;
                        exportConfigPath = Application.dataPath + exportConfigPath;

                        var musicFiles = m_Script.musicClips;
                        var sfxFiles = m_Script.sfxClips;
                        if (m_Script.m_ImportFromFolder)
                        {
                            musicFiles = EditorHelper.GetObjects<AudioClip>(musicSourcePath, "t:AudioClip").ToArray();
                            sfxFiles = EditorHelper.GetObjects<AudioClip>(sfxSourcePath, "t:AudioClip").ToArray();
                        }
                        string result = GetConfigTemplate();

                        //Musics
                        m_Script.musicClips = new AudioClip[musicFiles.Length];
                        string stringKeys = "";
                        string intKeys = "";
                        string enumKeys = "";
                        for (int i = 0; i < musicFiles.Length; i++)
                        {
                            m_Script.musicClips[i] = musicFiles[i];

                            stringKeys += $"\"{musicFiles[i].name}\"";
                            intKeys += $"{musicFiles[i].name} = {i}";
                            enumKeys += $"{musicFiles[i].name} = {i}";
                            if (i < musicFiles.Length - 1)
                            {
                                stringKeys += $",{Environment.NewLine}\t\t";
                                intKeys += $",{Environment.NewLine}\t\t";
                                enumKeys += $",{Environment.NewLine}\t\t";
                            }
                        }
                        result = result.Replace("<M_CONSTANT_INT_KEYS>", intKeys).Replace("<M_CONSTANT_STRING_KEYS>", stringKeys).Replace("<M_CONSTANTS_ENUM_KEYS>", enumKeys);

                        //SFXs
                        m_Script.sfxClips = new AudioClip[sfxFiles.Length];
                        stringKeys = "";
                        intKeys = "";
                        enumKeys = "";
                        for (int i = 0; i < sfxFiles.Length; i++)
                        {
                            m_Script.sfxClips[i] = sfxFiles[i];

                            stringKeys += $"\"{sfxFiles[i].name}\"";
                            intKeys += $"{sfxFiles[i].name} = {i}";
                            enumKeys += $"{sfxFiles[i].name} = {i}";
                            if (i < sfxFiles.Length - 1)
                            {
                                stringKeys += $",{Environment.NewLine}\t\t";
                                intKeys += $",{Environment.NewLine}\t\t";
                                enumKeys += $",{Environment.NewLine}\t\t";
                            }
                        }
                        result = result.Replace("<S_CONSTANT_INT_KEYS>", intKeys).Replace("<S_CONSTANT_STRING_KEYS>", stringKeys).Replace("<S_CONSTANTS_ENUM_KEYS>", enumKeys);

                        //Write result
                        System.IO.File.WriteAllText(exportConfigPath + "/AudioIDs.cs", result);

                        if (m_isAssetBundle)
                        {
                            var addressableSettings = AddressableAssetSettingsDefaultObject.Settings;
                            m_Script.m_abSfxClips = new AssetReferenceT<AudioClip>[m_Script.sfxClips.Length];
                            for (int i = 0; i < m_Script.sfxClips.Length; i++)
                            {
                                var clip = m_Script.sfxClips[i];
                                string path = AssetDatabase.GetAssetPath(clip);
                                string guid = AssetDatabase.AssetPathToGUID(path);
                                var entry = addressableSettings.FindAssetEntry(guid);
                                if (entry != null)
                                {
                                    m_Script.m_abSfxClips[i] = new AssetReferenceT<AudioClip>(guid);
                                    m_Script.sfxClips[i] = null;
                                }
                            }

                            m_Script.m_abMusicClips = new AssetReferenceT<AudioClip>[m_Script.musicClips.Length];
                            for (int i = 0; i < m_Script.musicClips.Length; i++)
                            {
                                var clip = m_Script.musicClips[i];
                                string path = AssetDatabase.GetAssetPath(clip);
                                string guid = AssetDatabase.AssetPathToGUID(path);
                                var entry = addressableSettings.FindAssetEntry(guid);
                                if (entry != null)
                                {
                                    m_Script.m_abMusicClips[i] = new AssetReferenceT<AudioClip>(guid);
                                    m_Script.musicClips[i] = null;
                                }
                            }
                        }

                        if (GUI.changed)
                        {
                            m_Script.m_MusicsPath = musicSourcePath.Replace(Application.dataPath, "");
                            m_Script.m_SfxsPath = sfxSourcePath.Replace(Application.dataPath, "");
                            m_Script.m_ConfigPath = exportConfigPath.Replace(Application.dataPath, "");
                            EditorUtility.SetDirty(m_Script);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                    }
                }, ColorHelper.LightAzure, true);
            }

            private string GetConfigTemplate()
            {
                if (string.IsNullOrEmpty(m_Script.m_NameClassSFX))
                    m_Script.m_NameClassSFX = "SfxIDs";
                if (string.IsNullOrEmpty(m_Script.m_NameClassMusic))
                    m_Script.m_NameClassMusic = "MusicIDs";

                string musicTemplate =
                    $"public static class {m_Script.m_NameClassMusic}\n"
                    + "{\n"
                    + "\tpublic const int <M_CONSTANT_INT_KEYS>;\n"
                    + "\tpublic static readonly string[] idStrings = new string[] { <M_CONSTANT_STRING_KEYS> };\n"
                    + "\tpublic enum Music { <M_CONSTANTS_ENUM_KEYS> };\n"
                    + "}";
                string sfxTemplate =
                    $"public static class {m_Script.m_NameClassSFX}\n"
                    + "{\n"
                    + "\tpublic const int <S_CONSTANT_INT_KEYS>;\n"
                    + "\tpublic static readonly string[] idStrings = new string[] { <S_CONSTANT_STRING_KEYS> };\n"
                    + "\tpublic enum Sfx { <S_CONSTANTS_ENUM_KEYS> };\n"
                    + "}";
                string content = "";
                if (!string.IsNullOrEmpty(m_Script.m_Namespace))
                {
                    content = $"namespace {m_Script.m_Namespace}" + "\n{" + $"\n\t{musicTemplate}" + $"\n\t{sfxTemplate}" + "\n}";
                }
                else
                {
                    content = $"\n{musicTemplate}\n{sfxTemplate}";
                }
                return content;
            }
        }
#endif
    }
}