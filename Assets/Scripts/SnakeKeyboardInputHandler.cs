using UnityEngine;
using TMPro;

public class SnakeKeyboardInputHandler : MonoBehaviour
{
    [SerializeField] private KeyCode _leftKeyCode ;
    [SerializeField] private KeyCode _rightKeyCode ;
    [SerializeField] private KeyCode _upKeyCode ;
    [SerializeField] private KeyCode _downKeyCode ;
    [SerializeField] private TextMeshProUGUI timerText;

    private IObjectMover _objectMover;
    private float _gameTime = 0f;

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
        timerText.text = "Таймер: " + Mathf.FloorToInt(_gameTime).ToString();

        if (Input.GetKeyDown(_upKeyCode))
            _objectMover.Rotate(Quaternion.Euler(0, 0, 90));
        else if (Input.GetKeyDown(_downKeyCode))
            _objectMover.Rotate(Quaternion.Euler(0, 0, -90));
        else if (Input.GetKeyDown(_rightKeyCode))
            _objectMover.Rotate(Quaternion.Euler(0, 0, 0));
        else if (Input.GetKeyDown(_leftKeyCode))
            _objectMover.Rotate(Quaternion.Euler(0, 0, 180));
    }
}