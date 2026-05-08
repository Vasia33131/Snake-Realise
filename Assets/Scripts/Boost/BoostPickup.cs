using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BoostPickup : MonoBehaviour
{
    [Header("Внешний вид на поле")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _speedSprite;
    [SerializeField] private Sprite _shieldSprite;
    [SerializeField] private Sprite _doubleScoreSprite;

    [SerializeField] private BoostType boostType;
    [SerializeField] [Min(0f)] private float _lifetimeOnField;

    private BoostSpawner _spawner;

    public BoostType Type => boostType;

    public void Configure(BoostType pickedType, BoostSpawner spawner, Sprite worldSpriteFromSpawner = null)
    {
        boostType = pickedType;
        _spawner = spawner;
        ApplyWorldVisual(worldSpriteFromSpawner);
    }

    /// <summary>Вызывается спавнером при очистке сцены, чтобы не дергать повторный спавн.</summary>
    public void ReleaseSpawner()
    {
        _spawner = null;
    }

    private void Start()
    {
        if (_lifetimeOnField > 0f)
            Destroy(gameObject, _lifetimeOnField);
    }

    private void OnDestroy()
    {
        if (_spawner != null)
            _spawner.NotifyBoostDestroyed(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsSnakeContact(other))
            return;

        if (BoostManager.Instance != null)
            BoostManager.Instance.ActivateBoost(boostType);

        Destroy(gameObject);
    }

    private void ApplyWorldVisual(Sprite fromSpawner)
    {
        SpriteRenderer sr = _spriteRenderer;
        if (sr == null)
            sr = GetComponent<SpriteRenderer>() ?? GetComponentInChildren<SpriteRenderer>();
        if (sr == null)
            return;

        Sprite s = fromSpawner;
        if (s == null)
        {
            s = boostType switch
            {
                BoostType.Speed => _speedSprite,
                BoostType.Shield => _shieldSprite,
                BoostType.DoubleScore => _doubleScoreSprite,
                _ => null
            };
        }

        if (s != null)
        {
            sr.sprite = s;
            sr.color = Color.white;
        }
        else
        {
            sr.color = boostType switch
            {
                BoostType.Speed => new Color(1f, 0.85f, 0.2f),
                BoostType.Shield => new Color(0.35f, 0.65f, 1f),
                BoostType.DoubleScore => new Color(0.35f, 1f, 0.45f),
                _ => Color.white
            };
        }
    }

    private static bool IsSnakeContact(Collider2D other)
    {
        if (other == null)
            return false;

        if (other.CompareTag("Player"))
            return true;

        return other.GetComponentInParent<IncrementObjectMover>() != null;
    }
}
