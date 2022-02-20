using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    internal Rigidbody2D rb;

    internal Animator animator;

    [SerializeField]
    internal int attack =0;
    
    [SerializeField]
    internal int life =0;

    [SerializeField]
    internal bool alterable = false;

    internal BoxCollider2D col;

    internal SpriteRenderer spriteRenderer;

    private void FixedUpdate() {
        ManageAttackRoutine();
        ManageWalkRoutine();
    }

    public abstract void ManageAttackRoutine();

    public abstract void ManageWalkRoutine();

    public abstract void ManageHurtFromPLayer(Collider2D other);

    internal void Attack(){
        animator.SetBool(Constants.ENEMY_ANIM_BOOL_ATTACK, true);
    }
    internal void Die(){
        animator.SetBool(Constants.ANIM_BOOL_DIE, true);
    }

    internal void BeginHurt(){
        animator.SetBool(Constants.ANIM_BOOL_HURT, true);
    }

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        ManageHurtFromPLayer(other);
    }

}
