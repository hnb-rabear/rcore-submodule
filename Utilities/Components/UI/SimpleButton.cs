using UnityEngine;
using UnityEngine.UI;

namespace RCore.Components
{
    [AddComponentMenu("RCore/UI/SimpleButton")]
    public class SimpleButton : JustButton
    {
        [SerializeField]
        protected Text mLabel;
        public Text label
        {
            get
            {
                if (mLabel == null && !m_findLabel)
                {
                    mLabel = GetComponentInChildren<Text>();
                    m_findLabel = true;
                }
                return mLabel;
            }
        }
        private bool m_findLabel;

#if UNITY_EDITOR
        [ContextMenu("Validate")]
        protected override void OnValidate()
        {
            base.OnValidate();

            if (mLabel == null)
                mLabel = GetComponentInChildren<Text>();
        }
#endif
    }
}