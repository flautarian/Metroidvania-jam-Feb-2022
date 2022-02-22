using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class FlyingEnemy : EnemyController
{

    public int patrolObjective = 0, patrolOrientation = 1;

    [SerializeField]
    private Vector2[] patrolPlaces;

    [SerializeField]
    private PlayerChecker playerChecker;

    private AIPath aIPath;

    private AIDestinationSetter aIDestination;

    private bool playerDetected = false;

    Transform objective;

    public override void StartSpecificClasses(){
        aIPath = GetComponent<AIPath>();
        aIDestination = GetComponent<AIDestinationSetter>();
    }

    public override void ManageWalkRoutine() {
        if(mustPatrol){
            if(patrolPlaces.Length > 0){
                transform.position = Vector2.MoveTowards(transform.position, patrolPlaces[patrolObjective], speed * Time.deltaTime);
                if(Vector2.Distance(transform.position, patrolPlaces[patrolObjective]) < 0.2f){
                    patrolObjective += patrolOrientation;
                    if(patrolObjective == patrolPlaces.Length -1)
                        patrolOrientation = -1;
                    if(patrolObjective == 0)
                        patrolOrientation = 1;
                }
                spriteRenderer.flipX = transform.position.x > patrolPlaces[patrolObjective].x;
            }
        }
        if(playerChecker.playerDetected != null){
            mustPatrol = false;
            aIDestination.target = playerChecker.playerDetected;
            aIPath.canMove = true;
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
            aIDestination.target = other.gameObject.transform;
            aIPath.canMove = true;
        }
    }

    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        
    }
}
