using UnityEngine;
using System;

/// <summary>
/// Base centrale partagée par les deux joueurs.
///
/// Setup Unity :
///   - Tag : "Base"
///   - Collider2D (Trigger) pour détecter les ennemis
///
/// Les ennemis (EnemyAI) appellent PrendreDegats() à l'arrivée.
/// Quand les PV tombent à 0, déclenche GameOver (défaite).
/// </summary>
public class BaseController : MonoBehaviour
{
    // ── Événement (HUD l'écoute) ──────────────────────────────────────────────
    public static event Action<int, int> OnPVChanges;   // (pvActuels, pvMax)

    // ── Inspector ─────────────────────────────────────────────────────────────
    [SerializeField] private int pvMax = 20;

    // ── État ──────────────────────────────────────────────────────────────────
    public int PVActuels  { get; private set; }
    public int PVMax      => pvMax;
    public float RatioPV  => (float)PVActuels / pvMax;

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    void Awake()
    {
        PVActuels = pvMax;
    }

    // ── Dégâts ────────────────────────────────────────────────────────────────
    public void PrendreDegats(int degats)
    {
        PVActuels = Mathf.Max(0, PVActuels - degats);
        OnPVChanges?.Invoke(PVActuels, pvMax);

        if (PVActuels <= 0)
            GameManager.Instance?.TriggerGameOver(false);
    }

    /// <summary>Soigner la base (utile pour debug ou power-up futur).</summary>
    public void Soigner(int soin)
    {
        PVActuels = Mathf.Min(pvMax, PVActuels + soin);
        OnPVChanges?.Invoke(PVActuels, pvMax);
    }
}
