using UnityEngine;

public class PlayerAttackState : State
{
    private PlayerController player;
    private StateMachine stateMachine;

    private float timer;

    public PlayerAttackState(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        timer = player.fireRate;

        player.Shoot();
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        // keep movement active
        float input = player.horizontalInput;
        player.SetVelocity(input * player.moveSpeed);

        if (timer <= 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
}