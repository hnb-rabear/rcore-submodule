using System;
using System.Collections.Generic;
using RCore.Common;
using UnityEngine;

namespace RCore.Framework.Data
{
	public class TimerTaskManager : MonoBehaviour
	{
		private const int MILLISECONDS_PER_SECOND = 1000;

		private static TimerTaskManager m_Instance;
		public static TimerTaskManager Instance
		{
			get
			{
				if (m_Instance == null)
				{
					var obj = new GameObject("TimerEventsGlobal");
					m_Instance = obj.AddComponent<TimerTaskManager>();
					obj.hideFlags = HideFlags.HideAndDontSave;
				}

				return m_Instance;
			}
		}

		private DateTime m_dayZero = new(2017, 1, 1);
		private long m_localMilliSecondsSinceBoot;
		public bool stop { get; set; }
		private float m_secondsElapsed;
		private List<TimerTask> m_timerTasks = new List<TimerTask>();

		public long GetSecondsSinceBoot()
		{
			var localMillisSeconds = (long)(DateTime.UtcNow - m_dayZero).TotalMilliseconds;
			if (m_localMilliSecondsSinceBoot == 0)
				m_localMilliSecondsSinceBoot = RNative.getMillisSinceBoot() - localMillisSeconds;

			var millisSinceBoot = localMillisSeconds + m_localMilliSecondsSinceBoot;
			return millisSinceBoot / MILLISECONDS_PER_SECOND;
		}

		public long GetCurrentServerSeconds()
		{
			var now = TimeHelper.GetServerTimeUtc();
			if (now != null)
				return (long)(now.Value - m_dayZero).TotalSeconds;
			return 0;
		}

		public void AddTimerTask(TimerTask pTimer)
		{
			if (!m_timerTasks.Contains(pTimer))
				m_timerTasks.Add(pTimer);
		}

		public void Update()
		{
			int count = m_timerTasks.Count;
			if (count > 0)
			{
				m_secondsElapsed += Time.unscaledDeltaTime;
				if (m_secondsElapsed >= 1.0f)
				{
					m_secondsElapsed -= 1.0f;
					var currentServerSeconds = GetCurrentServerSeconds();
					var currentLocalSeconds = GetSecondsSinceBoot();
					for (int i = count - 1; i >= 0; i--)
					{
						var task = m_timerTasks[i];
						if (task != null)
						{
							if (task.IsRunning)
								task.Update(currentServerSeconds, currentLocalSeconds);
							else
								m_timerTasks.RemoveAt(i);
						}
					}
				}
			}
		}
	}
}