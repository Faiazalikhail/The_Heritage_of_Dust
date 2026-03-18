public class PlayerIdleState : State
{
    private PlayerController player;
    private StateMachine stateMachine;

    public PlayerIdleState(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        player.SetVelocity(0);
    }

    public override void Update()
    {
        if (player.horizontalInput != 0)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}