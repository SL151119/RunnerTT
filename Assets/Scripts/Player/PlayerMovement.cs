using UnityEngine;
using VContainer;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerCollider _playerCollider;
    [SerializeField] private float _laneOffset = 2f;
    [SerializeField] private float _laneChangeSpeed = 5f;

    private int _currentLane = 1; // (0: left, 1: center, 2: right)

    private Vector3 _targetPosition;

    private bool _isRunning;

    private PlayerInput _playerInput;

    [Inject]
    public void Initialize(PlayerInput playerInput)
    {
        _targetPosition = Vector3.zero;

        _playerInput = playerInput;
        _playerInput.OnMove += HandleLaneChange;
    }

    private void Update()
    {
        if (!_isRunning)
        {
            return;
        }

        SmoothLaneChange();
    }

    private void HandleLaneChange(int direction)
    {
        if (!_isRunning)
        {
            return;
        }

        int targetLane = _currentLane + direction;

        if (targetLane < 0 || targetLane > 2)
        {
            return; // Ignore out-of-bounds lane changes
        }

        _currentLane = targetLane;
        _targetPosition = new Vector3((_currentLane - 1) * _laneOffset, 0, 0);
    }

    private void SmoothLaneChange()
    {
        _playerCollider.transform.localPosition = Vector3.Lerp(_playerCollider.transform.localPosition, _targetPosition, Time.deltaTime * _laneChangeSpeed);
    }

    public void StartMoving()
    {
        _isRunning = true;
    }

    public void StopMoving()
    {
        _isRunning = false;

        _playerInput.OnMove -= HandleLaneChange;
    }
}
