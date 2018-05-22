using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                var t = typeof(T);
                instance = (new GameObject(t.Name)).AddComponent<T>();
            }
            else if (instance != FindObjectOfType<T>())
            {
                Destroy(FindObjectOfType<T>());
            }
            DontDestroyOnLoad(FindObjectOfType<T>());
            return instance;
        }
    }
}