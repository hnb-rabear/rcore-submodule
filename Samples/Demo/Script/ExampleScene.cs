using UnityEngine;

namespace RCore.Demo
{
    public class ExampleScene : MonoBehaviour
    {
        void Start()
        {
            ExampleGameKeyValueDB.Instance.Init();
            MainPanel.instance.Init();
            ExamplePoolsManager.Instance.Init();
        }
    }
}