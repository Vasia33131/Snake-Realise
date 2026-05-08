using UnityEngine;

public class RecursivePositionRepeater : MonoBehaviour, IRecursivePositionRepeater, ICaudateObject
{
    [SerializeField] private RecursivePositionRepeater _tale;
    private IPositionRepeater _nextRepeater;

    public IPositionRepeater tale
    {
        get => _nextRepeater;
        set => _nextRepeater = value;
    }

    private void Start()
    {
        if (_tale != null)
            SetNextRepeater(_tale);

        gameObject.tag = "Tail";

        if (!TryGetComponent<Collider2D>(out Collider2D col))
            col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    public void SetNextRepeater(IPositionRepeater repeater)
    {
        if (repeater == null || repeater == _nextRepeater) return;
        _nextRepeater = repeater;
    }

    public void SetPosition(Vector3 position)
    {
        Vector3 lastPosition = transform.position;
        transform.position = position;

        if (_nextRepeater != null)
            _nextRepeater.SetPosition(lastPosition);
    }
}
