using UnityEngine;

namespace RCore.Example
{
    public class ExampleScene : MonoBehaviour
    {
        void Start()
        {
            ExampleGameData.Instance.Init();
            MainPanel.instance.Init();
            ExamplePoolsManager.Instance.Init();
        }
    }
}