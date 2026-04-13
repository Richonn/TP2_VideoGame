using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class DynamicMusicDirector : MonoBehaviour
{
    public static DynamicMusicDirector Instance { get; private set; }

    public enum Jingle { WaveStart, WaveComplete, BaseLowHP, Victory, Defeat }

    [System.Serializable]
    public class MusicState
    {
        public MusicTrack track;
        public AudioClip drums;
        public AudioClip melody;
        public AudioClip strings;
        [Range(0f, 1f)] public float drumsVolume = 0.9f;
        [Range(0f, 1f)] public float melodyVolume = 0.9f;
        [Range(0f, 1f)] public float stringsVolume = 0.9f;
    }

    [System.Serializable]
    public class JingleEntry
    {
        public Jingle id;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    [Header("Mixer")]
    [SerializeField] private AudioMixerGroup musicGroup;

    [Header("Tempo (used for beat-locked crossfade)")]
    [SerializeField] private float bpm = 120f;
    [SerializeField] private int beatsPerBar = 4;

    [Header("States")]
    [SerializeField] private MusicState[] states;

    [Header("Jingles")]
    [SerializeField] private JingleEntry[] jingles;

    [Header("Low HP threshold")]
    [Range(0f, 1f)] [SerializeField] private float lowHpRatio = 0.4f;

    private AudioSource _drumsA, _drumsB;
    private AudioSource _melodyA, _melodyB;
    private AudioSource _stringsA, _stringsB;
    private AudioSource _jingleSource;
    private bool _usingA = true;
    private MusicTrack _currentTrack = MusicTrack.None;
    private int _maxBaseHP = 1;
    private double _trackStartDspTime;

    public MusicTrack CurrentTrack => _currentTrack;
    public float SecondsPerBeat => 60f / Mathf.Max(1f, bpm);

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _drumsA = CreateLayer("Drums_A", true);
        _drumsB = CreateLayer("Drums_B", true);
        _melodyA = CreateLayer("Melody_A", true);
        _melodyB = CreateLayer("Melody_B", true);
        _stringsA = CreateLayer("Strings_A", true);
        _stringsB = CreateLayer("Strings_B", true);
        _jingleSource = CreateLayer("Jingles", false);
    }

    void OnEnable()
    {
        GameManager.OnPhaseChanged += OnPhase;
        GameManager.OnWaveChanged += OnWave;
        BaseController.OnHPChanged += OnHP;
    }

    void OnDisable()
    {
        GameManager.OnPhaseChanged -= OnPhase;
        GameManager.OnWaveChanged -= OnWave;
        BaseController.OnHPChanged -= OnHP;
    }

    private AudioSource CreateLayer(string name, bool loop)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform);
        AudioSource src = go.AddComponent<AudioSource>();
        src.loop = loop;
        src.playOnAwake = false;
        src.outputAudioMixerGroup = musicGroup;
        src.volume = 0f;
        return src;
    }

    private void OnPhase(GameManager.GameState state)
    {
        switch (state)
        {
            case GameManager.GameState.Menu:         SwitchTo(MusicTrack.Menu); break;
            case GameManager.GameState.Preparation:  SwitchTo(MusicTrack.Preparation); break;
            case GameManager.GameState.Defense:      SwitchTo(MusicTrack.DefenseLight); break;
            case GameManager.GameState.GameOver:     break;
        }
    }

    private void OnWave(int wave)
    {
        PlayJingle(Jingle.WaveStart);
    }

    private void OnHP(int currentHP, int maxHP)
    {
        _maxBaseHP = Mathf.Max(1, maxHP);
        float ratio = (float)currentHP / _maxBaseHP;

        if (currentHP <= 0)
        {
            PlayJingle(Jingle.Defeat);
            return;
        }

        if (_currentTrack == MusicTrack.DefenseLight && ratio < lowHpRatio)
        {
            SwitchTo(MusicTrack.DefenseIntense);
            PlayJingle(Jingle.BaseLowHP);
        }
        else if (_currentTrack == MusicTrack.DefenseIntense && ratio > lowHpRatio + 0.15f)
        {
            SwitchTo(MusicTrack.DefenseLight);
        }
    }

    public void SwitchTo(MusicTrack track)
    {
        if (track == _currentTrack) return;
        MusicState s = FindState(track);
        if (s == null) return;

        _currentTrack = track;

        AudioSource nextDrums   = _usingA ? _drumsB   : _drumsA;
        AudioSource nextMelody  = _usingA ? _melodyB  : _melodyA;
        AudioSource nextStrings = _usingA ? _stringsB : _stringsA;
        AudioSource prevDrums   = _usingA ? _drumsA   : _drumsB;
        AudioSource prevMelody  = _usingA ? _melodyA  : _melodyB;
        AudioSource prevStrings = _usingA ? _stringsA : _stringsB;
        _usingA = !_usingA;

        double dspNow = AudioSettings.dspTime;
        double spb = SecondsPerBeat;
        double nextBar = spb * beatsPerBar;
        double elapsed = dspNow - _trackStartDspTime;
        double remainder = elapsed % nextBar;
        double startAt = dspNow + (nextBar - remainder);
        if (startAt - dspNow > nextBar) startAt = dspNow;
        _trackStartDspTime = startAt;

        ScheduleLayer(nextDrums, s.drums, startAt);
        ScheduleLayer(nextMelody, s.melody, startAt);
        ScheduleLayer(nextStrings, s.strings, startAt);

        float fade = (float)(nextBar);
        StartCoroutine(FadeLayer(nextDrums, s.drumsVolume, fade));
        StartCoroutine(FadeLayer(nextMelody, s.melodyVolume, fade));
        StartCoroutine(FadeLayer(nextStrings, s.stringsVolume, fade));
        StartCoroutine(FadeLayer(prevDrums, 0f, fade));
        StartCoroutine(FadeLayer(prevMelody, 0f, fade));
        StartCoroutine(FadeLayer(prevStrings, 0f, fade));
    }

    private void ScheduleLayer(AudioSource src, AudioClip clip, double dspStart)
    {
        if (src == null) return;
        if (clip == null) { src.Stop(); src.clip = null; return; }
        src.clip = clip;
        src.volume = 0f;
        src.PlayScheduled(dspStart);
    }

    private IEnumerator FadeLayer(AudioSource src, float target, float duration)
    {
        if (src == null) yield break;
        float start = src.volume;
        float t = 0f;
        while (t < duration)
        {
            if (src == null) yield break;
            t += Time.unscaledDeltaTime;
            src.volume = Mathf.Lerp(start, target, Easing.Evaluate(Easing.Ease.EaseInOutSine, t / duration));
            yield return null;
        }
        if (src != null)
        {
            src.volume = target;
            if (target <= 0.001f) src.Stop();
        }
    }

    public void PlayJingle(Jingle id)
    {
        if (_jingleSource == null) return;
        JingleEntry e = FindJingle(id);
        if (e == null || e.clip == null) return;
        _jingleSource.volume = e.volume;
        _jingleSource.PlayOneShot(e.clip, e.volume);
    }

    private MusicState FindState(MusicTrack track)
    {
        if (states == null) return null;
        foreach (MusicState s in states)
            if (s != null && s.track == track) return s;
        return null;
    }

    private JingleEntry FindJingle(Jingle id)
    {
        if (jingles == null) return null;
        foreach (JingleEntry j in jingles)
            if (j != null && j.id == id) return j;
        return null;
    }
}
