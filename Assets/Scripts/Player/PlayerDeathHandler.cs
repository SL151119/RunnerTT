using UnityEngine;
using VContainer;

public class PlayerDeathHandler : MonoBehaviour
{
    private Animator _animator;
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    [Inject]
    public void Initialize(Animator animator)
    {
        _animator = animator;
    }

    public void TriggerDeathAnimation()
    {
        _animator.SetTrigger(DeadHash);
    }
}
