using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worm : EnemyController
{
    // Start is called before the first frame update

    [SerializeField]
    private int distanceCheckPlayer =0;

    [SerializeField]
    private LayerMask layerMask;

    private bool canAttack = false;

    [SerializeField]
    private WormShoot shoot;

    public float attackTime =0;

    [SerializeField]
    private float rechargeAttackTime =0;
    void Start()
    {
        attackTime = Time.time;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void ManageAttackRoutine(){
        if(attackTime + rechargeAttackTime < Time.time && !shoot.gameObject.activeInHierarchy){
            RaycastHit2D hit = Physics2D.Raycast(transform.position, spriteRenderer.flipX ? Vector2.left : Vector2.right, distanceCheckPlayer, layerMask);
            Debug.DrawRay(col.bounds.center + new Vector3(col.bounds.extents.x, 0), (spriteRenderer.flipX ? Vector2.left : Vector2.right) * distanceCheckPlayer, hit.collider != null ? Color.green : Color.red);
            if(hit.collider != null){
                Attack();
                attackTime = Time.time;
            }
        }
    }

    public void Shot(){
        shoot.gameObject.SetActive(true);
        shoot.orientation = spriteRenderer.flipX;
        shoot.PrepareShoot();
    }

    public override void ManageWalkRoutine(){
        // soy un gusanico, ni ando ni pico :D
    }

    public override void ManageHurtFromPLayer(Collider2D other)
    {
        if(other.gameObject.tag.Equals(Constants.TAG_PLAYER_WEAPON) && !animator.GetBool(Constants.ANIM_BOOL_HURT)){
            BeginHurt();
            //rb.AddForce(new Vector2(other.transform.position.x > transform.position.x ? -10 : 10, 0));
        }
    }
}
