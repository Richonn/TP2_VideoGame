using UnityEngine;

[CreateAssetMenu(fileName = "MapBlueprint", menuName = "TowerDefense/Procedural/Map Blueprint")]
public class MapBlueprint : ScriptableObject
{
    [Header("Identity")]
    public string displayName = "Default Map";
    public int presetSeed = 12345;
    public bool useRandomSeed = false;

    [Header("Dimensions (cells)")]
    public int width = 20;
    public int height = 10;

    [Header("Tile variants")]
    public BlockVariantSet ground;
    public BlockVariantSet trees;
    public BlockVariantSet bushes;
    public BlockVariantSet rocks;

    [Header("Density (0..1) of obstacle tiles in the right half of the map")]
    [Range(0f, 1f)] public float treeDensity = 0.18f;
    [Range(0f, 1f)] public float bushDensity = 0.08f;
    [Range(0f, 1f)] public float rockDensity = 0.05f;

    [Header("Safe corridor")]
    [Tooltip("Number of central Y rows that stay walkable for the enemy path.")]
    public int safePathHeight = 2;

    [Header("Obstacle zone")]
    [Tooltip("Only spawn obstacles in cells with x >= this index (keeps left half clear for tower placement).")]
    public int obstacleMinX = 10;
}
