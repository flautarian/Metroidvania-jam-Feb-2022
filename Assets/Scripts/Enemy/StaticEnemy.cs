using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnemy : EnemyController
{
    // Start is called before the first frame update

    [SerializeField]
    private int distanceCheckPlayer =0;

    [SerializeField]
    private LayerMask layerMask;

    private bool canAttack = false;

    public float attackTime =0;

    [SerializeField]
    private float rechargeAttackTime =0;
    

    public override void StartSpecificClasses(){
        attackTime = Time.time;
    }

    public override void ManageAttackRoutine(){
        if(attackTime + rechargeAttackTime < Time.time){
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.eulerAngles.y != 0 ? Vector2.left : Vector2.right, distanceCheckPlayer, layerMask);
            Debug.DrawRay(col.bounds.center + new Vector3(col.bounds.extents.x, 0), (transform.eulerAngles.y != 0 ? Vector2.left : Vector2.right) * distanceCheckPlayer, hit.collider != null ? Color.green : Color.red);
            if(hit.collider != null){
                Attack();
                attackTime = Time.time;
            }
        }
    }

    public override void ManageWalkRoutine(){
        // soy un gusanico, ni ando ni pico :D
    }

    public override void PathFollow()
    {
        // soy un gusanico, ni ando ni pico :D
    }

    public override void ManageHurtFromPlayer(Transform t)
    {
        if(!animator.GetBool(Constants.ANIM_BOOL_HURT)){
            BeginHurt();
            life -= GameManager.Instance.GetPlayerTotalAttack();
            if(life <= 0){
                life = 0;
                BeginDead();
            }
        }
    }
}
