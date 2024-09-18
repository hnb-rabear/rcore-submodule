using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if USE_DOTWEEN
#endif

namespace RCore.Components
{
    [Obsolete("Use BaseAudioManager")]
    public class StandaloneAudioManager : BaseAudioManager { }
}