using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public sealed class ActiveBoostIcon : MonoBehaviour
{
    [Header("Размер строки (Layout Group часто сжимает детей до нуля)")]
    [SerializeField] private Vector2 _rowSize = new Vector2(96f, 96f);
    [SerializeField] [Range(0.2f, 1f)] private float _iconFill = 0.72f;

    [SerializeField] private Image _iconImage;
    [SerializeField] private float _blinkingStartTime = 1f;

    private CanvasGroup _canvasGroup;
    private BoostType _type;
    private bool _hasType;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        ApplyLayout();
    }

    public void AssignType(BoostType type, Sprite sprite)
    {
        _type = type;
        _hasType = true;

        ApplyLayout();

        if (_iconImage != null && sprite != null)
            _iconImage.sprite = sprite;

        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;
    }

    private void ApplyLayout()
    {
        var rt = transform as RectTransform;
        if (rt != null)
            rt.sizeDelta = _rowSize;

        LayoutElement le = GetComponent<LayoutElement>();
        if (le == null)
            le = gameObject.AddComponent<LayoutElement>();
        le.minWidth = _rowSize.x;
        le.minHeight = _rowSize.y;
        le.preferredWidth = _rowSize.x;
        le.preferredHeight = _rowSize.y;
        le.flexibleWidth = 0f;
        le.flexibleHeight = 0f;

        if (_iconImage != null)
        {
            RectTransform irt = _iconImage.rectTransform;
            float side = Mathf.Min(_rowSize.x, _rowSize.y) * _iconFill;
            irt.sizeDelta = new Vector2(side, side);
        }
    }

    private void LateUpdate()
    {
        if (!_hasType || BoostManager.Instance == null)
            return;

        float remaining = BoostManager.Instance.GetRemaining(_type);
        if (remaining <= 0f)
        {
            FinishAndHide();
            return;
        }

        if (_canvasGroup != null)
        {
            if (remaining <= _blinkingStartTime)
                _canvasGroup.alpha = Mathf.Sin(Time.time * 30f) > 0f ? 1f : 0f;
            else
                _canvasGroup.alpha = 1f;
        }
    }

    public void ForceHide()
    {
        FinishAndHide();
    }

    private void FinishAndHide()
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = 1f;

        _hasType = false;
        gameObject.SetActive(false);
    }
}
