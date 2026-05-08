using UnityEngine;

public class BoostSpawner : MonoBehaviour
{
    [SerializeField] private BoostPickup _boostPrefab;
    [Header("Спрайты подбираемого буста (если пусто — берутся с префаба BoostPickup)")]
    [SerializeField] private Sprite _pickupSpeedSprite;
    [SerializeField] private Sprite _pickupShieldSprite;
    [SerializeField] private Sprite _pickupDoubleScoreSprite;
    [SerializeField] private Transform _spawnZoneBottomLeft;
    [SerializeField] private Transform _spawnZoneTopRight;
    [SerializeField] [Min(0.1f)] private float spawnInterval = 10f;
    [SerializeField] private Camera _camera;

    private BoostPickup _activeInstance;
    private float _nextSpawnTime;
    private static readonly BoostType[] Types = { BoostType.Speed, BoostType.Shield, BoostType.DoubleScore };

    private void Awake()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    private void Start()
    {
        // Следующий спавн не раньше чем через spawnInterval (и после подбора — тоже ждём интервал).
        _nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (_activeInstance != null)
            return;

        if (Time.time < _nextSpawnTime)
            return;

        TrySpawn();
        if (_activeInstance == null)
            _nextSpawnTime = Time.time + spawnInterval;
    }

    public void NotifyBoostDestroyed(BoostPickup instance)
    {
        if (!Application.isPlaying)
            return;

        if (instance != null && _activeInstance == instance)
            _activeInstance = null;

        if (_activeInstance == null)
            _nextSpawnTime = Time.time + spawnInterval;
    }

    private void TrySpawn()
    {
        if (_boostPrefab == null || _spawnZoneBottomLeft == null || _spawnZoneTopRight == null)
            return;

        if (_activeInstance != null)
            return;

        BoostType type = Types[Random.Range(0, Types.Length)];
        BoostPickup pickup = Instantiate(_boostPrefab, transform.position, Quaternion.identity);

        if (!FieldItemRandomPlacer.TryRelocate(
                pickup.transform,
                _spawnZoneBottomLeft,
                _spawnZoneTopRight,
                _camera))
        {
            Destroy(pickup.gameObject);
            return;
        }

        Sprite worldSprite = type switch
        {
            BoostType.Speed => _pickupSpeedSprite,
            BoostType.Shield => _pickupShieldSprite,
            BoostType.DoubleScore => _pickupDoubleScoreSprite,
            _ => null
        };

        pickup.Configure(type, this, worldSprite);
        _activeInstance = pickup;
    }

    private void OnDestroy()
    {
        if (_activeInstance == null)
            return;

        BoostPickup held = _activeInstance;
        _activeInstance = null;
        held.ReleaseSpawner();
        Destroy(held.gameObject);
    }
}
