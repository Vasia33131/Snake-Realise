using UnityEngine;

public class EatTaleIncrementer : MonoBehaviour
{
    [SerializeField] private RecursivePositionRepeater _talePrefab;
    private ICaudateObject _caudate;

    void Start()
    {
        _caudate = GetComponent<ICaudateObject>();
        if (_caudate == null)
        {
            Debug.LogError("ICaudateObject component not found!", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col == null) return;

        var eat = col.GetComponent<IEatDestroyer>();
        if (eat != null)
        {
            // Проверяем префаб хвоста
            if (_talePrefab != null)
            {
                AddTailSegment();
                eat.Destroy();
            }
            else
            {
                Debug.LogError("Tale prefab is not assigned!", this);
            }
        }
    }

    private void AddTailSegment()
    {
        Vector3 spawnPosition = _caudate.tale != null ?
            ((MonoBehaviour)_caudate.tale).transform.position :
            transform.position - transform.right * 0.5f;

        var newSegment = Instantiate(_talePrefab, spawnPosition, Quaternion.identity);

        if (_caudate.tale != null)
        {
            newSegment.SetNextRepeater(_caudate.tale);
        }
        _caudate.tale = newSegment;
    }
}