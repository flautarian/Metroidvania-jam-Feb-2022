using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{
    [Header("Basic Params")]
    internal Rigidbody2D rb;

    [SerializeField]
    internal Animator animator;
    [SerializeField]
    internal float speed = 5f;

    [SerializeField]
    internal int attack =0;
    
    [SerializeField]
    internal int life =0;

    [SerializeField]
    internal bool mustPatrol = true;

    [SerializeField]
    internal bool alterable = false;

    internal BoxCollider2D col;
    
    [SerializeField]
    internal GroundChecker groundChecker;

    internal SpriteRenderer spriteRenderer;

    [Header("IA Behavior")]
    public bool followEnabled = true;
    internal Seeker seeker;
    internal Transform target;

    [SerializeField]
    internal Transform enemyTransform;

    [SerializeField]
    internal float activateDistance = 50f;

    [SerializeField]
    internal float stayAndAttackDistance = 5f;

    [SerializeField]
    internal float pathUpdateSeconds = 0.5f;

    [Header("Physics")]

    [SerializeField]
    internal float nextWaypointDistance = 3f;

    [SerializeField]
    internal float jumpNodeHeightRequirement = 0.8f;

    [SerializeField]
    internal int jumpNodeListMinRequirement = 5;

    [SerializeField]
    internal float jumpModifier = 0.3f;

    [SerializeField]
    internal float jumpCheckOffset = 0.1f;
    
    [Header("Other")]
    internal Path path;
    internal int currentWaypoint = 0;
    internal bool jumpEnabled = false;
    internal bool directionLookEnabled = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if(animator is null)
            animator = GetComponent<Animator>();
            
        seeker = GetComponent<Seeker>();
        col = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartSpecificClasses();
    }

    private void FixedUpdate() {
        ManageAttackRoutine();
        ManageWalkRoutine();
        if(animator.GetBool(Constants.ANIM_BOOL_DEAD))
            this.gameObject.SetActive(false);
    }

    public abstract void StartSpecificClasses();

    public abstract void ManageAttackRoutine();

    public abstract void ManageWalkRoutine();

    public abstract void ManageHurtFromPlayer(Transform t);

    internal void Attack(){
        animator.SetBool(Constants.ENEMY_ANIM_BOOL_ATTACK, true);
    }
    internal void BeginDead(){
        animator.SetBool(Constants.ANIM_BOOL_DIE, true);
    }

    internal void BeginHurt(){
        animator.SetBool(Constants.ANIM_BOOL_HURT, true);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(Constants.TAG_PLAYER_WEAPON.Equals(other.gameObject.tag))
            ManageHurtFromPlayer(other.transform.parent);
    }

    
    internal void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    public abstract void PathFollow();

    internal bool TargetInDistance()
    {
        var distance = Vector2.Distance(transform.position, target.transform.position);
        //Debug.LogFormat("Distance: {0}", distance);
        if(target != null){
            var result = distance < activateDistance;
            if(!result){
                mustPatrol = true;
            }
            return result;
        }
        return false;
    }

    internal bool EnemyIsCloserToTarget(){
        return Vector2.Distance(transform.position, target.transform.position) <= stayAndAttackDistance;
    }

    internal void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    internal bool CanMoveToTarget(){
        return !animator.GetBool(Constants.ANIM_BOOL_HURT) 
            && !animator.GetBool(Constants.ENEMY_ANIM_BOOL_ATTACK) 
            && !EnemyIsCloserToTarget();
    }

}
