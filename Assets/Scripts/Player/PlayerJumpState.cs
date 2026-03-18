public class PlayerJumpState : State
{
    private PlayerController player;
    private StateMachine stateMachine;

    public PlayerJumpState(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        player.Jump();
    }

    public override void Update()
    {
        float input = player.horizontalInput;

        player.SetVelocity(input * player.moveSpeed);

        if (player.isGrounded)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}