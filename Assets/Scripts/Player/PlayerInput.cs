using UnityEngine;

public class PlayerInput
{
    private readonly PlayerControls _controls;
    public event System.Action<int> OnMove;
    public event System.Action OnExit;

    public PlayerInput(PlayerControls controls)
    {
        _controls = controls;

        AssignInputEvents();
    }

    public void AssignInputEvents()
    {
        _controls.Player.Move.performed += ctx => OnMove?.Invoke(Mathf.RoundToInt(ctx.ReadValue<float>()));

        _controls.Player.ExitGame.performed += ctx => OnExit?.Invoke();

        _controls.Player.Enable();
    }

    public void DisableInputs()
    {
        _controls.Player.Disable();
    }
}
