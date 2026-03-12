using UnityEngine;

/// <summary>
/// Affiche un quadrillage sur la map via des LineRenderers.
/// 21 lignes verticales + 11 lignes horizontales = 32 objets au lieu de 200 sprites.
///
/// Attacher sur n'importe quel GameObject de la scène Game.
/// Dépend de GridManager (doit être présent dans la scène).
/// </summary>
public class GridOverlay : MonoBehaviour
{
    [Header("Apparence")]
    [SerializeField] private Color couleur      = new Color(1f, 1f, 1f, 0.15f);
    [SerializeField] private float epaisseur    = 0.03f;
    [SerializeField] private int   sortingOrder = 1;

    void Start()
    {
        if (GridManager.Instance == null)
        {
            Debug.LogWarning("[GridOverlay] GridManager introuvable.");
            return;
        }
        GenererGrille();
    }

    private void GenererGrille()
    {
        GridManager gm     = GridManager.Instance;
        float       taille = gm.TailleCellule;
        float       demi   = taille * 0.5f;

        Node coin = gm.ObtenirNoeud(0, 0);
        if (coin == null) return;

        // Coin bas-gauche de la grille
        Vector2 origine = coin.worldPosition - new Vector2(demi, demi);
        float   largeur = gm.Largeur * taille;
        float   hauteur = gm.Hauteur * taille;

        // Lignes verticales (Largeur + 1)
        for (int x = 0; x <= gm.Largeur; x++)
        {
            float px = origine.x + x * taille;
            CréerLigne(
                new Vector3(px, origine.y, 0f),
                new Vector3(px, origine.y + hauteur, 0f),
                $"VLine_{x}"
            );
        }

        // Lignes horizontales (Hauteur + 1)
        for (int y = 0; y <= gm.Hauteur; y++)
        {
            float py = origine.y + y * taille;
            CréerLigne(
                new Vector3(origine.x, py, 0f),
                new Vector3(origine.x + largeur, py, 0f),
                $"HLine_{y}"
            );
        }
    }

    private void CréerLigne(Vector3 debut, Vector3 fin, string nom)
    {
        GameObject go = new GameObject(nom);
        go.transform.SetParent(transform);

        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.positionCount    = 2;
        lr.SetPosition(0, debut);
        lr.SetPosition(1, fin);

        lr.startColor         = couleur;
        lr.endColor           = couleur;
        lr.startWidth         = epaisseur;
        lr.endWidth           = epaisseur;
        lr.useWorldSpace      = true;
        lr.sortingLayerName   = "Grid Overlay";
        lr.sortingOrder       = sortingOrder;

        // Matériau sans texture — couleur unie
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.material.color = couleur;
    }
}
