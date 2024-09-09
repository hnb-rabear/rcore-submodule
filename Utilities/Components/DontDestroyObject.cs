/***
 * Author RadBear - nbhung71711@gmail.com - 2017
 **/

using System.Collections;
using UnityEngine;

namespace RCore.Components
{
    public class DontDestroyObject : MonoBehaviour
    {
        private static DontDestroyObject m_Instance;

        private IEnumerator Start()
        {
            if (m_Instance == null)
                m_Instance = this;
            else if (m_Instance != this)
                Destroy(gameObject);
            yield return null;
            if (transform.childCount > 0)
                DontDestroyOnLoad(gameObject);
            else
                Destroy(gameObject);
        }
    }
}