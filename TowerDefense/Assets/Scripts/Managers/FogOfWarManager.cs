using UnityEngine;

/// <summary>
/// Brouillard de guerre basé sur la position des agents (PAS sur la caméra).
///
/// Fonctionnement :
///   - Couvre la map de tuiles sombres (une par cellule 2×2)
///   - Chaque frame, calcule les zones révélées :
///       • rayon configurable autour de chaque joueur
///       • portée de tir de chaque tour
///       • zone fixe autour de la base centrale
///   - Anime l'alpha des tuiles en fondu progressif
///   - Désactive le SpriteRenderer des ennemis dont la cellule est dans le brouillard
///
/// Attacher sur un GameObject vide dans la scène Game.
/// </summary>
public class FogOfWarManager : MonoBehaviour
{
    public static FogOfWarManager Instance { get; private set; }

    [Header("Apparence")]
    [SerializeField] private Color couleurBrouillard = new Color(0.05f, 0.05f, 0.1f, 0.88f);
    [SerializeField] private float vitesseAnimation  = 4f;   // unités d'alpha par seconde

    [Header("Vision")]
    [SerializeField] private float rayonJoueur = 5f;
    [SerializeField] private float rayonBase   = 3f;

    // ── Ordres de rendu ───────────────────────────────────────────────────────
    private const int SORT_BROUILLARD = 10;  // au-dessus des ennemis/tours
    private const int SORT_JOUEUR     = 15;  // au-dessus du brouillard

    // ── Données internes ──────────────────────────────────────────────────────
    private SpriteRenderer[,] _tuiles;
    private float[,]          _alphasCibles;   // 0 = révélé, 1 = sombre
    private GridManager       _gm;
    private Sprite            _spriteBloc;

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        _gm = GridManager.Instance;
        if (_gm == null)
        {
            Debug.LogWarning("[FogOfWar] GridManager introuvable — brouillard désactivé.");
            enabled = false;
            return;
        }

        _spriteBloc = CréerSpriteCarré();
        CréerTuiles();
        ElevéJoueursAuDessus();
    }

    void Update()
    {
        MettreAJourVision();
        AnimerTuiles();
        CacherEnnemis();
    }

    // ── Création des tuiles ───────────────────────────────────────────────────
    private void CréerTuiles()
    {
        int   larg   = _gm.Largeur;
        int   haut   = _gm.Hauteur;
        float taille = _gm.TailleCellule;

        _tuiles       = new SpriteRenderer[larg, haut];
        _alphasCibles = new float[larg, haut];

        for (int x = 0; x < larg; x++)
        {
            for (int y = 0; y < haut; y++)
            {
                Node noeud = _gm.ObtenirNoeud(x, y);
                if (noeud == null) continue;

                GameObject go = new GameObject($"Fog_{x}_{y}");
                go.transform.SetParent(transform);
                go.transform.position   = new Vector3(noeud.worldPosition.x, noeud.worldPosition.y, 0f);
                go.transform.localScale = new Vector3(taille, taille, 1f);

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sprite       = _spriteBloc;
                sr.color        = couleurBrouillard;
                sr.sortingOrder = SORT_BROUILLARD;

                _tuiles[x, y]       = sr;
                _alphasCibles[x, y] = 1f;   // tout sombre au départ
            }
        }
    }

    /// <summary>Crée un sprite 1×1 px blanc — la couleur vient de SpriteRenderer.color.</summary>
    private static Sprite CréerSpriteCarré()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        // PPU=1 → sprite 1 unité monde, le localScale 2×2 du Transform l'agrandit
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    // ── Mise à jour de la vision ──────────────────────────────────────────────
    private void MettreAJourVision()
    {
        int larg = _gm.Largeur;
        int haut = _gm.Hauteur;

        // Tout mettre à sombre
        for (int x = 0; x < larg; x++)
            for (int y = 0; y < haut; y++)
                _alphasCibles[x, y] = 1f;

        // Révéler autour des joueurs
        foreach (PlayerController joueur in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
            RévélerAutourDe(joueur.transform.position, rayonJoueur);

        // Révéler autour des tours (rayon = portée de tir)
        foreach (Tower tour in FindObjectsByType<Tower>(FindObjectsSortMode.None))
            RévélerAutourDe(tour.transform.position, tour.portee);

        // Révéler autour de la base centrale
        GameObject baseObj = GameObject.FindWithTag("Base");
        if (baseObj != null) RévélerAutourDe(baseObj.transform.position, rayonBase);
    }

    private void RévélerAutourDe(Vector2 centre, float rayon)
    {
        int   larg    = _gm.Largeur;
        int   haut    = _gm.Hauteur;
        float rayonSq = rayon * rayon;

        for (int x = 0; x < larg; x++)
        {
            for (int y = 0; y < haut; y++)
            {
                Node noeud = _gm.ObtenirNoeud(x, y);
                if (noeud == null) continue;

                if ((noeud.worldPosition - centre).sqrMagnitude <= rayonSq)
                    _alphasCibles[x, y] = 0f;
            }
        }
    }

    // ── Animation des tuiles ──────────────────────────────────────────────────
    private void AnimerTuiles()
    {
        int   larg    = _gm.Largeur;
        int   haut    = _gm.Hauteur;
        float step    = vitesseAnimation * Time.deltaTime;
        float alphaMax = couleurBrouillard.a;

        for (int x = 0; x < larg; x++)
        {
            for (int y = 0; y < haut; y++)
            {
                SpriteRenderer sr = _tuiles[x, y];
                if (sr == null) continue;

                Color c = sr.color;
                c.a      = Mathf.MoveTowards(c.a, _alphasCibles[x, y] * alphaMax, step);
                sr.color = c;
            }
        }
    }

    // ── Culling logique des ennemis ───────────────────────────────────────────
    private void CacherEnnemis()
    {
        foreach (EnemyAI ennemi in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            bool révélé = EstCelluleRévélée(ennemi.transform.position);
            SpriteRenderer sr = ennemi.GetComponent<SpriteRenderer>();
            if (sr != null) sr.enabled = révélé;
        }
    }

    /// <summary>Retourne true si la cellule à cette position monde est révélée.</summary>
    public bool EstCelluleRévélée(Vector2 positionMonde)
    {
        Node noeud = _gm.MondeVersNoeud(positionMonde);
        if (noeud == null) return false;
        return _alphasCibles[noeud.gridX, noeud.gridY] < 0.5f;
    }

    // ── Joueurs toujours visibles au-dessus du brouillard ─────────────────────
    private void ElevéJoueursAuDessus()
    {
        foreach (PlayerController joueur in FindObjectsByType<PlayerController>(FindObjectsSortMode.None))
        {
            SpriteRenderer sr = joueur.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sortingOrder = SORT_JOUEUR;
        }
    }
}
