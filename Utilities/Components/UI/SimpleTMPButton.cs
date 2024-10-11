/***
 * Author RadBear - nbhung71711 @gmail.com - 2018
 **/

using TMPro;
using UnityEngine;
using RCore.Common;

namespace RCore.Components
{
	[AddComponentMenu("RCore/UI/SimpleTMPButton")]
	public class SimpleTMPButton : JustButton
	{
		[SerializeField] protected TextMeshProUGUI mLabelTMP;

		public TextMeshProUGUI labelTMP
		{
			get
			{
				if (mLabelTMP == null && !mFindLabel)
				{
					mLabelTMP = GetComponentInChildren<TextMeshProUGUI>();
					mFindLabel = true;
				}
				return mLabelTMP;
			}
		}

		private bool mFindLabel;

		[SerializeField] protected bool mFontColorSwap;
		[SerializeField] protected Color mFontColorActive;
		[SerializeField] protected Color mFontColorInactive;

		[SerializeField] protected bool m_LabelMatSwap;
		[SerializeField] public Material m_LabelMatActive;
		[SerializeField] public Material m_LabelMatInactive;

#if UNITY_EDITOR
		[ContextMenu("Validate")]
		protected override void OnValidate()
		{
			base.OnValidate();

			if (mLabelTMP == null)
				mLabelTMP = GetComponentInChildren<TextMeshProUGUI>();
			if (mLabelTMP == null)
				m_LabelMatSwap = false;
			if (!m_LabelMatSwap)
			{
				m_LabelMatActive = null;
				m_LabelMatInactive = null;
			}
			else if (m_LabelMatActive == null)
			{
				m_LabelMatActive = mLabelTMP.fontSharedMaterial;
			}
		}
#endif
		
		public override void SetEnable(bool pValue)
		{
			base.SetEnable(pValue);

			if (pValue)
			{
				if (mFontColorSwap)
					mLabelTMP.color = mFontColorActive;
				if (m_LabelMatSwap && m_LabelMatActive != null && m_LabelMatInactive != null)
				{
					var labels = gameObject.FindComponentsInChildren<TextMeshProUGUI>();
					foreach (var label in labels)
					{
						if (label.font == mLabelTMP.font && label.fontSharedMaterial == mLabelTMP.fontSharedMaterial)
							label.fontSharedMaterial = m_LabelMatActive;
					}
				}
			}
			else
			{
				if (mFontColorSwap)
					mLabelTMP.color = mFontColorInactive;
				if (m_LabelMatSwap && m_LabelMatActive != null && m_LabelMatInactive != null)
				{
					var labels = gameObject.FindComponentsInChildren<TextMeshProUGUI>();
					foreach (var label in labels)
					{
						if (label.font == mLabelTMP.font && label.fontSharedMaterial == mLabelTMP.fontSharedMaterial)
							label.fontSharedMaterial = m_LabelMatInactive;
					}
				}
			}
		}
	}
}