using System.Collections.Generic;
using UnityEngine;

public sealed class BoostManager : MonoBehaviour
{
    public static BoostManager Instance { get; private set; }

    [SerializeField] private BoostUIManager _boostUi;

    private readonly Dictionary<BoostType, float> _remaining = new Dictionary<BoostType, float>();
    private IncrementObjectMover _snake;
    private int _scoreMultiplier = 1;

    public int ScoreMultiplier => _scoreMultiplier;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResolveSnake();
    }

    private void OnDestroy()
    {
        ClearAllEffects();

        if (Instance == this)
            Instance = null;
    }

    public void ActivateBoost(BoostType type, float duration = 5f)
    {
        if (duration <= 0f)
            return;

        _remaining[type] = duration;
        ResolveSnake();
        RecomputeModifiersFromDictionary();
        _boostUi?.RegisterBoost(type);
    }

    public float GetRemaining(BoostType type)
    {
        return _remaining.TryGetValue(type, out float t) ? t : 0f;
    }

    public bool IsBoostActive(BoostType type)
    {
        return _remaining.TryGetValue(type, out float t) && t > 0f;
    }

    private void Update()
    {
        if (_remaining.Count == 0)
            return;

        float dt = Time.deltaTime;
        var keys = new List<BoostType>(_remaining.Keys);
        bool changed = false;

        for (int i = 0; i < keys.Count; i++)
        {
            BoostType key = keys[i];
            if (!_remaining.TryGetValue(key, out float rem))
                continue;

            rem -= dt;
            if (rem <= 0f)
            {
                _remaining.Remove(key);
                changed = true;
                _boostUi?.OnBoostEnded(key);
            }
            else
            {
                _remaining[key] = rem;
            }
        }

        if (changed)
            RecomputeModifiersFromDictionary();
    }

    private void ResolveSnake()
    {
        if (_snake == null)
            _snake = FindObjectOfType<IncrementObjectMover>();
    }

    private void RecomputeModifiersFromDictionary()
    {
        ResolveSnake();

        bool speed = _remaining.TryGetValue(BoostType.Speed, out float rs) && rs > 0f;
        bool shield = _remaining.TryGetValue(BoostType.Shield, out float rsh) && rsh > 0f;
        bool dbl = _remaining.TryGetValue(BoostType.DoubleScore, out float rd) && rd > 0f;

        if (_snake != null)
        {
            _snake.SetSpeedMultiplier(speed ? 2f : 1f);
            _snake.SetShieldActive(shield);
            _snake.SetScoreMultiplier(dbl ? 2f : 1f);
        }

        _scoreMultiplier = dbl ? 2 : 1;
    }

    private void ClearAllEffects()
    {
        _remaining.Clear();
        _scoreMultiplier = 1;

        if (_snake == null)
            _snake = FindObjectOfType<IncrementObjectMover>();

        if (_snake != null)
        {
            _snake.SetSpeedMultiplier(1f);
            _snake.SetShieldActive(false);
            _snake.SetScoreMultiplier(1f);
        }

        _boostUi?.ClearAll();
    }
}
