using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalEnemy : EnemyController
{
    [SerializeField]
    GroundChecker groundChecker;

    private bool goRight = true;

    private bool playerDetected = false;

    Transform objective;

    public override void StartSpecificClasses(){
    }

    public override void ManageWalkRoutine() {
        if(mustPatrol){
            animator.SetBool(Constants.ANIM_BOOL_RUN, true);
            if(!groundChecker.TouchesGround()){
                transform.eulerAngles = new Vector3(0, goRight ? -180 : 0, 0);
                goRight = !goRight;
            }
            if(!animator.GetBool(Constants.ANIM_BOOL_HURT))
                rb.velocity = (goRight ? Vector2.right : Vector2.left) * speed;
        }
        else if(playerDetected){
            // TODO: implement follow player
        }
    }

    public override void ManageAttackRoutine()
    {
        
    }

    public override void ManageHurtFromPLayer(Collider2D other)
    {
        if(other.gameObject.tag.Equals(Constants.TAG_PLAYER_WEAPON) && !animator.GetBool(Constants.ANIM_BOOL_HURT)){
            BeginHurt();
            if(alterable){
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(other.transform.position.x > transform.position.x ? -5 : 5, 10), ForceMode2D.Impulse);
            }
                
        }
    }

}
