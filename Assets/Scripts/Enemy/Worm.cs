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
    private Shot shot;

    public float attackTime =0;

    [SerializeField]
    private float rechargeAttackTime =0;
    

    public override void StartSpecificClasses(){
        attackTime = Time.time;
    }

    public override void ManageAttackRoutine(){
        if(attackTime + rechargeAttackTime < Time.time && !shot.gameObject.activeInHierarchy){
            RaycastHit2D hit = Physics2D.Raycast(transform.position, spriteRenderer.flipX ? Vector2.left : Vector2.right, distanceCheckPlayer, layerMask);
            Debug.DrawRay(col.bounds.center + new Vector3(col.bounds.extents.x, 0), (spriteRenderer.flipX ? Vector2.left : Vector2.right) * distanceCheckPlayer, hit.collider != null ? Color.green : Color.red);
            if(hit.collider != null){
                Attack();
                attackTime = Time.time;
            }
        }
    }

    public void Shot(){
        shot.gameObject.SetActive(true);
        shot.orientation = spriteRenderer.flipX;
        shot.PrepareShot();
    }

    public override void ManageWalkRoutine(){
        // soy un gusanico, ni ando ni pico :D
    }

    public override void ManageHurtFromPlayer(Transform t)
    {
        if(!animator.GetBool(Constants.ANIM_BOOL_HURT)){
            BeginHurt();
            life -= GameManager.Instance.GetPlayerTotalAttack();
            //rb.AddForce(new Vector2(other.transform.position.x > transform.position.x ? -10 : 10, 0));
        }
    }
}
