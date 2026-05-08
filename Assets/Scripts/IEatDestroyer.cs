using UnityEngine;

public interface IEatDestroyer
{
    void Destroy();
}

public interface IObjectMover
{
    float speed { get; }
    void MoveForward();
    void Stop();
    void Rotate(Quaternion quaternion);
}

public interface ICaudateObject
{
    IPositionRepeater tale { get; set; }
}

public interface IPositionRepeater
{
    void SetPosition(Vector3 position);
}

public interface IRecursivePositionRepeater : IPositionRepeater
{
    void SetNextRepeater(IPositionRepeater repeater);
}