using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
#if USE_DOTWEEN
using DG.Tweening;
#endif

namespace RCore.Components
{
    public class AudioManager : BaseAudioManager
    {
#region Members

        private static AudioManager m_Instance;
        public static AudioManager Instance => m_Instance;

#endregion

        //=====================================

#region MonoBehaviour

        private void Awake()
        {
            if (m_Instance == null)
                m_Instance = this;
            else if (m_Instance != this)
                Destroy(gameObject);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            m_sfxSourceUnlimited.loop = false;
            m_sfxSourceUnlimited.playOnAwake = false;
            m_musicSource.loop = true;
            m_musicSource.playOnAwake = false;
            
            if (audioCollection == null)
                audioCollection = Resources.Load<AudioCollection>("AudioCollection");
        }
#endif

#endregion

        //=====================================

#region Public

        public AudioSource PlaySFX(string pFileName, int limitNumber, bool pLoop = false, float pPitchRandomMultiplier = 1)
        {
            if (!m_enabledSfx) return null;
            var clip = audioCollection.GetSFXClip(pFileName);
            return PlaySFX(clip, limitNumber, pLoop, pPitchRandomMultiplier);
        }

        public AudioSource PlaySFX(int pIndex, int limitNumber, bool pLoop = false, float pPitchRandomMultiplier = 1)
        {
            if (!m_enabledSfx) return null;
            var clip = audioCollection.GetSFXClip(pIndex);
            return PlaySFX(clip, limitNumber, pLoop, pPitchRandomMultiplier);
        }

        public void StopSFX(int pIndex)
        {
            var clip = audioCollection.GetSFXClip(pIndex);
            StopSFX(clip);
        }

        public void PlayMusic(string pFileName, bool pLoop = false, float pFadeDuration = 0)
        {
            var clip = audioCollection.GetMusicClip(pFileName);
            PlayMusic(clip, pLoop, pFadeDuration);
        }

        public void PlayMusicById(int pId, bool pLoop = false, float pFadeDuration = 0, float pVolume = 1f)
        {
            var clip = audioCollection.GetMusicClip(pId);
            PlayMusic(clip, pLoop, pFadeDuration, pVolume);
        }

        public void PlayMusicByIds(int[] pIds, float pFadeDuration = 0, float pVolume = 1f)
        {
            var clips = new AudioClip[pIds.Length];
            for (int i = 0; i < pIds.Length; i++)
                clips[i] = audioCollection.GetMusicClip(pIds[i]);
            PlayMusics(clips, pFadeDuration, pVolume);
        }

#endregion

        //=====================================

#region Private

#if UNITY_EDITOR

        [CustomEditor(typeof(AudioManager))]
        protected class AudioManagerEditor : BaseAudioManagerEditor
        {
            private AudioManager m_Target;

            protected override void OnEnable()
            {
                base.OnEnable();

                m_Target = target as AudioManager;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (m_Target.audioCollection == null)
                    EditorGUILayout.HelpBox("AudioManager require AudioCollection. " + "To create AudioCollection, In project window select RUtilities/Create Audio Collection", MessageType.Error);
                else if (GUILayout.Button("Open Audio Collection"))
                    Selection.activeObject = m_Target.audioCollection;
            }
        }

#endif

#endregion
    }
}