
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
	private static T instance;

	public static T Instance
	{
		get {
			if(instance == null)
			{
				instance = GameObject.FindObjectOfType<T>();

				if(instance == null)
				{
					instance = new GameObject(typeof(T).Name).AddComponent<T>();
				}
			}

			return instance;
		}
	}

	protected virtual void Awake()
	{
		if(instance != null && instance != this)
		{
			Debug.LogWarning("Sigleton of type " + typeof(T).Name + "already exists");
			DestroyImmediate(this.gameObject);
		}
	}

	protected virtual void Start()
	{

	}

	protected virtual void Update()
	{

	}
}