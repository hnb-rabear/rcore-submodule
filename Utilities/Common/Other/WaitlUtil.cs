/**
 * Author RadBear - nbhung71711@gmail.com - 2018
 **/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace RCore.Common
{
    public abstract class WaitUtil
    {
        [Serializable]
        public class CountdownEvent
        {
            public int id;
            public Action<float> onTimeOut;
            public ConditionalDelegate breakCondition;
            public float waitTime;
            public bool unscaledTime;
            public bool autoRestart;

            public float Elapsed { get; private set; }
            public float ElapsedOffset { get; private set; }
            public bool IsTimeOut => Elapsed >= waitTime;
            public float RemainSeconds() => waitTime - Elapsed > 0 ? waitTime - Elapsed : 0;

            public CountdownEvent(int pid = 0)
            {
                id = pid;
            }

            public void Set(CountdownEvent other)
            {
                id = other.id;
                onTimeOut = other.onTimeOut;
                breakCondition = other.breakCondition;
                waitTime = other.waitTime;
                unscaledTime = other.unscaledTime;
                Elapsed = other.Elapsed;
                ElapsedOffset = other.ElapsedOffset;
            }

            public virtual void AddElapsedTime(float pValue)
            {
                Elapsed += pValue;
            }

            public void Restart()
            {
                Elapsed = 0;
                ElapsedOffset = 0;
            }

            public void SetElapsedOffset(float pValue)
            {
                ElapsedOffset = pValue;
            }

            public void Run()
            {
                Restart();
                Start(this);
            }

            public void SetId(int pId)
            {
                id = pId;
            }

            public void Stop()
            {
                waitTime = Elapsed;
            }
        }

        public class CountdownIntervalEvent : CountdownEvent
        {
            public Action<float> onUpdate;
            public float updateInterval;
            private float m_updateInterval;
            public override void AddElapsedTime(float pValue)
            {
                base.AddElapsedTime(pValue);

                float remainSeconds = RemainSeconds();
                if (updateInterval <= 0 || remainSeconds <= 0)
                    return;

                m_updateInterval += pValue;
                if (m_updateInterval > updateInterval)
                {
                    m_updateInterval -= updateInterval;
                    onUpdate(remainSeconds);
                }
            }
        }

        //=======================================================

        [Serializable]
        public class ConditionEvent
        {
            public int id;
            public ConditionalDelegate triggerCondition;
            public Action onTrigger;
            public Action onUpdate;

            public ConditionEvent(int pId = 0)
            {
                id = pId;
            }

            public void Set(ConditionEvent other)
            {
                id = other.id;
                triggerCondition = other.triggerCondition;
                onTrigger = other.onTrigger;
                onUpdate = other.onUpdate;
            }

            public void Start()
            {
                WaitUtil.Start(this);
            }
        }

        //=======================================================

        private static CoroutineMediator m_Mediator => CoroutineMediator.Instance;

        /// <summary>
        /// This Wait uses Update to calcualate time
        /// </summary>
        public static CountdownEvent Start(CountdownEvent pScheduleEvent)
        {
            return m_Mediator.WaitForSecond(pScheduleEvent);
        }
        public static CountdownEvent Start(float pTime, Action pDoSomething)
        {
            return m_Mediator.WaitForSecond(new CountdownEvent()
            {
                waitTime = pTime, onTimeOut = (s) =>
                {
                    pDoSomething();
                }
            });
        }
        public static CountdownEvent Start(float pTime, Action<float> pDoSomething)
        {
            return m_Mediator.WaitForSecond(new CountdownEvent() { waitTime = pTime, onTimeOut = pDoSomething });
        }
        public static CountdownEvent Start(float pTime, bool pUnscaledTime, Action<float> pDoSomething)
        {
            return m_Mediator.WaitForSecond(new CountdownEvent() { waitTime = pTime, onTimeOut = pDoSomething, unscaledTime = pUnscaledTime });
        }
        public static void RemoveCountdownEvent(int pId)
        {
            m_Mediator.RemoveTimeAction(pId);
        }
        public static void RemoveCountdownEvent(CountdownEvent pEvent)
        {
            m_Mediator.RemoveTimeAction(pEvent);
        }
        public static void RaiseDelayableEvent(BaseEvent pEvent, float pDelay = 0)
        {
            m_Mediator.AddDelayableEvent(new DelayableEvent(pEvent, pDelay));
        }

        /// <summary>
        /// This Wait uses Update to check condition
        /// </summary>
        public static ConditionEvent Start(ConditionEvent pScheduleEvent)
        {
            return m_Mediator.WaitForCondition(pScheduleEvent);
        }
        public static ConditionEvent Start(ConditionalDelegate pTriggerCondition, Action pDoSomething)
        {
            return m_Mediator.WaitForCondition(new ConditionEvent() { onTrigger = pDoSomething, triggerCondition = pTriggerCondition });
        }
        public static void RemoveConditionEvent(int pId)
        {
            m_Mediator.RemoveTriggerAction(pId);
        }
        public static void RemoveConditionEvent(ConditionEvent pEvent)
        {
            m_Mediator.RemoveTriggerAction(pEvent);
        }

        /// <summary>
        /// External update
        /// </summary>
        public static IUpdate AddUpdate(IUpdate pUpdate)
        {
            return m_Mediator.AddUpdate(pUpdate);
        }
        public static void RemoveUpdate(IUpdate pUpdate)
        {
            m_Mediator.RemoveUpdate(pUpdate);
        }

        /// <summary>
        /// External Queue
        /// </summary>
        public static void Enqueue(Action pDoSomething)
        {
            m_Mediator.Enqueue(pDoSomething);
        }

        public static Task WaitTask(Task pTask, Action pOnTaskFinished)
        {
            Start(new ConditionEvent()
            {
                triggerCondition = () => pTask.IsCompleted,
                onTrigger = pOnTaskFinished
            });
            return pTask;
        }

        public static ResourceRequest WaitTask(ResourceRequest pTask, Action pOnTaskFinished)
        {
            Start(new ConditionEvent()
            {
                triggerCondition = () => pTask.isDone,
                onTrigger = pOnTaskFinished
            });
            return pTask;
        }
    }

    //====================== COUNT DOWN EVENTS SYSTEM =========================

    public class CountdownEventsManager
    {
        private readonly List<WaitUtil.CountdownEvent> m_countdownEvents = new List<WaitUtil.CountdownEvent>();
        private float m_timeBeforePause;
        private float m_pauseSeconds;

        public bool IsEmpty => m_countdownEvents.Count == 0;

        public void LateUpdate()
        {
            lock (m_countdownEvents)
            {
                for (int i = m_countdownEvents.Count - 1; i >= 0; i--)
                {
                    var d = m_countdownEvents[i];

                    if (d.unscaledTime)
                        d.AddElapsedTime(Time.unscaledDeltaTime + m_pauseSeconds);
                    else
                        d.AddElapsedTime(Time.deltaTime);
                    if (d.breakCondition != null && d.breakCondition())
                    {
                        if (!d.autoRestart)
                            m_countdownEvents.Remove(d);
                        else
                            d.Restart();
                    }
                    else if (d.IsTimeOut)
                    {
                        d.onTimeOut(d.Elapsed - d.waitTime);
                        if (!d.autoRestart)
                            m_countdownEvents.Remove(d);
                        else
                            d.Restart();
                    }
                }
            }
            m_pauseSeconds = 0;
        }

        public void OnApplicationPause(bool pause)
        {
            if (pause)
                m_timeBeforePause = Time.realtimeSinceStartup;
            else if (m_timeBeforePause > 0)
                m_pauseSeconds = Time.realtimeSinceStartup - m_timeBeforePause;
        }

        public void Register(WaitUtil.CountdownEvent pEvent)
        {
            if (pEvent.id == 0)
            {
                if (!m_countdownEvents.Contains(pEvent))
                    m_countdownEvents.Add(pEvent);
            }
            else
            {
                bool exist = false;
                for (int i = 0; i < m_countdownEvents.Count; i++)
                {
                    if (pEvent.id == m_countdownEvents[i].id)
                    {
                        exist = true;
                        m_countdownEvents[i] = pEvent;
                        break;
                    }
                }

                if (!exist)
                    m_countdownEvents.Add(pEvent);
            }
        }

        public void UnRegister(int pId)
        {
            for (int i = 0; i < m_countdownEvents.Count; i++)
            {
                var d = m_countdownEvents[i];
                if (d.id == pId)
                {
                    m_countdownEvents.Remove(d);
                    return;
                }
            }
        }

        public void UnRegister(WaitUtil.CountdownEvent pEvent)
        {
            m_countdownEvents.Remove(pEvent);
        }
    }

    //====================== CONDITION EVENTS SYSTEM ===============================

    public class ConditionEventsManager
    {
        private readonly List<WaitUtil.ConditionEvent> m_ConditionEvents = new List<WaitUtil.ConditionEvent>();

        public bool IsEmpty => m_ConditionEvents.Count == 0;

        public void LateUpdate()
        {
            lock (m_ConditionEvents)
            {
                for (int i = m_ConditionEvents.Count - 1; i >= 0; i--)
                {
                    var d = m_ConditionEvents[i];
                    if (d.triggerCondition())
                    {
                        d.onTrigger();
                        m_ConditionEvents.Remove(d);
                    }
                    else
                    {
                        d.onUpdate?.Invoke();
                    }
                }
            }
        }

        public void Register(WaitUtil.ConditionEvent pEvent)
        {
            if (pEvent.id == 0)
            {
                m_ConditionEvents.Add(pEvent);
            }
            else
            {
                bool exist = false;
                for (int i = 0; i < m_ConditionEvents.Count; i++)
                {
                    if (pEvent.id == m_ConditionEvents[i].id)
                    {
                        exist = true;
                        m_ConditionEvents[i] = pEvent;
                        break;
                    }
                }

                if (!exist)
                    m_ConditionEvents.Add(pEvent);
            }
        }

        public void UnRegister(int pId)
        {
            for (int i = 0; i < m_ConditionEvents.Count; i++)
            {
                var d = m_ConditionEvents[i];
                if (d.id == pId)
                {
                    m_ConditionEvents.Remove(d);
                    return;
                }
            }
        }

        public void UnRegister(WaitUtil.ConditionEvent pEvent)
        {
            m_ConditionEvents.Remove(pEvent);
        }
    }

    public class DelayableEvent
    {
        public string key;
        public float delay;
        public BaseEvent @event;
        public DelayableEvent(BaseEvent @event, float pDelay)
        {
            @event = @event;
            key = @event.GetType().ToString();
            delay = pDelay;
        }
    }
}