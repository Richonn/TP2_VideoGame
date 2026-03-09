using UnityEngine;

/// <summary>
/// Représente une case de la grille de navigation.
/// Utilisé par GridManager et AStarPathfinder.
/// </summary>
public class Node
{
    public bool walkable;
    public Vector2 worldPosition;
    public int gridX, gridY;

    // Coûts A*
    public int gCost;   // coût depuis le départ
    public int hCost;   // heuristique vers l'arrivée
    public int fCost => gCost + hCost;

    public Node parent;

    public Node(bool walkable, Vector2 worldPos, int gridX, int gridY)
    {
        this.walkable      = walkable;
        this.worldPosition = worldPos;
        this.gridX         = gridX;
        this.gridY         = gridY;
    }

    public void Reinitialiser()
    {
        gCost  = int.MaxValue;
        hCost  = 0;
        parent = null;
    }
}
