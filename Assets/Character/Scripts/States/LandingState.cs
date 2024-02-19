using UnityEngine;

public class LandingState : BaseState
{
    public LandingState(PlayerController player, Animator animator) : base(player, animator) { }

    public override void OnEnter(IState previousState)
    {
        animator.CrossFade(player.GetAnimation("Landing"), 0.01f);
    }
}
