#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class TrackSegment : MonoBehaviour
{
    [SerializeField] private Transform _pathParent;

    [SerializeField] private Transform _objectRoot;
    [SerializeField] private Transform _collectableRoot;

    [SerializeField] private GameObject[] _possibleObstacles;

    private float _worldLength;

    [HideInInspector]
    public float[] obstaclePositions;

    public TrackManager Manager { get; set; }
    public GameObject[] PossibleObstacles => _possibleObstacles;
    public Transform ObjectRoot => _objectRoot;
    public Transform CollectableRoot => _collectableRoot;
    public float WorldLength => _worldLength;

    private void OnEnable()
    {
        UpdateWorldLength();

        GameObject obj = new GameObject("ObjectRoot");
        obj.transform.SetParent(transform);
        _objectRoot = obj.transform;

        obj = new GameObject("Collectables");
        obj.transform.SetParent(_objectRoot);
        _collectableRoot = obj.transform;
    }

    public void GetPointAtInWorldUnit(float wt, out Vector3 pos, out Quaternion rot)
    {
        float t = wt / _worldLength;
        GetPointAt(t, out pos, out rot);
    }

    public void GetPointAt(float t, out Vector3 pos, out Quaternion rot)
    {
        float clampedT = Mathf.Clamp01(t);
        float scaledT = (_pathParent.childCount - 1) * clampedT;
        int index = Mathf.FloorToInt(scaledT);
        float segmentT = scaledT - index;

        Transform orig = _pathParent.GetChild(index);
        if (index == _pathParent.childCount - 1)
        {
            pos = orig.position;
            rot = orig.rotation;
            return;
        }

        Transform target = _pathParent.GetChild(index + 1);

        pos = Vector3.Lerp(orig.position, target.position, segmentT);
        rot = Quaternion.Lerp(orig.rotation, target.rotation, segmentT);
    }

    private void UpdateWorldLength()
    {
        _worldLength = 0;

        for (int i = 1; i < _pathParent.childCount; ++i)
        {
            Transform orig = _pathParent.GetChild(i - 1);
            Transform end = _pathParent.GetChild(i);

            Vector3 vec = end.position - orig.position;
            _worldLength += vec.magnitude;
        }
    }

    public void Cleanup()
    {
        Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_pathParent == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        for (int i = 1; i < _pathParent.childCount; ++i)
        {
            Transform orig = _pathParent.GetChild(i - 1);
            Transform end = _pathParent.GetChild(i);

            Gizmos.DrawLine(orig.position, end.position);
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < obstaclePositions.Length; ++i)
        {
            Vector3 pos;
            Quaternion rot;
            GetPointAt(obstaclePositions[i], out pos, out rot);
            Gizmos.DrawSphere(pos, 0.5f);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(TrackSegment))]
class TrackSegmentEditor : Editor
{
    private TrackSegment _segment;

    private void OnEnable()
    {
        _segment = (TrackSegment)target;
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        base.OnInspectorGUI();

        // Button to add a new obstacle
        if (GUILayout.Button("Add obstacles"))
        {
            Undo.RecordObject(_segment, "Add Obstacle");
            ArrayUtility.Add(ref _segment.obstaclePositions, 0.0f);
            EditorUtility.SetDirty(_segment);
        }

        if (_segment.obstaclePositions != null)
        {
            int toRemove = -1;

            for (int i = 0; i < _segment.obstaclePositions.Length; ++i)
            {
                GUILayout.BeginHorizontal();

                // Slider for obstacle position
                float newValue = EditorGUILayout.Slider(
                    "Obstacle Position",
                    _segment.obstaclePositions[i],
                    0.0f,
                    1.0f
                );

                // Check for changes
                if (!Mathf.Approximately(newValue, _segment.obstaclePositions[i]))
                {
                    Undo.RecordObject(_segment, "Change Obstacle Position");
                    _segment.obstaclePositions[i] = newValue;
                    EditorUtility.SetDirty(_segment);
                }

                // Remove button
                if (GUILayout.Button("-", GUILayout.MaxWidth(32)))
                {
                    toRemove = i;
                }

                GUILayout.EndHorizontal();
            }

            // Remove selected obstacle
            if (toRemove != -1)
            {
                Undo.RecordObject(_segment, "Remove Obstacle");
                ArrayUtility.RemoveAt(ref _segment.obstaclePositions, toRemove);
                EditorUtility.SetDirty(_segment);
            }
        }
    }
}
#endif

