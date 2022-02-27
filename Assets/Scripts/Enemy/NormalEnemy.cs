using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NormalEnemy : EnemyController
{

    [Header("Enemy parameters")]

    [SerializeField]
    GroundChecker groundChecker;

    [SerializeField]
    private PlayerChecker playerChecker;

    private bool goRight = true;

    private bool playerDetected = false;

    [SerializeField]
    private Transform enemyTransform;

    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;

    [Header("Physics")]
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public int jumpNodeListMinRequirement = 5;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;

    [Header("Other")]
    private Path path;
    private int currentWaypoint = 0;
    RaycastHit2D isGrounded;
    Seeker seeker;

    public override void StartSpecificClasses(){
        seeker = GetComponent<Seeker>();
    }

    public override void ManageWalkRoutine() {
        if(mustPatrol){
            // PATROL
            animator.SetBool(Constants.ANIM_BOOL_RUN, true);
            if(!groundChecker.TouchesGround()){
                enemyTransform.eulerAngles = new Vector3(0, goRight ? -180 : 0, 0);
                goRight = !goRight;
            }
            if(!animator.GetBool(Constants.ANIM_BOOL_HURT))
                enemyTransform.position = new Vector2( enemyTransform.position.x + ((goRight ? 1 : -1) * speed * Time.deltaTime) , enemyTransform.transform.position.y);
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
        
    }

    public override void ManageHurtFromPlayer(Transform t)
    {
        if(!animator.GetBool(Constants.ANIM_BOOL_HURT)){
            BeginHurt();
            life -= GameManager.Instance.GetPlayerTotalAttack();
            if(alterable){
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(t.position.x > transform.position.x ? -7 : 7, 10), ForceMode2D.Impulse);
                playerChecker.playerDetected = t;
            }
        }
    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    private void PathFollow()
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
        Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.01f);
        
        // Direction Calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed;

        // Jump
        if(!animator.GetBool(Constants.ANIM_BOOL_HURT)){
            if (jumpEnabled && isGrounded)
            {
                if (direction.y > jumpNodeHeightRequirement && path.vectorPath.Count > jumpNodeListMinRequirement)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpModifier);
                }
            }
            // Movement
            rb.velocity = new Vector2(force.x, rb.velocity.y);

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

    }

    private bool TargetInDistance()
    {
        if(target != null){
            return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
        }
        return false;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

}
