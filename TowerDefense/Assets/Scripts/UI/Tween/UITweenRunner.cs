using UnityEngine;

public class UITweenRunner : MonoBehaviour
{
    private static UITweenRunner _instance;

    public static UITweenRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("UITweenRunner");
                _instance = go.AddComponent<UITweenRunner>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
