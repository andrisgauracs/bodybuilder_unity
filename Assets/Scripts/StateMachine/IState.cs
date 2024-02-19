public interface IState
{
    void OnEnter(IState state = null);
    void Update();
    void FixedUpdate();
    void OnExit();

}