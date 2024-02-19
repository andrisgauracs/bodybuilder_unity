using UnityEngine;

public class WalkingState : BaseState
{
    public WalkingState(PlayerController player, Animator animator) : base(player, animator) { }

    public override void OnEnter(IState previousState)
    {
        animator.CrossFade(player.GetAnimation("Walk"), 0.04f);
    }
}
