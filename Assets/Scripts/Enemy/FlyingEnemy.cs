using UnityEngine;
using Pathfinding;
public class FlyingEnemy : EnemyController
{
    public int patrolObjective = 0, patrolOrientation = 1;

    [SerializeField]
    private Vector2[] patrolPlaces;

    [SerializeField]
    private PlayerChecker playerChecker;

    private bool playerDetected = false;
    private Collider2D colliderObject;

    public override void StartSpecificClasses(){
        seeker = GetComponent<Seeker>();
        colliderObject = GetComponent<Collider2D>();
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
                enemyTransform.eulerAngles = new Vector3(0, transform.position.x > patrolPlaces[patrolObjective].x ? -180 : 0, 0);
            }
        }
        if(playerChecker.playerDetected != null){
             // AI
            if(mustPatrol)
                InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
            mustPatrol = false;
            target = playerChecker.playerDetected;
            followEnabled = true;
            if (TargetInDistance() && followEnabled)
            {
                PathFollow();
            }
        }
    }

    public override void ManageAttackRoutine()
    {
        if(groundChecker.canDetectPlayer && groundChecker.TouchesPlayer()){
            if(target != null){
                if(target.TryGetComponent<PlayerController>(out PlayerController player)){
                    player.HurtPlayer(attack, transform.position);
                }
            }
        }
    }

    public override void ManageHurtFromPlayer(Transform t)
    {
        if(!animator.GetBool(Constants.ANIM_BOOL_HURT)){
            life -= GameManager.Instance.GetPlayerTotalAttack();
            if(life <= 0){
                life = 0;
                BeginDead();
            }
            BeginHurt();
            if(playerChecker.playerDetected is null)
                playerChecker.playerDetected = t;
            if(alterable){
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(t.position.x > transform.position.x ? -3 : 3, t.position.y > transform.position.y ? -5 : 5), ForceMode2D.Impulse);
            }
        }
    }

    public override void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }
        // See if colliding with anything
        Vector3 startOffset = transform.position - new Vector3(0f, colliderObject.bounds.extents.y + jumpCheckOffset);
        if(CanMoveToTarget()){
            // Direction Calculation
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed;
            // Movement
            rb.velocity = force;

            // Next Waypoint
            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

            // Direction Graphics Handling
            if (directionLookEnabled)
            {
                if (rb.velocity.x > 0.05f)
                {
                    transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
                else if (rb.velocity.x < -0.05f)
                {
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
        }
        else if(!animator.GetBool(Constants.ANIM_BOOL_HURT) && !animator.GetBool(Constants.ENEMY_ANIM_BOOL_ATTACK)){
                rb.velocity = Vector2.zero;
                animator.SetBool(Constants.ENEMY_ANIM_BOOL_ATTACK, true);
        }
    }
}
