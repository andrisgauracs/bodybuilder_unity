using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(PlayerController player, Animator animator) : base(player, animator) { }

    public override void OnEnter(IState previousState)
    {
        animator.CrossFade(player.GetAnimation("Idle"), previousState?.GetType() == typeof(JumpState) ? 0.01f : crosFadeDuration);
    }
}
