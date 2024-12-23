using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SimpleBarricade : Obstacle
{
    private const int MinObstacleCount = 0;
    private const int MaxObstacleCount = 2;
    private const int LeftMostLaneIndex = -1;
    private const int RightMostLaneIndex = 1;

    public override async UniTask Spawn(TrackSegment segment, float t)
    {
        int obstacleCount = Random.Range(MinObstacleCount, MaxObstacleCount + 1);
        int startLane = Random.Range(LeftMostLaneIndex, RightMostLaneIndex + 1);

        Vector3 position;
        Quaternion rotation;
        segment.GetPointAt(t, out position, out rotation);

        for (int i = 0; i < obstacleCount; ++i)
        {
            int lane = startLane + i;
            lane = lane > RightMostLaneIndex ? LeftMostLaneIndex : lane;

            GameObject obj = await InstantiateObstacle(position, rotation);

            obj.transform.SetParent(segment.ObjectRoot, true);
            obj.transform.position += obj.transform.right * lane * segment.Manager.LaneOffset;
        }
    }

    private async UniTask<GameObject> InstantiateObstacle(Vector3 position, Quaternion rotation)
    {
        await UniTask.Yield();
        return Instantiate(gameObject, position, rotation);
    }
}
