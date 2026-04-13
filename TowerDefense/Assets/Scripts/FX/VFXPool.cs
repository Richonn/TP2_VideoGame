using System.Collections.Generic;
using UnityEngine;

public class VFXPool
{
    private readonly GameObject _prefab;
    private readonly Transform _parent;
    private readonly Queue<GameObject> _pool = new Queue<GameObject>();

    public VFXPool(GameObject prefab, Transform parent, int prewarm = 0)
    {
        _prefab = prefab;
        _parent = parent;
        for (int i = 0; i < prewarm; i++)
            _pool.Enqueue(CreateInstance());
    }

    private GameObject CreateInstance()
    {
        GameObject go = Object.Instantiate(_prefab, _parent);
        go.SetActive(false);
        return go;
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        GameObject go = _pool.Count > 0 ? _pool.Dequeue() : CreateInstance();
        go.transform.SetPositionAndRotation(position, rotation);
        go.SetActive(true);
        return go;
    }

    public void Release(GameObject go)
    {
        if (go == null) return;
        go.SetActive(false);
        _pool.Enqueue(go);
    }
}
