using UnityEngine;

/// <summary>
/// Affiche un quadrillage léger sur la map en traçant les lignes de la grille.
/// Génère un LineRenderer par ligne (horizontale + verticale).
///
/// Attacher sur n'importe quel GameObject de la scène Game.
/// Dépend de GridManager (doit être présent dans la scène).
/// </summary>
public class GridOverlay : MonoBehaviour
{
    [Header("Apparence")]
    [SerializeField] private Sprite spriteCase;
    [SerializeField] private Color couleur      = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private int   sortingOrder = 1;   // au-dessus du fond, sous les entités

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    void Start()
    {
        if (GridManager.Instance == null)
        {
            Debug.LogWarning("[GridOverlay] GridManager introuvable.");
            return;
        }

        if (spriteCase == null)
        {
            Debug.LogWarning("[GridOverlay] Aucun sprite assigné.");
            return;
        }

        GenererGrille();
    }

    // ── Génération ────────────────────────────────────────────────────────────
    private void GenererGrille()
    {
        GridManager gm = GridManager.Instance;

        // Créer un sprite dans chaque case
        for (int x = 0; x < gm.Largeur; x++)
        {
            for (int y = 0; y < gm.Hauteur; y++)
            {
                Node node = gm.ObtenirNoeud(x, y);
                if (node == null) continue;

                CreerSpriteCase(node.worldPosition);
            }
        }
    }

    private void CreerSpriteCase(Vector3 position)
    {
        GameObject go = new GameObject("GridSprite");
        go.transform.SetParent(transform);
        go.transform.position = position;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingLayerName = "Grid Overlay";
        sr.sprite = spriteCase;
        sr.color = couleur;
        sr.sortingOrder = sortingOrder;
    }
}
