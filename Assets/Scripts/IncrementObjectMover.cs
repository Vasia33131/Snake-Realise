using System.Collections;
using UnityEngine;

public class IncrementObjectMover : MonoBehaviour, IObjectMover, ICaudateObject
{
    [SerializeField] private float _delay = 0.05f;
    [SerializeField] private RecursivePositionRepeater _tale;
    [SerializeField] private Camera mainCamera;

    private Quaternion _lastRotation;
    private Coroutine _moveRoutine;
    private SpriteRenderer _renderer;
    private Vector2 _screenBounds;

    public float speed => _renderer.bounds.size.x / _delay;
    public IPositionRepeater tale
    {
        get => _tale;
        set
        {
            if ((IPositionRepeater)_tale != value && value is RecursivePositionRepeater)
            {
                var temp = _tale;
                _tale = (RecursivePositionRepeater)value;
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

        _screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
    }

    public void MoveForward()
    {
        if (_renderer == null) return;
        Stop();
        _moveRoutine = StartCoroutine(MoveRoutine());
    }

    public void Stop()
    {
        if (_renderer == null) return;
        if (_moveRoutine != null) StopCoroutine(_moveRoutine);
    }

    public void Rotate(Quaternion quaternion)
    {
        if (Quaternion.Angle(_lastRotation, quaternion) == 180) return;

        if (_renderer == null) return;

        transform.rotation = quaternion;
        _lastRotation = quaternion;
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            var lastPosition = transform.position;
            Vector3 newPosition = transform.position + transform.right * _renderer.bounds.size.x;

            // Телепортация при выходе за границы
            if (newPosition.x > _screenBounds.x)
                newPosition.x = -_screenBounds.x + _renderer.bounds.size.x;
            else if (newPosition.x < -_screenBounds.x)
                newPosition.x = _screenBounds.x - _renderer.bounds.size.x;

            if (newPosition.y > _screenBounds.y)
                newPosition.y = -_screenBounds.y + _renderer.bounds.size.y;
            else if (newPosition.y < -_screenBounds.y)
                newPosition.y = _screenBounds.y - _renderer.bounds.size.y;

            transform.position = newPosition;

            if (_tale != null)
                _tale.SetPosition(lastPosition);

            yield return new WaitForSecondsRealtime(_delay);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        // Проверяем наличие тега безопасным способом
        if (collision.CompareTag("Wall"))
        {
            TeleportToOppositeSide();
        }
        else if (collision.CompareTag("Tail"))
        {
            // Дополнительная проверка, чтобы голова не реагировала на первый сегмент хвоста
            if (collision.gameObject != _tale?.gameObject)
            {
                GameManager.Instance.GameOver();
            }
        }
    }

    private void TeleportToOppositeSide()
    {
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        Vector3 newPos = Vector3.zero;

        if (viewportPos.x < 0 || viewportPos.x > 1)
        {
            newPos = mainCamera.ViewportToWorldPoint(new Vector3(1 - viewportPos.x, viewportPos.y, 0));
            newPos.z = transform.position.z;
        }
        else if (viewportPos.y < 0 || viewportPos.y > 1)
        {
            newPos = mainCamera.ViewportToWorldPoint(new Vector3(viewportPos.x, 1 - viewportPos.y, 0));
            newPos.z = transform.position.z;
        }

        transform.position = newPos;
    }
}