#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using RCore.Common;
using Debug = UnityEngine.Debug;

namespace RCore.Example
{
    public class ExamplePoolsManager : MonoBehaviour
    {
#region Members

        private static ExamplePoolsManager mInstance;
        public static ExamplePoolsManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = FindObjectOfType<ExamplePoolsManager>();
                return mInstance;
            }
        }

        [SerializeField] private CustomPool<Transform> mBuiltInPool; //This is a example single pool outside of Pool container

        /// <summary>
        /// Container which contain all pools of Transform Object
        /// </summary>
        private PoolsContainer<Transform> mPoolsContainerTransform;
        /// <summary>
        /// Container which contain all pools of Image Objects
        /// </summary>
        private PoolsContainer<Image> mImagePools;

        public Transform redCubePrefab;
        public Transform blueCubePrefab;
        public Transform greenCubePrefab;
        public Transform spherePrefab;

#endregion

        //============================================================

#region MonoBehaviour

        private void Start()
        {
            // Simple Benchmark, It basically is FPS Counter
            TimerEventsInScene.Instance.StartBenchmark(300, (fPS, minFPS, maxFPS) => Debug.Log($"Benchmark Finished: FPS:{fPS} MinFPS:{minFPS} MaxFPS:{maxFPS}"));

            //Simple Wait For Work
            int a = 0;
            TimerEventsInScene.Instance.WaitForSeconds(3f, s => Debug.Log("Wait 3s"));
            TimerEventsInScene.Instance.WaitForSeconds(5f, s => Debug.Log("Wait 5f"));
            TimerEventsInScene.Instance.WaitForSeconds(7f, s => Debug.Log("Wait 7f"));
            TimerEventsInScene.Instance.WaitForSeconds(9f, s =>
            {
                Debug.Log("Wait 9f");
                a = 9;
            });
            TimerEventsInScene.Instance.WaitForCondition(() => a == 9, () => Debug.Log("Wait till a = 9"));
        }

        public void Init()
        {
            //Init pools manager for cube prefabs
            mPoolsContainerTransform = new PoolsContainer<Transform>("pool", 3, transform);

            //Set parent for build in pool
            mBuiltInPool.SetParent(mPoolsContainerTransform.container);

            //Add buildin pool to dictionary of multi-pools
            mPoolsContainerTransform.Add(mBuiltInPool);
        }

#endregion

        //===============================================================

#region Public

        /// <summary>
        /// Release pooled object
        /// </summary>
        public void Spawn(Transform prefab, Vector3 position)
        {
            mPoolsContainerTransform.Spawn(prefab, position);
            var pool = mPoolsContainerTransform.Get(prefab);
            var activeList = pool.ActiveList();
            if (activeList.Count > 10)
                pool.Release(activeList[0]);
        }

        /// <summary>
        /// Spawn pooled object
        /// </summary>
        public void Spawn(Transform prefab, Transform position)
        {
            mPoolsContainerTransform.Spawn(prefab, position);
            var pool = mPoolsContainerTransform.Get(prefab);
            var activeList = pool.ActiveList();
            if (activeList.Count > 10)
                pool.Release(activeList[0]);
        }

        /// <summary>
        /// Release pooled object
        /// </summary>
        public void Release(Transform pObj)
        {
            mPoolsContainerTransform.Release(pObj);
        }

        /// <summary>
        /// Release pooled object
        /// </summary>
        public void Release(GameObject pObj)
        {
            mPoolsContainerTransform.Release(pObj);
        }

        /// <summary>
        /// Auto release pooled object after seconds
        /// </summary>
        public void Release(Transform pObj, float pCountdown)
        {
            if (pCountdown == 0)
            {
                TimerEventsInScene.Instance.RemoveCountdownEvent(pObj.GetInstanceID());
                mPoolsContainerTransform.Release(pObj);
            }
            else
            {
                TimerEventsInScene.Instance.WaitForSeconds(new CountdownEvent()
                {
                    id = pObj.GetInstanceID(),
                    waitTime = pCountdown,
                    onTimeOut = s => mPoolsContainerTransform.Release(pObj)
                });
            }
        }

        /// <summary>
        /// Auto release pooled object after seconds
        /// </summary>
        public void Release(GameObject pObj, float pCountdown)
        {
            if (pCountdown == 0)
            {
                TimerEventsInScene.Instance.RemoveCountdownEvent(pObj.GetInstanceID());
                mPoolsContainerTransform.Release(pObj);
            }
            else
            {
                TimerEventsInScene.Instance.WaitForSeconds(new CountdownEvent()
                {
                    id = pObj.GetInstanceID(),
                    waitTime = pCountdown,
                    onTimeOut = s => mPoolsContainerTransform.Release(pObj)
                });
            }
        }

#endregion

        //======================================================================

#region Editor

#if UNITY_EDITOR
        [CustomEditor(typeof(ExamplePoolsManager))]
        public class ExamplePoolsManagerEditor : UnityEditor.Editor
        {
            private ExamplePoolsManager mTarget;

            private void OnEnable()
            {
                mTarget = target as ExamplePoolsManager;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (Application.isPlaying)
                    mTarget.mPoolsContainerTransform.DrawOnEditor();
                else
                    EditorGUILayout.HelpBox("Click play to see how it work", MessageType.Info);
            }
        }
#endif

#endregion
    }
}