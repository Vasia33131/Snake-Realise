using UnityEngine;
using TMPro;

public class SnakeKeyboardInputHandler : MonoBehaviour
{
    [SerializeField] private KeyCode _leftKeyCode;
    [SerializeField] private KeyCode _rightKeyCode;
    [SerializeField] private KeyCode _upKeyCode;
    [SerializeField] private KeyCode _downKeyCode;
    [SerializeField] private TextMeshProUGUI timerText;

    private IObjectMover _objectMover;
    private float _gameTime;
    private int _lastDisplayedSeconds = int.MinValue;

    private void Start()
    {
        _objectMover = GetComponent<IObjectMover>();
        if (_objectMover == null)
            Debug.LogError("Вы забыли добавить компонент ObjectMover! Код не стабилен!");
        else
            _objectMover.MoveForward();
    }

    private void Update()
    {
        if (_objectMover == null) return;

        _gameTime += Time.deltaTime;

        if (timerText != null)
        {
            int secs = Mathf.FloorToInt(_gameTime);
            if (secs != _lastDisplayedSeconds)
            {
                _lastDisplayedSeconds = secs;
                timerText.SetText("Таймер: {0}", secs);
            }
        }

        if (Input.GetKeyDown(_upKeyCode))
            _objectMover.Rotate(Quaternion.Euler(0f, 0f, 90f));
        else if (Input.GetKeyDown(_downKeyCode))
            _objectMover.Rotate(Quaternion.Euler(0f, 0f, -90f));
        else if (Input.GetKeyDown(_rightKeyCode))
            _objectMover.Rotate(Quaternion.Euler(0f, 0f, 0f));
        else if (Input.GetKeyDown(_leftKeyCode))
            _objectMover.Rotate(Quaternion.Euler(0f, 0f, 180f));
    }
}
