using UnityEngine;

public class PlayerDeadState : State
{
    private PlayerController player;

    public PlayerDeadState(PlayerController player, StateMachine stateMachine)
    {
        this.player = player;
    }

    public override void Enter()
    {
        player.SetVelocity(0);

        player.GetComponent<Animator>().SetBool("isDead", true);

        player.enabled = false;
    }


}