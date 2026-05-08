using UnityEngine;

/// <summary>Случайная позиция в прямоугольнике между двумя углами + тот же raycast от камеры, что у еды (<see cref="EatPositionChanger"/>).</summary>
public static class FieldItemRandomPlacer
{
    public const int TryCounts = 1000;
    public const float RaycastDistance = 100f;

    public static bool TryRelocate(Transform target, Transform leftUpBound, Transform rightDownBound, Camera camera)
    {
        if (target == null || leftUpBound == null || rightDownBound == null)
            return false;

        if (camera == null)
            camera = Camera.main;
        if (camera == null)
            return false;

        float z = target.position.z;

        for (int i = 0; i < TryCounts; i++)
        {
            Vector3 newPos = new Vector3(
                Random.Range(leftUpBound.position.x, rightDownBound.position.x),
                Random.Range(leftUpBound.position.y, rightDownBound.position.y),
                z);

            if (!IsValidPosition(newPos, camera))
                continue;

            target.position = newPos;
            return true;
        }

        // Raycast может не отсекать 2D-коллайдеры / не найти свободную точку за 1000 попыток — всё равно ставим в зону.
        Vector3 fallback = new Vector3(
            Random.Range(leftUpBound.position.x, rightDownBound.position.x),
            Random.Range(leftUpBound.position.y, rightDownBound.position.y),
            z);
        target.position = fallback;
        return true;
    }

    private static bool IsValidPosition(Vector3 pos, Camera camera)
    {
        Vector3 direction = camera.transform.position - pos;
        return !Physics.Raycast(camera.transform.position, direction, RaycastDistance);
    }
}
