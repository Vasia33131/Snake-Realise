using System.Collections.Generic;
using UnityEngine;

public sealed class BoostUIManager : MonoBehaviour
{
    [SerializeField] private Transform _activeBoostPanel;
    [SerializeField] private ActiveBoostIcon _iconPrefab;
    [SerializeField] private Sprite _speedIcon;
    [SerializeField] private Sprite _shieldIcon;
    [SerializeField] private Sprite _doubleScoreIcon;

    private readonly Dictionary<BoostType, ActiveBoostIcon> _rows = new Dictionary<BoostType, ActiveBoostIcon>();

    public void RegisterBoost(BoostType type)
    {
        if (_activeBoostPanel == null || _iconPrefab == null)
            return;

        if (!_rows.TryGetValue(type, out ActiveBoostIcon row))
        {
            row = Instantiate(_iconPrefab, _activeBoostPanel);
            _rows[type] = row;
        }

        row.gameObject.SetActive(true);
        row.AssignType(type, ResolveSprite(type));
    }

    public void OnBoostEnded(BoostType type)
    {
        if (_rows.TryGetValue(type, out ActiveBoostIcon row) && row != null)
            row.ForceHide();
    }

    public void ClearAll()
    {
        foreach (KeyValuePair<BoostType, ActiveBoostIcon> kv in _rows)
        {
            if (kv.Value != null)
                kv.Value.ForceHide();
        }
    }

    private Sprite ResolveSprite(BoostType type)
    {
        return type switch
        {
            BoostType.Speed => _speedIcon,
            BoostType.Shield => _shieldIcon,
            BoostType.DoubleScore => _doubleScoreIcon,
            _ => null
        };
    }
}
