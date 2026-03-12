using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Rush, Tank, Flanker }

    [Header("Type")]
    [SerializeField] public EnemyType type = EnemyType.Rush;

    [Header("Base Stats")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private int maxHP = 3;
    [SerializeField] private int baseDamage = 1;

    [Header("Navigation")]
    [SerializeField] private float waypointTolerance = 0.15f;

    public static event Action OnEnemyDied;

    private int _currentHP;
    private int _goldReward;
    private List<Vector2> _path;
    private int _waypointIndex;
    private Transform _baseTarget;
    private bool _arrived;

    public int WaypointIndex => _waypointIndex;

    void Awake()
    {
        ConfigureByType();
        _currentHP = maxHP;
    }

    void Start()
    {
        GameObject baseObj = GameObject.FindWithTag("Base");
        if (baseObj != null)
            _baseTarget = baseObj.transform;
        else
            Debug.LogWarning("[EnemyAI] No GameObject tagged 'Base' found!");

        RecalculatePath();
        GridManager.OnGridUpdated += RecalculatePath;
    }

    void OnDestroy()
    {
        GridManager.OnGridUpdated -= RecalculatePath;
    }

    void Update()
    {
        if (_arrived) return;
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Defense) return;
        FollowPath();
    }

    private void ConfigureByType()
    {
        switch (type)
        {
            case EnemyType.Rush:
                speed = 3.5f;
                maxHP = 2;
                baseDamage = 1;
                _goldReward = 10;
                break;
            case EnemyType.Tank:
                speed = 1f;
                maxHP = 10;
                baseDamage = 3;
                _goldReward = 25;
                break;
            case EnemyType.Flanker:
                speed = 2.5f;
                maxHP = 4;
                baseDamage = 1;
                _goldReward = 15;
                break;
        }
    }

    public void RecalculatePath()
    {
        if (_baseTarget == null || AStarPathfinder.Instance == null) return;

        int penalty = type == EnemyType.Flanker ? 50 : 0;

        List<Vector2> newPath = AStarPathfinder.Instance.FindPath(
            transform.position,
            _baseTarget.position,
            penalty
        );

        if (newPath != null)
        {
            _path = newPath;
            _waypointIndex = 0;
        }
        else
        {
            Debug.LogWarning($"[EnemyAI] {gameObject.name}: path blocked!");
        }
    }

    private void FollowPath()
    {
        if (_path == null || _waypointIndex >= _path.Count) return;

        Vector2 destination = _path[_waypointIndex];
        transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, destination) < waypointTolerance)
            _waypointIndex++;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Base"))
        {
            _arrived = true;
            other.GetComponent<BaseController>()?.TakeDamage(baseDamage);
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;
        if (_currentHP <= 0)
            Die();
    }

    private void Die()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.Add(1, _goldReward);
            ResourceManager.Instance.Add(2, _goldReward);
        }

        OnEnemyDied?.Invoke();
        Destroy(gameObject);
    }
}
