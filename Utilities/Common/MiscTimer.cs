using System;
using UnityEngine;

namespace RCore.Common
{
    public class MiscTimer
    {
        public Action onFinished;
        public float timeTarget;
        private bool m_active;
        private bool m_finished;
        private float m_elapsedTime;

        public bool IsRunning => m_active && !m_finished;
        public float RemainTime => timeTarget - m_elapsedTime;

        public MiscTimer()
        {
            m_finished = true;
        }

        public void UpdateWithTimeScale(float pElapsedTime)
        {
            if (m_active)
            {
                m_elapsedTime += pElapsedTime * Time.timeScale;
                if (m_elapsedTime > timeTarget)
                    Finish();
            }
        }

        public void Update(float pElapsedTime)
        {
            if (m_active)
            {
                m_elapsedTime += pElapsedTime;
                if (m_elapsedTime > timeTarget)
                    Finish();
            }
        }

        public void Start(float pTagetTime)
        {
            if (pTagetTime <= 0)
            {
                m_finished = true;
                m_active = false;
            }
            else
            {
                m_elapsedTime = 0;
                timeTarget = pTagetTime;
                m_finished = false;
                m_active = true;
            }
        }

        public void Finish()
        {
            m_elapsedTime = timeTarget;
            m_active = false;
            m_finished = true;

            onFinished?.Invoke();
        }

        internal void SetElapsedTime(float pValue)
        {
            m_elapsedTime = pValue;
        }

        public float GetElapsedTime() => m_elapsedTime;

        public void Stop()
        {
            m_elapsedTime = 0;
            m_finished = false;
            m_active = false;
        }
    }
}