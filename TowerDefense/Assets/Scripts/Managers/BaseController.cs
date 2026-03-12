using UnityEngine;
using System;

public class BaseController : MonoBehaviour
{
    public static event Action<int, int> OnHPChanged;

    [SerializeField] private int maxHP = 20;

    public int CurrentHP { get; private set; }
    public int MaxHP => maxHP;
    public float HPRatio => (float)CurrentHP / maxHP;

    void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - damage);
        OnHPChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP <= 0)
            GameManager.Instance?.TriggerGameOver(false);
    }

    public void Heal(int amount)
    {
        CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
        OnHPChanged?.Invoke(CurrentHP, maxHP);
    }
}
