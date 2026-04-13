using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get; private set; }

    [SerializeField] private MapBlueprint blueprint;
    [SerializeField] private Transform spawnRoot;
    [SerializeField] private bool generateOnAwake = true;
    [SerializeField] private int overrideSeed = 0;
    [SerializeField] private bool useOverrideSeed = false;
    [SerializeField] private float spawnAnimDuration = 0.4f;
    [SerializeField] private float spawnAnimStagger = 0.015f;

    public int LastSeed { get; private set; }

    private readonly List<GameObject> _spawned = new List<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (generateOnAwake) Generate();
    }

    public void Generate()
    {
        if (blueprint == null)
        {
            Debug.LogWarning("[MapGenerator] No blueprint set — skipping generation.");
            return;
        }

        ClearSpawned();

        int seed;
        if (useOverrideSeed) seed = overrideSeed;
        else if (blueprint.useRandomSeed) seed = Random.Range(int.MinValue, int.MaxValue);
        else seed = blueprint.presetSeed;
        LastSeed = seed;

        System.Random rng = new System.Random(seed);
        GridManager gm = GridManager.Instance;
        if (gm == null)
        {
            Debug.LogError("[MapGenerator] GridManager missing.");
            return;
        }

        int midY = blueprint.height / 2;
        int halfPath = Mathf.Max(0, blueprint.safePathHeight / 2);

        for (int x = 0; x < blueprint.width; x++)
        {
            for (int y = 0; y < blueprint.height; y++)
            {
                if (x < blueprint.obstacleMinX) continue;

                bool inSafeCorridor = Mathf.Abs(y - midY) <= halfPath;
                if (inSafeCorridor) continue;

                double roll = rng.NextDouble();
                BlockVariantSet set = PickSet(roll);
                if (set == null) continue;

                GameObject prefab = set.PickRandom(rng);
                if (prefab == null) continue;

                Vector2 worldPos = gm.CellCenter(x, y);
                GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity, spawnRoot != null ? spawnRoot : transform);
                _spawned.Add(instance);

                Vector3 targetScale = instance.transform.localScale;
                instance.transform.localScale = Vector3.zero;
                float delay = (x + y) * spawnAnimStagger;
                StartCoroutine(SpawnAnim(instance.transform, targetScale, delay));
            }
        }

        StartCoroutine(RefreshGridNextFrame());
    }

    private BlockVariantSet PickSet(double roll)
    {
        float treeT = blueprint.treeDensity;
        float bushT = treeT + blueprint.bushDensity;
        float rockT = bushT + blueprint.rockDensity;

        if (roll < treeT) return blueprint.trees;
        if (roll < bushT) return blueprint.bushes;
        if (roll < rockT) return blueprint.rocks;
        return null;
    }

    private IEnumerator SpawnAnim(Transform t, Vector3 target, float delay)
    {
        float elapsed = 0f;
        while (elapsed < delay)
        {
            if (t == null) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < spawnAnimDuration)
        {
            if (t == null) yield break;
            elapsed += Time.deltaTime;
            float n = Easing.Evaluate(Easing.Ease.EaseOutBack, elapsed / spawnAnimDuration);
            t.localScale = Vector3.LerpUnclamped(Vector3.zero, target, n);
            yield return null;
        }
        if (t != null) t.localScale = target;
    }

    private IEnumerator RefreshGridNextFrame()
    {
        yield return null;
        GridManager.Instance?.UpdateGrid();
    }

    private void ClearSpawned()
    {
        foreach (GameObject go in _spawned)
            if (go != null) Destroy(go);
        _spawned.Clear();
    }
}
