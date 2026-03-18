using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public int health = 1;
    protected Animator anim;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    public virtual void TakeDamage()
    {
        health--;

        if (health > 0)
        {
            anim.SetBool("IsHurt", true);
            Invoke(nameof(StopHurt), 0.3f);
        }
        else
        {
            Die();
        }
    }

    void StopHurt()
    {
        anim.SetBool("IsHurt", false);
    }

    protected virtual void Die()
    {
        anim.SetBool("isDead", true);
        Destroy(gameObject, 1f);
    }
}