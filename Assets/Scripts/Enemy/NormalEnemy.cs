using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NormalEnemy : EnemyController
{

    public enum EnemyTypes{
        ENEMYTHROWTOTARGET,
        ENEMYFOLLOWTARGET
    }

    [Header("Enemy parameters")]

    [SerializeField]
    private PlayerChecker playerChecker;
    private bool goRight = true;
    private bool playerDetected = false;
    [SerializeField]
    private EnemyTypes enemyType = EnemyTypes.ENEMYFOLLOWTARGET;
    private RaycastHit2D isGrounded;
    private Collider2D colliderObject;

    [SerializeField]
    private int minVectorPathsUntilAttack = 0;
    public override void StartSpecificClasses(){
        seeker = GetComponent<Seeker>();
        colliderObject = GetComponent<Collider2D>();
    }
    public override void ManageWalkRoutine() {
        if(mustPatrol){
            //if(playerChecker.playerDetected != null)
            //    playerChecker.playerDetected = null;
            // PATROL
            animator.SetBool(Constants.ANIM_BOOL_RUN, true);
            if(!groundChecker.TouchesGround()){
                enemyTransform.eulerAngles = new Vector3(0, goRight ? -180 : 0, 0);
                goRight = !goRight;
            }
            if(!animator.GetBool(Constants.ANIM_BOOL_HURT)){
                enemyTransform.position = new Vector2( enemyTransform.position.x + ((goRight ? 1 : -1) * speed * Time.deltaTime) , enemyTransform.transform.position.y);
                rb.velocity = Vector2.zero;
            }
        }
        if(playerChecker.playerDetected != null){
            // AI
            if(mustPatrol)
                InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
            mustPatrol = false;
            target = playerChecker.playerDetected;
            switch(enemyType){
                case EnemyTypes.ENEMYFOLLOWTARGET:
                    followEnabled = true;
                    if (TargetInDistance() && followEnabled)
                    {
                        PathFollow();
                    }
                break;
                case EnemyTypes.ENEMYTHROWTOTARGET:
                    if(!animator.GetBool(Constants.ENEMY_ANIM_BOOL_ATTACK))
                    animator.SetBool(Constants.ENEMY_ANIM_BOOL_ATTACK, true);
                break;
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
                return;
            }
            BeginHurt();
            playerChecker.playerDetected = t;
            if(alterable){
                rb.velocity = Vector2.zero;
                rb.AddForce(new Vector2(t.position.x > transform.position.x ? -7 : 7, 10), ForceMode2D.Impulse);
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
        isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.01f);
        // Direction Calculation
        //Debug.Log("Count: " + path.vectorPath.Count + " in waypoint " + currentWaypoint);

        if(CanMoveToTarget()){
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * speed;
            //Debug.LogFormat("X: {0}, Y: {1}", direction.x, direction.y);
            // Jump
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
        else if(!animator.GetBool(Constants.ANIM_BOOL_HURT) && !animator.GetBool(Constants.ENEMY_ANIM_BOOL_ATTACK)){
                rb.velocity = Vector2.zero;
                animator.SetBool(Constants.ENEMY_ANIM_BOOL_ATTACK, true);
        }
    }

}
