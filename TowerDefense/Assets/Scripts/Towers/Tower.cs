using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Stats")]
    public int cost = 50;
    public float range = 3f;
    public int damage = 2;
    public float fireRate = 1f;

    [Header("Targeting")]
    [SerializeField] private LayerMask enemyLayer;

    private float _timer;

    void Update()
    {
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Defense) return;

        _timer += Time.deltaTime;
        if (_timer >= 1f / fireRate)
        {
            _timer = 0f;
            ShootAtEnemy();
        }
    }

    private void ShootAtEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        if (enemies.Length == 0) return;

        EnemyAI target = null;
        int maxWaypoint = -1;

        foreach (Collider2D col in enemies)
        {
            EnemyAI ai = col.GetComponent<EnemyAI>();
            if (ai != null && ai.WaypointIndex > maxWaypoint)
            {
                maxWaypoint = ai.WaypointIndex;
                target = ai;
            }
        }

        target?.TakeDamage(damage);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
