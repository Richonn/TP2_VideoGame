using UnityEngine;

[CreateAssetMenu(fileName = "BlockVariantSet", menuName = "TowerDefense/Procedural/Block Variant Set")]
public class BlockVariantSet : ScriptableObject
{
    public GameObject[] variants;

    public GameObject PickRandom(System.Random rng)
    {
        if (variants == null || variants.Length == 0) return null;
        return variants[rng.Next(0, variants.Length)];
    }
}
