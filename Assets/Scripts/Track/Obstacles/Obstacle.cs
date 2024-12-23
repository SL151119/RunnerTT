using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    public abstract UniTask Spawn(TrackSegment segment, float t);
}
