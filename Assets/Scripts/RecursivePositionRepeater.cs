using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecursivePositionRepeater : MonoBehaviour, IRecursivePositionRepeater, ICaudateObject
{
    [SerializeField] private RecursivePositionRepeater _tale;
    private IPositionRepeater _nextRepeater;

    public IPositionRepeater tale { get => _nextRepeater; set => _nextRepeater = value; }

    private void Start()
    {
        if (_tale != null)
            SetNextRepeater(_tale);

        // Устанавливаем тег для всех сегментов хвоста
        gameObject.tag = "Tail";

        // Добавляем коллайдер, если его нет
        if (!TryGetComponent<Collider2D>(out _))
        {
            var collider = gameObject.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
        }
    }

    public void SetNextRepeater(IPositionRepeater repeater)
    {
        if (repeater == null || repeater == _nextRepeater) return;
        _nextRepeater = repeater;
    }

    public void SetPosition(Vector3 position)
    {
        var lastPosition = transform.position;
        transform.position = position;

        if (_nextRepeater != null)
            _nextRepeater.SetPosition(lastPosition);
    }
}