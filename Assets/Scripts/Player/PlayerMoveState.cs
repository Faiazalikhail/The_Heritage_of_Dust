using UnityEngine;

public class PlayerMoveState : State
{
    private PlayerController player;
    private StateMachine stateMachine;

    public PlayerMoveState(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Update()
    {
        float input = player.horizontalInput;

        player.SetVelocity(input * player.moveSpeed);

        // Flip character
        if (input != 0)
        {
            player.transform.localScale = new Vector3(Mathf.Sign(input), 1, 1);
        }

        if (input == 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}