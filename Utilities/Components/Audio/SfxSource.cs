using System;
using UnityEngine;
using RCore.Common;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using RCore.Inspector;
using UnityEngine.Serialization;
using Debug = RCore.Common.Debug;
#if UNITY_EDITOR
using RCore.Common.Editor;
using UnityEditor;
using UnityEditor.Events;
#endif

namespace RCore.Components
{
    public class SfxSource : MonoBehaviour
    {
        [SerializeField] private string[] mClips;
        [SerializeField] private bool mIsLoop;
        [SerializeField, Range(0.5f, 2f)] private float m_PitchRandomMultiplier = 1f;
        [SerializeField] private int m_Limit;
        /// <summary>
        /// Standalone audio source
        /// </summary>
        [SerializeField] private AudioSource m_AudioSource;
        /// <summary>
        /// Standalone audio volume
        /// </summary>
        [SerializeField, Range(0, 1f)] private float m_Vol = 1f;

        private bool m_initialized;
        private int[] m_indexes = Array.Empty<int>();

        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            Init();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (m_AudioSource == null)
                m_AudioSource = GetComponent<AudioSource>();

            if (m_AudioSource != null)
            {
                m_AudioSource.loop = false;
                m_AudioSource.playOnAwake = false;
            }
        }

        private void Init()
        {
            if (m_initialized)
                return;

            if (AudioManager.Instance == null)
            {
                Debug.LogError("Not found Audio Manager!");
                return;
            }

            m_indexes = new int[mClips.Length];
            for (int i = 0; i < mClips.Length; i++)
                AudioManager.Instance.audioCollection.GetSFXClip(mClips[i], out m_indexes[i]);

            m_initialized = true;
        }

        public void SetLoop(bool pLoop) => mIsLoop = pLoop;

        public void SetVolume(float pVal) => m_Vol = pVal;

        public void SetUp(string[] pClips, bool pIsLoop)
        {
            mClips = pClips;
            mIsLoop = pIsLoop;
        }

        public void PlaySFX()
        {
            if (m_initialized)
                return;
            var audioManager = AudioManager.Instance;
            if (!audioManager.EnabledSFX)
                return;

            if (m_indexes.Length > 0)
            {
                int index = m_indexes[Random.Range(0, mClips.Length)];
                if (m_AudioSource == null)
                {
                    if (audioManager != null)
                        audioManager.PlaySFX(index, m_Limit, mIsLoop, m_PitchRandomMultiplier);
                }
                else
                {
                    if (!audioManager.EnabledSFX)
                        return;
                    var clip = audioManager.audioCollection.GetSFXClip(index);
                    m_AudioSource.volume = audioManager.SFXVolume * audioManager.MasterVolume * m_Vol;
                    m_AudioSource.loop = mIsLoop;
                    m_AudioSource.clip = clip;
                    m_AudioSource.pitch = 1;
                    if (m_PitchRandomMultiplier != 1)
                    {
                        if (Random.value < .5)
                            m_AudioSource.pitch *= Random.Range(1 / m_PitchRandomMultiplier, 1);
                        else
                            m_AudioSource.pitch *= Random.Range(1, m_PitchRandomMultiplier);
                    }
                    if (!mIsLoop)
                        m_AudioSource.PlayOneShot(clip);
                    else
                        m_AudioSource.Play();
                }
            }
        }

        public void StopSFX()
        {
            if (m_AudioSource == null)
            {
                UnityEngine.Debug.LogError($"This AudioSFX must have independent to able to stop, {gameObject.name}");
                return;
            }

            m_AudioSource.Stop();
        }

#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(SfxSource))]
        private class AudioSFXEditor : UnityEditor.Editor
        {
            private AudioCollection m_tempAudioCollection;
            private EditorPrefsString m_audioCollectionPath;
            private SfxSource m_script;
            private string m_search = "";
            private UnityEngine.UI.Button m_button;

            private void OnEnable()
            {
                m_script = target as SfxSource;

                m_script.mClips ??= Array.Empty<string>();

                m_button = m_script.GetComponent<UnityEngine.UI.Button>();
                m_audioCollectionPath = new EditorPrefsString($"{typeof(AudioCollection).FullName}");

                if (m_tempAudioCollection == null)
                {
                    if (!string.IsNullOrEmpty(m_audioCollectionPath.Value))
                        m_tempAudioCollection = (AudioCollection)AssetDatabase.LoadAssetAtPath(m_audioCollectionPath.Value, typeof(AudioCollection));
                }
                if (m_tempAudioCollection == null)
                {
                    var audioManager = FindObjectOfType<BaseAudioManager>();
                    if (audioManager != null)
                    {
                        m_tempAudioCollection = audioManager.audioCollection;
                        m_audioCollectionPath.Value = AssetDatabase.GetAssetPath(m_tempAudioCollection);
                    }
                }
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                EditorHelper.Separator("Editor");

                if (m_tempAudioCollection == null)
                {
                    if (m_tempAudioCollection == null)
                        EditorGUILayout.HelpBox("AudioSFX require AudioCollection. " + "To create AudioCollection, select Project windows RUtilities/Create Audio Collection", MessageType.Error);

                    var asset = (AudioCollection)EditorHelper.ObjectField<AudioCollection>(m_tempAudioCollection, "Audio Collection", 120);
                    if (asset != m_tempAudioCollection)
                    {
                        m_tempAudioCollection = asset;
                        if (m_tempAudioCollection != null)
                            m_audioCollectionPath.Value = AssetDatabase.GetAssetPath(m_tempAudioCollection);
                    }
                    return;
                }

                m_tempAudioCollection = (AudioCollection)EditorHelper.ObjectField<AudioCollection>(m_tempAudioCollection, "Audio Collection", 120);

                if (m_script.mClips.Length > 0)
                    EditorHelper.BoxVertical(() =>
                    {
                        for (int i = 0; i < m_script.mClips.Length; i++)
                        {
                            int i1 = i;
                            EditorHelper.BoxHorizontal(() =>
                            {
                                EditorHelper.TextField(m_script.mClips[i1], "");
                                if (EditorHelper.ButtonColor("x", Color.red, 24))
                                {
                                    var list = m_script.mClips.ToList();
                                    list.Remove(m_script.mClips[i1]);
                                    m_script.mClips = list.ToArray();
                                }
                            });
                        }
                    }, Color.yellow, true);

                EditorHelper.BoxVertical(() =>
                {
                    m_search = EditorHelper.TextField(m_search, "Search");
                    if (!string.IsNullOrEmpty(m_search))
                    {
                        var sfxNames = m_tempAudioCollection.GetSFXNames();
                        if (sfxNames != null && sfxNames.Length > 0)
                        {
                            for (int i = 0; i < sfxNames.Length; i++)
                            {
                                if (sfxNames[i].ToLower().Contains(m_search.ToLower()))
                                {
                                    if (GUILayout.Button(sfxNames[i]))
                                    {
                                        var list = m_script.mClips.ToList();
                                        if (!list.Contains(sfxNames[i]))
                                        {
                                            list.Add(sfxNames[i]);
                                            m_script.mClips = list.ToArray();
                                            m_search = "";
                                            EditorGUI.FocusTextInControl(null);
                                        }
                                    }
                                }
                            }
                        }
                        else
                            EditorGUILayout.HelpBox("No results", MessageType.Warning);
                    }
                }, Color.white, true);

                if (EditorHelper.ButtonColor("Open Sounds Collection"))
                    Selection.activeObject = m_tempAudioCollection;

                if (m_button != null)
                {
                    if (EditorHelper.ButtonColor("Add to OnClick event"))
                    {
                        UnityAction action = m_script.PlaySFX;
                        UnityEventTools.AddVoidPersistentListener(m_button.onClick, action);
                    }
                }

                if (GUI.changed)
                    EditorUtility.SetDirty(m_script);
            }
        }
#endif
    }
}