using UnityEngine;

public class EatPositionChanger : MonoBehaviour, IEatDestroyer
{
    [SerializeField] private Transform leftUpBound;
    [SerializeField] private Transform rightDownBound;

    /// <summary>Те же углы прямоугольника спавна, что использует <see cref="GeneratePosition"/>.</summary>
    public Transform SharedLeftUpBound => leftUpBound;
    public Transform SharedRightDownBound => rightDownBound;

    private Camera _camera;

    private void Awake()
    {
        _camera = Camera.main;
    }

    public void Destroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(1);
        else
            Debug.LogWarning("GameManager.Instance is null!");

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayEatSound();
        else
            Debug.LogWarning("AudioManager.Instance is null!");

        GeneratePosition();
    }

    private void GeneratePosition()
    {
        if (_camera == null)
            _camera = Camera.main;
        FieldItemRandomPlacer.TryRelocate(transform, leftUpBound, rightDownBound, _camera);
    }
}
