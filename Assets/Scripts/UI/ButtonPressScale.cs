using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Уменьшает масштаб кнопки при нажатии и возвращает при отпускании или уходе курсора/пальца.
/// Повесьте на тот же объект, что и <see cref="UnityEngine.UI.Button"/> (нужен Graphic с Raycast Target).
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class ButtonPressScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField] [Range(0.5f, 1f)] private float _pressedScale = 0.92f;

    private RectTransform _rect;
    private Vector3 _baseScale;
    private bool _pressed;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _baseScale = _rect.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
        _rect.localScale = _baseScale * _pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData) => Release();

    public void OnPointerExit(PointerEventData eventData) => Release();

    private void Release()
    {
        if (!_pressed) return;
        _pressed = false;
        _rect.localScale = _baseScale;
    }

    private void OnDisable()
    {
        _pressed = false;
        if (_rect != null)
            _rect.localScale = _baseScale;
    }
}
