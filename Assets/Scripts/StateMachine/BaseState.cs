using UnityEngine;

public abstract class BaseState : IState
{
    protected readonly PlayerController player;
    protected readonly Animator animator;

    protected BaseState(PlayerController player, Animator animator)
    {
        this.player = player;
        this.animator = animator;
    }

    protected const float crosFadeDuration = 0.1f;
    public virtual void FixedUpdate()
    {
        // noop
    }

    public virtual void OnEnter(IState previousState)
    {
        // noop
    }

    public virtual void OnExit()
    {
        // noop
    }

    public virtual void Update()
    {
        // noop
    }
}