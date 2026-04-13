using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourcePool
{
    private readonly Transform _parent;
    private readonly AudioMixerGroup _mixerGroup;
    private readonly Queue<AudioSource> _pool = new Queue<AudioSource>();
    private readonly List<AudioSource> _active = new List<AudioSource>();

    public AudioSourcePool(Transform parent, AudioMixerGroup mixerGroup, int prewarm = 8)
    {
        _parent = parent;
        _mixerGroup = mixerGroup;
        for (int i = 0; i < prewarm; i++)
            _pool.Enqueue(CreateSource());
    }

    private AudioSource CreateSource()
    {
        GameObject go = new GameObject("PooledAudioSource");
        go.transform.SetParent(_parent);
        AudioSource src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.outputAudioMixerGroup = _mixerGroup;
        go.SetActive(false);
        return src;
    }

    public AudioSource Get()
    {
        AudioSource src = _pool.Count > 0 ? _pool.Dequeue() : CreateSource();
        src.gameObject.SetActive(true);
        _active.Add(src);
        return src;
    }

    public void Release(AudioSource src)
    {
        if (src == null) return;
        src.Stop();
        src.clip = null;
        src.gameObject.SetActive(false);
        _active.Remove(src);
        _pool.Enqueue(src);
    }

    public void Tick()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            AudioSource src = _active[i];
            if (src == null) { _active.RemoveAt(i); continue; }
            if (!src.isPlaying) Release(src);
        }
    }
}
