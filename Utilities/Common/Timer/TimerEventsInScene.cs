using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RCore.Common
{
    public class TimerEventsInScene : TimerEvents
    {
        private static TimerEventsInScene m_Instance;
        public static TimerEventsInScene Instance
        {
            get
            {
                if (m_Instance == null)
                    CreatInstance();

                return m_Instance;
            }
        }

        public static void CreatInstance()
        {
            var obj = new GameObject("TimerEventsInScene");
            m_Instance = obj.AddComponent<TimerEventsInScene>();
            obj.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}