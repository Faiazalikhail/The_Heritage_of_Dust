using UnityEngine;

public class PlayerHurtState : State
{
    private PlayerController player;
    private StateMachine stateMachine;

    private float timer = 0.3f;

    public PlayerHurtState(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public override void Enter()
    {
        timer = 0.3f;

        // small knockback (optional)
        player.SetVelocity(-player.transform.localScale.x * 3f);
    }

    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }

    public override void Exit()
    {
        player.GetComponent<Animator>().SetBool("isHurt", false);
    }
}