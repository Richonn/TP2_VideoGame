using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Algorithme A* maison pour la navigation sur grille 2D.
///
/// Implémentation :
///   - Mouvement 8-directionnel (orthogonal coût 10, diagonal coût 14)
///   - Heuristique octile (cohérente avec le mouvement diagonal)
///   - Retourne null si aucun chemin n'existe (chemin bloqué)
///
/// Usage : AStarPathfinder.Instance.TrouverChemin(depart, arrivee)
/// </summary>
public class AStarPathfinder : MonoBehaviour
{
    // ── Singleton ─────────────────────────────────────────────────────────────
    public static AStarPathfinder Instance { get; private set; }

    private const int COUT_ORTHO     = 10;
    private const int COUT_DIAGONAL  = 14;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── API publique ──────────────────────────────────────────────────────────

    /// <summary>
    /// Calcule le chemin le plus court entre deux positions monde.
    /// </summary>
    /// <returns>
    /// Liste ordonnée de positions monde à suivre,
    /// ou null si le chemin est impossible.
    /// </returns>
    public List<Vector2> TrouverChemin(Vector2 depart, Vector2 arrivee)
    {
        GridManager grille = GridManager.Instance;
        if (grille == null) return null;

        Node noeudDepart  = grille.MondeVersNoeud(depart);
        Node noeudArrivee = grille.MondeVersNoeud(arrivee);

        if (noeudDepart == null || noeudArrivee == null) return null;
        if (!noeudArrivee.walkable) return null;

        // Réinitialiser les coûts avant la recherche
        grille.ReinitialisationCouts();

        List<Node>    openSet   = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        noeudDepart.gCost = 0;
        noeudDepart.hCost = Heuristique(noeudDepart, noeudArrivee);
        openSet.Add(noeudDepart);

        while (openSet.Count > 0)
        {
            // Sélectionner le noeud avec le fCost minimal
            Node courant = MeilleurNoeud(openSet);

            if (courant == noeudArrivee)
                return ReconstruireChemin(noeudArrivee);

            openSet.Remove(courant);
            closedSet.Add(courant);

            foreach (Node voisin in grille.ObtenirVoisins(courant))
            {
                if (!voisin.walkable || closedSet.Contains(voisin)) continue;

                bool diagonal = voisin.gridX != courant.gridX && voisin.gridY != courant.gridY;
                int coutMouvement = diagonal ? COUT_DIAGONAL : COUT_ORTHO;
                int nouveauGCost  = courant.gCost + coutMouvement;

                if (nouveauGCost < voisin.gCost)
                {
                    voisin.gCost  = nouveauGCost;
                    voisin.hCost  = Heuristique(voisin, noeudArrivee);
                    voisin.parent = courant;

                    if (!openSet.Contains(voisin))
                        openSet.Add(voisin);
                }
            }
        }

        // Aucun chemin trouvé
        return null;
    }

    // ── Fonctions internes ────────────────────────────────────────────────────

    /// <summary>Retourne le noeud avec le fCost le plus bas dans la liste ouverte.</summary>
    private Node MeilleurNoeud(List<Node> openSet)
    {
        Node meilleur = openSet[0];
        for (int i = 1; i < openSet.Count; i++)
        {
            if (openSet[i].fCost < meilleur.fCost ||
               (openSet[i].fCost == meilleur.fCost && openSet[i].hCost < meilleur.hCost))
            {
                meilleur = openSet[i];
            }
        }
        return meilleur;
    }

    /// <summary>
    /// Heuristique octile — cohérente avec le mouvement 8-directionnel.
    /// Formule : 14 * min(dx,dy) + 10 * |dx - dy|
    /// </summary>
    private int Heuristique(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);
        return COUT_DIAGONAL * Mathf.Min(dx, dy) + COUT_ORTHO * Mathf.Abs(dx - dy);
    }

    /// <summary>Remonte la chaîne de parents pour reconstruire le chemin.</summary>
    private List<Vector2> ReconstruireChemin(Node arrivee)
    {
        List<Vector2> chemin = new List<Vector2>();
        Node courant = arrivee;

        while (courant != null)
        {
            chemin.Add(courant.worldPosition);
            courant = courant.parent;
        }

        chemin.Reverse();
        return chemin;
    }
}
