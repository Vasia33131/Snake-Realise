using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatTaleIncrementer : MonoBehaviour
{
    private const int SegmentListCapacity = 64;

    [SerializeField] private RecursivePositionRepeater _talePrefab;
    [SerializeField] [Min(0.02f)] private float _segmentPulseDuration = 0.12f;
    [SerializeField] [Min(1f)] private float _pulseScaleMultiplier = 1.22f;

    private readonly List<Transform> _segmentBuffer = new List<Transform>(SegmentListCapacity);
    private ICaudateObject _caudate;
    private bool _growAnimationActive;

    private void Awake()
    {
        _caudate = GetComponent<ICaudateObject>();
        if (_caudate == null)
            Debug.LogError("ICaudateObject component not found!", this);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col == null || _growAnimationActive) return;

        var eat = col.GetComponent<IEatDestroyer>();
        if (eat == null) return;

        if (_talePrefab == null)
        {
            Debug.LogError("Tale prefab is not assigned!", this);
            return;
        }

        StartCoroutine(EatGrowThenExtendRoutine(eat));
    }

    private IEnumerator EatGrowThenExtendRoutine(IEatDestroyer eat)
    {
        _growAnimationActive = true;
        try
        {
            FillSegmentChain(_segmentBuffer, _caudate);
            for (int i = 0; i < _segmentBuffer.Count; i++)
                yield return PulseSegmentScale(_segmentBuffer[i]);

            AddTailSegment();
            eat.Destroy();
        }
        finally
        {
            _growAnimationActive = false;
        }
    }

    private static void FillSegmentChain(List<Transform> into, ICaudateObject head)
    {
        into.Clear();
        ICaudateObject node = head;
        while (node != null)
        {
            into.Add(((MonoBehaviour)node).transform);
            node = node.tale as ICaudateObject;
        }
    }

    private IEnumerator PulseSegmentScale(Transform segment)
    {
        Vector3 baseScale = segment.localScale;
        Vector3 peakScale = baseScale * _pulseScaleMultiplier;
        float half = _segmentPulseDuration * 0.5f;

        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            float u = Mathf.Clamp01(t / half);
            u = u * u * (3f - 2f * u);
            segment.localScale = Vector3.Lerp(baseScale, peakScale, u);
            yield return null;
        }

        segment.localScale = peakScale;

        for (float t = 0f; t < half; t += Time.deltaTime)
        {
            float u = Mathf.Clamp01(t / half);
            u = u * u * (3f - 2f * u);
            segment.localScale = Vector3.Lerp(peakScale, baseScale, u);
            yield return null;
        }

        segment.localScale = baseScale;
    }

    private void AddTailSegment()
    {
        Vector3 spawnPosition = _caudate.tale != null
            ? ((MonoBehaviour)_caudate.tale).transform.position
            : transform.position - transform.right * 0.5f;

        RecursivePositionRepeater newSegment = Instantiate(_talePrefab, spawnPosition, Quaternion.identity);

        if (_caudate.tale != null)
            newSegment.SetNextRepeater(_caudate.tale);

        _caudate.tale = newSegment;
    }
}
