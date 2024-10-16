﻿/***
 * Author RadBear - nbhung71711 @gmail.com - 2018
 **/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using RCore.Common;
using UnityEngine.Serialization;

namespace RCore.Components
{
    [AddComponentMenu("RCore/UI/JustButton")]
    public class JustButton : Button
    {
        protected enum PivotForScale
        {
            Bot,
            Top,
            TopLeft,
            BotLeft,
            TopRight,
            BotRight,
            Center,
        }

        public enum PerfectRatio
        {
            None,
            Width,
            Height,
        }

        private static Material m_GreyMat;

        [SerializeField] protected PivotForScale mPivotForFX = PivotForScale.Center;
        [SerializeField] protected bool mEnabledFX = true;
        [SerializeField] protected Image mImg;
        [SerializeField] protected Vector2 mInitialScale = Vector2.one;
        [SerializeField] protected PerfectRatio m_PerfectRatio = PerfectRatio.Height;

        [SerializeField] protected bool mGreyMatEnabled;
        [SerializeField] protected bool mImgSwapEnabled;
        [SerializeField] public Sprite mImgActive;
        [SerializeField] public Sprite mImgInactive;
        [FormerlySerializedAs("m_SFXClip")]
        [SerializeField] protected string m_SfxClip = "sfx_button_click";

        public Image img
        {
            get
            {
                if (mImg == null)
                    mImg = targetGraphic as Image;
                return mImg;
            }
        }
        public Material imgMaterial
        {
            get => img.material;
            set => img.material = value;
        }
        public RectTransform rectTransform => targetGraphic.rectTransform;

        private PivotForScale m_prePivot;
        private Action m_inactionStateAction;
        private bool m_active = true;
        private int m_PerfectSpriteId;

        public virtual void SetEnable(bool pValue)
        {
            m_active = pValue;
            enabled = pValue || m_inactionStateAction != null;
            if (pValue)
            {
                if (mImgSwapEnabled)
                    mImg.sprite = mImgActive;
                else
                    imgMaterial = null;
            }
            else
            {
                if (mImgSwapEnabled)
                {
                    mImg.sprite = mImgInactive;
                }
                else
                {
                    transform.localScale = mInitialScale;

                    //Use grey material here
                    if (mGreyMatEnabled)
                        imgMaterial = GetGreyMat();
                }
            }
        }

        public virtual void SetInactiveStateAction(Action pAction)
        {
            m_inactionStateAction = pAction;
            enabled = m_active || m_inactionStateAction != null;
        }

        protected override void Start()
        {
            base.Start();

            m_prePivot = mPivotForFX;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (mEnabledFX)
                transform.localScale = mInitialScale;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (Application.isPlaying)
                return;

            base.OnValidate();

            if (targetGraphic == null)
            {
                var images = gameObject.GetComponentsInChildren<Image>();
                if (images.Length > 0)
                {
                    targetGraphic = images[0];
                    mImg = (Image)targetGraphic;
                }
            }
            if (targetGraphic != null && mImg == null)
                mImg = targetGraphic as Image;

            if (transition == Transition.Animation && GetComponent<Animator>() != null)
            {
                GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
                mEnabledFX = false;
            }
            
            if (mEnabledFX)
                mInitialScale = transform.localScale;

            RefreshPivot();

            m_PerfectSpriteId = 0;
            CheckPerfectRatio();
        }
#endif

        protected override void OnEnable()
        {
            base.OnEnable();

            if (mEnabledFX)
                transform.localScale = mInitialScale;
            
            if (mImg.sprite != null && m_PerfectSpriteId != mImg.sprite.GetInstanceID())
                CheckPerfectRatio();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!m_active)
            {
                if (m_inactionStateAction != null)
                {
                    m_inactionStateAction();
                    if (TryGetComponent(out Animator component))
                        component.SetTrigger("Pressed");
                }
            }

            if (m_active)
            {
                base.OnPointerDown(eventData);
                if (!string.IsNullOrEmpty(m_SfxClip))
                    EventDispatcher.Raise(new SFXTriggeredEvent(m_SfxClip));
            }

            if (mEnabledFX)
            {
                if (mPivotForFX != m_prePivot)
                {
                    m_prePivot = mPivotForFX;
                    RefreshPivot(rectTransform);
                }

                transform.localScale = mInitialScale * 0.95f;
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (m_active)
                base.OnPointerUp(eventData);

            if (mEnabledFX)
            {
                transform.localScale = mInitialScale;
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (m_active)
                base.OnPointerClick(eventData);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (m_active)
                base.OnSelect(eventData);
        }

        public void RefreshPivot()
        {
            RefreshPivot(rectTransform);
        }

        private void RefreshPivot(RectTransform pRect)
        {
            switch (mPivotForFX)
            {
                case PivotForScale.Bot:
                    SetPivot(pRect, new Vector2(0.5f, 0));
                    break;
                case PivotForScale.BotLeft:
                    SetPivot(pRect, new Vector2(0, 0));
                    break;
                case PivotForScale.BotRight:
                    SetPivot(pRect, new Vector2(1, 0));
                    break;
                case PivotForScale.Top:
                    SetPivot(pRect, new Vector2(0.5f, 1));
                    break;
                case PivotForScale.TopLeft:
                    SetPivot(pRect, new Vector2(0, 1f));
                    break;
                case PivotForScale.TopRight:
                    SetPivot(pRect, new Vector2(1, 1f));
                    break;
                case PivotForScale.Center:
                    SetPivot(pRect, new Vector2(0.5f, 0.5f));
                    break;
            }
        }

        public void SetPivot(RectTransform pRectTransform, Vector2 pivot)
        {
            if (pRectTransform == null) return;

            var size = pRectTransform.rect.size;
            var deltaPivot = pRectTransform.pivot - pivot;
            var deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y);
            pRectTransform.pivot = pivot;
            pRectTransform.localPosition -= deltaPosition;
        }

        public Material GetGreyMat()
        {
            if (m_GreyMat == null)
                //mGreyMat = new Material(Shader.Find("NBCustom/Sprites/Greyscale"));
                m_GreyMat = Resources.Load<Material>("Greyscale");
            return m_GreyMat;
        }

        public void EnableGrey(bool pValue)
        {
            mGreyMatEnabled = pValue;
        }

        public bool Enabled() { return enabled && m_active; }
        
        public void SetActiveSprite(Sprite pSprite)
        {
            mImgActive = pSprite;
        }

        protected void CheckPerfectRatio()
        {
            if (m_PerfectRatio == PerfectRatio.Width)
            {
                var image1 = mImg;
                if (image1 != null && image1.sprite != null && image1.type == Image.Type.Sliced && m_PerfectSpriteId != image1.sprite.GetInstanceID())
                {
                    var nativeSize = image1.sprite.NativeSize();
                    var rectSize = rectTransform.sizeDelta;
                    if (rectSize.x > 0 && rectSize.x < nativeSize.x)
                    {
                        var ratio = nativeSize.x * 1f / rectSize.x;
                        image1.pixelsPerUnitMultiplier = ratio;
                    }
                    else
                        image1.pixelsPerUnitMultiplier = 1;
                    m_PerfectSpriteId = image1.sprite.GetInstanceID();
                }
            }
            else if (m_PerfectRatio == PerfectRatio.Height)
            {
                var image1 = mImg;
                if (image1 != null && image1.sprite != null && image1.type == Image.Type.Sliced && m_PerfectSpriteId != image1.sprite.GetInstanceID())
                {
                    var nativeSize = image1.sprite.NativeSize();
                    var rectSize = rectTransform.sizeDelta;
                    if (rectSize.y > 0 && rectSize.y < nativeSize.y)
                    {
                        var ratio = nativeSize.y * 1f / rectSize.y;
                        image1.pixelsPerUnitMultiplier = ratio;
                    }
                    else
                        image1.pixelsPerUnitMultiplier = 1;
                    m_PerfectSpriteId = image1.sprite.GetInstanceID();
                }
            }
        }
    }
}