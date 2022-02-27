using UnityEngine;
using Pathfinding;
public class FlyingEnemy : EnemyController
{

    public int patrolObjective = 0, patrolOrientation = 1;

    [SerializeField]
    private Vector2[] patrolPlaces;

    [SerializeField]
    private PlayerChecker playerChecker;
    
    [SerializeField]
    private AIPath aIPath;
    
    [SerializeField]
    private AIDestinationSetter aIDestination;

    private bool playerDetected = false;

    [SerializeField]
    private Transform enemyTransform;

    private Transform objective;



    public override void StartSpecificClasses(){
    }

    public override void ManageWalkRoutine() {
        if(mustPatrol){
            if(patrolPlaces.Length > 0){
                enemyTransform.position = Vector2.MoveTowards(enemyTransform.position, patrolPlaces[patrolObjective], speed * Time.deltaTime);
                if(Vector2.Distance(enemyTransform.position, patrolPlaces[patrolObjective]) < 0.2f){
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
            transform.localScale = new Vector3(aIPath.desiredVelocity.x < 0.1f ? 1f : -1f, 1f, 1f);
        }
    }

    public override void ManageAttackRoutine()
    {
        
    }

    public override void ManageHurtFromPlayer(Transform t)
    {
        if(!animator.GetBool(Constants.ANIM_BOOL_HURT)){
            BeginHurt();
            if(playerChecker.playerDetected == null)
                playerChecker.playerDetected = t;
            if(alterable){
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(t.position.x > transform.position.x ? -5 : 5, 10), ForceMode2D.Impulse);
            }
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
