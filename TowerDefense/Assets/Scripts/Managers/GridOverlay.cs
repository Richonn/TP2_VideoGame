using UnityEngine;

public class GridOverlay : MonoBehaviour
{
    [Header("Appearance")]
    [SerializeField] private Color color = new Color(1f, 1f, 1f, 0.15f);
    [SerializeField] private float thickness = 0.03f;
    [SerializeField] private int sortingOrder = 1;

    void Start()
    {
        if (GridManager.Instance == null) return;
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        GridManager gm = GridManager.Instance;
        float size = gm.CellSize;
        float half = size * 0.5f;

        Node corner = gm.GetNode(0, 0);
        if (corner == null) return;

        Vector2 origin = corner.worldPosition - new Vector2(half, half);
        float gridWidth = gm.Width * size;
        float gridHeight = gm.Height * size;

        for (int x = 0; x <= gm.Width; x++)
        {
            float px = origin.x + x * size;
            CreateLine(new Vector3(px, origin.y, 0f), new Vector3(px, origin.y + gridHeight, 0f), $"VLine_{x}");
        }

        for (int y = 0; y <= gm.Height; y++)
        {
            float py = origin.y + y * size;
            CreateLine(new Vector3(origin.x, py, 0f), new Vector3(origin.x + gridWidth, py, 0f), $"HLine_{y}");
        }
    }

    private void CreateLine(Vector3 start, Vector3 end, string name)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = thickness;
        lr.endWidth = thickness;
        lr.useWorldSpace = true;
        lr.sortingLayerName = "Grid Overlay";
        lr.sortingOrder = sortingOrder;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.material.color = color;
    }
}
