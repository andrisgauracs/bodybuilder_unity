using UnityEngine;

public class JumpState : BaseState
{
    public JumpState(PlayerController player, Animator animator) : base(player, animator) { }

    public override void OnEnter(IState previousState)
    {
        animator.CrossFade(player.GetAnimation("Jumping"), 0.01f);
    }

    public override void OnExit()
    {
        animator.speed = 1f;
    }
}
