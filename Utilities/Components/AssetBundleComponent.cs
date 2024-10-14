#if ADDRESSABLES
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RCore.Sample
{
	public class AssetBundleComponent : MonoBehaviour
	{
		public bool auto;
		public Transform parent;
		public AssetReferenceGameObject reference;
		internal bool loading { get; private set; }
		internal GameObject instance { get; private set; }
		private void Start()
		{
			if (auto)
				InstantiateAsync();
		}
		public async UniTask<T> InstantiateAsync<T>() where T : Component
		{
			if (instance == null)
			{
				loading = true;
				instance = await Addressables.InstantiateAsync(reference, parent);
				loading = false;
				if (instance != null)
				{
					instance.transform.localPosition = Vector3.zero;
					Debug.Log($"Instantiate Asset Bundle {instance.name}");
				}
			}
			var component = instance.GetComponent<T>();
			return component;
		}
		public async UniTask<GameObject> InstantiateAsync()
		{
			if (instance == null)
			{
				loading = true;
				instance = await Addressables.InstantiateAsync(reference, parent);
				loading = false;
				if (instance != null)
				{
					instance.transform.localPosition = Vector3.zero;
					instance.name = instance.name;
					Debug.Log($"Instantiate Asset Bundle {instance.name}");
				}
			}
			return instance;
		}
		private void OnDestroy()
		{
			if (instance != null)
			{
				Debug.Log($"Unload Asset Bundle {instance.name}");
				Addressables.ReleaseInstance(instance.gameObject);
			}
		}
	}
}
#endif