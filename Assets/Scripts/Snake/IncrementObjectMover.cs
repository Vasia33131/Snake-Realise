using System.Collections;
using UnityEngine;

public class IncrementObjectMover : MonoBehaviour, IObjectMover, ICaudateObject
{
    [SerializeField] private float _delay = 0.05f;
    [SerializeField] private RecursivePositionRepeater _tale;
    [SerializeField] private Camera mainCamera;

    private float _baseMovePeriod;
    private float _speedMultiplier = 1f;
    private bool _shieldActive;

    private Quaternion _lastRotation;
    private Coroutine _moveRoutine;
    private SpriteRenderer _renderer;
    private Vector2 _screenBounds;
    private float _cellWorldWidth;
    private float _cellWorldHeight;

    public float speed
    {
        get
        {
            if (_renderer == null || _delay <= 0f)
                return 0f;
            return _cellWorldWidth / _delay;
        }
    }

    public IPositionRepeater tale
    {
        get => _tale;
        set
        {
            if ((IPositionRepeater)_tale != value && value is RecursivePositionRepeater newTail)
            {
                var temp = _tale;
                _tale = newTail;
                _tale.SetNextRepeater(temp);
            }
        }
    }

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        if (_renderer == null)
            Debug.LogError("Для корректного использования IncrementObjectMover требуется SpriteRenderer!");

        _lastRotation = transform.rotation;

        if (mainCamera == null)
            mainCamera = Camera.main;

        _baseMovePeriod = _delay;
        ApplySpeedMultiplierToDelay();

        _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        RefreshCellMetrics();
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = Mathf.Max(0.01f, multiplier);
        ApplySpeedMultiplierToDelay();

        if (_renderer != null && _moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
            RefreshCellMetrics();
            _moveRoutine = StartCoroutine(MoveRoutine());
        }
    }

    public void SetShieldActive(bool active)
    {
        _shieldActive = active;
    }

    /// <summary>Точка расширения API; учёт очков — в <see cref="BoostManager"/> и <see cref="GameManager"/>.</summary>
    public void SetScoreMultiplier(float multiplier)
    {
        _ = multiplier;
    }

    private void ApplySpeedMultiplierToDelay()
    {
        _delay = Mathf.Max(0.002f, _baseMovePeriod / _speedMultiplier);
    }

    private void RefreshCellMetrics()
    {
        if (_renderer == null) return;
        Vector3 size = _renderer.bounds.size;
        _cellWorldWidth = size.x;
        _cellWorldHeight = size.y;
    }

    public void MoveForward()
    {
        if (_renderer == null) return;
        Stop();
        RefreshCellMetrics();
        _moveRoutine = StartCoroutine(MoveRoutine());
    }

    public void Stop()
    {
        if (_renderer == null) return;
        if (_moveRoutine != null)
            StopCoroutine(_moveRoutine);
    }

    public void Rotate(Quaternion quaternion)
    {
        if (Quaternion.Angle(_lastRotation, quaternion) == 180f) return;
        if (_renderer == null) return;

        transform.rotation = quaternion;
        _lastRotation = quaternion;
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            Vector3 lastPosition = transform.position;
            Vector3 newPosition = transform.position + transform.right * _cellWorldWidth;

            if (!_shieldActive)
            {
                if (newPosition.x > _screenBounds.x)
                    newPosition.x = -_screenBounds.x + _cellWorldWidth;
                else if (newPosition.x < -_screenBounds.x)
                    newPosition.x = _screenBounds.x - _cellWorldWidth;

                if (newPosition.y > _screenBounds.y)
                    newPosition.y = -_screenBounds.y + _cellWorldHeight;
                else if (newPosition.y < -_screenBounds.y)
                    newPosition.y = _screenBounds.y - _cellWorldHeight;
            }

            transform.position = newPosition;

            if (_tale != null)
                _tale.SetPosition(lastPosition);

            yield return new WaitForSecondsRealtime(_delay);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) => HandleContact(collision);

    private void OnCollisionEnter2D(Collision2D collision) => HandleContact(collision.collider);

    private void HandleContact(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.CompareTag("Wall"))
        {
            if (!_shieldActive)
                TeleportToOppositeSide();
        }
        else if (collision.CompareTag("Tail"))
        {
            if (_shieldActive)
                return;

            if (collision.gameObject != _tale?.gameObject && GameManager.Instance != null)
                GameManager.Instance.GameOver();
        }
        else if (collision.CompareTag("Bomb"))
        {
            if (_shieldActive)
                return;

            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
        }
    }

    private void TeleportToOppositeSide()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        Vector3 newPos = Vector3.zero;

        if (viewportPos.x < 0f || viewportPos.x > 1f)
        {
            newPos = mainCamera.ViewportToWorldPoint(new Vector3(1f - viewportPos.x, viewportPos.y, 0f));
            newPos.z = transform.position.z;
        }
        else if (viewportPos.y < 0f || viewportPos.y > 1f)
        {
            newPos = mainCamera.ViewportToWorldPoint(new Vector3(viewportPos.x, 1f - viewportPos.y, 0f));
            newPos.z = transform.position.z;
        }

        transform.position = newPos;
    }
}
