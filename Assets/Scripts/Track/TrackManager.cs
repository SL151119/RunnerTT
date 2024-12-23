using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class TrackManager : MonoBehaviour
{
    [SerializeField] private PlayerMovement _playerMovement;

    [Header("General Settings")]
    [SerializeField] private float _minSpeed = 5.0f;
    [SerializeField] private float _maxSpeed = 10.0f;
    [SerializeField] private float _laneOffset = 3.0f;

    [Header("ThemeData")]
    [SerializeField] private ThemeData _themeData;

    private float _currentSegmentDistance;
    private float _currentZoneDistance;

    private float _speed;

    private bool _isMoving;

    private int _currentZone;
    private int _safeSegmentLeft;

    public float LaneOffset => _laneOffset;
    private Camera MainCamera => Camera.main;

    private EnergySpawner _energySpawner;

    private List<TrackSegment> _segments = new List<TrackSegment>();
    private List<TrackSegment> _pastSegments = new List<TrackSegment>();

    private readonly int _previousSegment = -1;

    /* If player reaches a certain distance from origin(given by FloatingOriginThreshold) - 
     * moves everything back by that threshold to "reset" the player to the origin. 
     * This allow to avoid floating point error on long run.*/
    private const float FloatingOriginThreshold = 10000f;

    private const float StartingSegmentDistance = 2f;
    private const int StartingSafeSegments = 2;
    private const int DesiredSegmentCount = 10;
    private const float SegmentRemovalDistance = -30f;
    private const float Acceleration = 0.2f;

    [Inject]
    public void Initialize(EnergySpawner energySpawner)
    {
        _energySpawner = energySpawner;
    }

    public void StartMove()
    {
        _playerMovement.StartMoving();
        _isMoving = true;

        _speed = _minSpeed;
    }

    public void StopMove()
    {
        _isMoving = false;
    }

    public void Begin()
    {
        _currentSegmentDistance = StartingSegmentDistance;

        _playerMovement.gameObject.SetActive(true);

        MainCamera.transform.SetParent(_playerMovement.transform, true);

        _currentZone = 0;
        _currentZoneDistance = 0;

        _safeSegmentLeft = StartingSafeSegments;
    }

    public void End()
    {
        for (int i = 0; i < _pastSegments.Count; ++i)
        {
            Destroy(_pastSegments[i].gameObject);
        }

        _segments.Clear();
        _pastSegments.Clear();

        _playerMovement.StopMoving();
        
        StopMove();

        gameObject.SetActive(false);
    }

    private int _spawnedSegments = 0;

    void Update()
    {
        if (!_isMoving)
        {
            return;
        }

        while (_spawnedSegments < (DesiredSegmentCount))
        {
            SpawnNewSegment();
            _spawnedSegments++;
        }

        float scaledSpeed = _speed * Time.deltaTime;
        _currentZoneDistance += scaledSpeed;

        _currentSegmentDistance += scaledSpeed;

        if (_currentSegmentDistance > _segments[0].WorldLength)
        {
            _currentSegmentDistance -= _segments[0].WorldLength;

            // m_PastSegments are segment we already passed, we keep them to move them and destroy them later 
            // but they aren't part of the game anymore 
            _pastSegments.Add(_segments[0]);
            _segments.RemoveAt(0);
            _spawnedSegments--;
        }

        Vector3 currentPos;
        Quaternion currentRot;
        Transform characterTransform = _playerMovement.transform;

        _segments[0].GetPointAtInWorldUnit(_currentSegmentDistance, out currentPos, out currentRot);

        // Floating origin implementation
        // Move the whole world back to 0,0,0 when we get too far away.
        bool needRecenter = currentPos.sqrMagnitude > FloatingOriginThreshold;

        if (needRecenter)
        {
            int count = _segments.Count;
            for (int i = 0; i < count; i++)
            {
                _segments[i].transform.position -= currentPos;
            }

            count = _pastSegments.Count;
            for (int i = 0; i < count; i++)
            {
                _pastSegments[i].transform.position -= currentPos;
            }

            // Recalculate current world position based on the moved world
            _segments[0].GetPointAtInWorldUnit(_currentSegmentDistance, out currentPos, out currentRot);
        }

        characterTransform.rotation = currentRot;
        characterTransform.position = currentPos;

        // Still move past segment until they aren't visible anymore.
        for (int i = 0; i < _pastSegments.Count; ++i)
        {
            if ((_pastSegments[i].transform.position - currentPos).z < SegmentRemovalDistance)
            {
                _pastSegments[i].Cleanup();
                _pastSegments.RemoveAt(i);
                i--;
            }
        }

        if (_speed < _maxSpeed)
        {
            _speed += Acceleration * Time.deltaTime;
        }
        else
        {
            _speed = _maxSpeed;
        }
    }

    public void ChangeZone()
    {
        _currentZone += 1;
        
        if (_currentZone >= _themeData.Zones.Length)
        {
            _currentZone = 0;
        }

        _currentZoneDistance = 0;
    }

    private readonly Vector3 _offScreenSpawnPos = Vector3.zero;

    public void SpawnNewSegment()
    {
        if (_themeData.Zones[_currentZone].length < _currentZoneDistance)
        {
            ChangeZone();
        }

        int segmentUse = Random.Range(0, _themeData.Zones[_currentZone].prefabList.Length);
        if (segmentUse == _previousSegment)
        {
            segmentUse = (segmentUse + 1) % _themeData.Zones[_currentZone].prefabList.Length;
        }

        GameObject segmentToUse = Instantiate(
            _themeData.Zones[_currentZone].prefabList[segmentUse],
            _offScreenSpawnPos,
            Quaternion.identity
        );

        TrackSegment newSegment = segmentToUse.GetComponent<TrackSegment>();

        Vector3 currentExitPoint;
        Quaternion currentExitRotation;
        if (_segments.Count > 0)
        {
            _segments[_segments.Count - 1].GetPointAt(1.0f, out currentExitPoint, out currentExitRotation);
        }
        else
        {
            currentExitPoint = transform.position;
            currentExitRotation = transform.rotation;
        }

        newSegment.transform.rotation = currentExitRotation;

        Vector3 entryPoint;
        Quaternion entryRotation;
        newSegment.GetPointAt(0.0f, out entryPoint, out entryRotation);

        Vector3 pos = currentExitPoint + (newSegment.transform.position - entryPoint);
        newSegment.transform.position = pos;
        newSegment.Manager = this;

        newSegment.transform.localScale = new Vector3((Random.value > 0.5f ? -1 : 1), 1, 1);
        newSegment.ObjectRoot.localScale = new Vector3(1.0f / newSegment.transform.localScale.x, 1, 1);

        if (_safeSegmentLeft <= 0)
        {
            SpawnObstacle(newSegment);
            SpawnEnergies(newSegment);
        }
        else
        {
            _safeSegmentLeft -= 1;
        }

        _segments.Add(newSegment);
    }

    public void SpawnObstacle(TrackSegment segment)
    {
        if (segment.PossibleObstacles.Length != 0)
        {
            for (int i = 0; i < segment.obstaclePositions.Length; ++i)
            {
                GameObject possibleObstacle = segment.PossibleObstacles[Random.Range(0, segment.PossibleObstacles.Length)];
                SpawnFromPossibleObstacles(possibleObstacle, segment, i);
            }
        }
    }

    private void SpawnFromPossibleObstacles(GameObject possibleObstacle, TrackSegment segment, int posIndex)
    {
        GameObject obj = possibleObstacle;

        if (obj.TryGetComponent<Obstacle>(out var obstacle))
            obstacle.Spawn(segment, segment.obstaclePositions[posIndex]);

    }

    public void SpawnEnergies(TrackSegment segment)
    {
        const float increment = 4f; // Energy spawn frequency
        float currentWorldPos = 0.0f;

        int energiesToSpawn = Random.Range(1, 3);

        while (currentWorldPos < segment.WorldLength && energiesToSpawn > 0)
        {
            Vector3 pos;
            Quaternion rot;
            segment.GetPointAtInWorldUnit(currentWorldPos, out pos, out rot);

            for (int attempt = 0; attempt < 3; ++attempt)
            {
                int randomLane = Random.Range(-1, 2); // Choose a random lane (-1: left, 0: center, 1: right)
                Vector3 lanePosition = pos + (randomLane * _laneOffset * (rot * Vector3.right));

                lanePosition.y = lanePosition.y + 0.7f; //Prevent spawn energies under ground

                // Check if this position is too close to any obstacle
                bool isTooClose = false;
                foreach (float obstacleT in segment.obstaclePositions)
                {
                    float obstacleWorldPos = obstacleT * segment.WorldLength;
                    if (Mathf.Abs(obstacleWorldPos - currentWorldPos) < 2.0f) // Minimum safe distance
                    {
                        isTooClose = true;
                        break;
                    }
                }

                if (!isTooClose)
                {
                    _energySpawner.SpawnEnergy(lanePosition, rot, segment.CollectableRoot);

                    energiesToSpawn--;
                    break; // Move to the next world position
                }
            }

            currentWorldPos += increment;
        }
    }
}
