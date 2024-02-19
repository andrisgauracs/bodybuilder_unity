using UnityEngine;

public class RunningState : BaseState
{
    public RunningState(PlayerController player, Animator animator) : base(player, animator) { }

    public override void OnEnter(IState previousState)
    {
        animator.CrossFade(player.GetAnimation("Run"), 0.04f);
    }
}
