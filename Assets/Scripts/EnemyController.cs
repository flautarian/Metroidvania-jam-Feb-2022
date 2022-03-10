using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public abstract class EnemyController : MonoBehaviour
{

    public enum EnemyTypes{
        ENEMYTHROWTOTARGET,
        ENEMYFOLLOWTARGET,
        STATICTARGET,
        STATICTHROWTARGET
    }
    public enum DropOption{
        NOTHING, BRONZECOIN, PLATECOIN, GOLDCOIN, MINIHEART, HEART, VIAL
    }
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
    [SerializeField]
    internal EnemyTypes enemyType = EnemyTypes.STATICTARGET;
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

    [SerializeField]
    internal DropOption[] dropOptions;
    
    [SerializeField]
    internal int dropQuantity;
    
    [Header("Other")]
    internal Path path;
    internal int currentWaypoint = 0;
    internal bool jumpEnabled = false;
    internal bool directionLookEnabled = true;
    
    [SerializeField]
    internal string shotPrefabNameAndPath;

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

    public void Shot(){
        if(EnemyTypes.ENEMYTHROWTOTARGET.Equals(enemyType) ||
            EnemyTypes.STATICTHROWTARGET.Equals(enemyType)){
            GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_CAST_SHOT, transform.position);
            var shot = GameManager.Instance.RequestAndExecuteGameObject(shotPrefabNameAndPath, transform.position);
            if(shot != null && shot.TryGetComponent<Shot>(out Shot s)){
                s.orientation = transform.eulerAngles.y != 0;
                s.PrepareShot();
            }
        }
    }

    private void OnDisable() {
        // TODO: esto aqui produce un fallo, estudiar poner en otra funcion
        if(life <= 0){
            GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_ENEMY_DIE, transform.position);
            InstantiateItemDrop();
        }
        //TODO: instantiate item drop here!
    }

    private void InstantiateItemDrop(){
        for(int i =0; i < dropQuantity; i++){
            GenerateRandomItemDrop();
        }
    }

    private void GenerateRandomItemDrop(){
        int result = Random.Range(0, dropOptions.Length-1);
        switch(dropOptions[result]){
            case DropOption.NOTHING:
            break;
            case DropOption.HEART:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PREFAB_HEARTH, transform.position);
            break;
            case DropOption.MINIHEART:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PREFAB_MINI_HEARTH, transform.position);
            break;
            case DropOption.GOLDCOIN:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PREFAB_PLATE_COIN, transform.position);
            break;
            case DropOption.PLATECOIN:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PREFAB_PLATE_COIN, transform.position);
            break;
            case DropOption.BRONZECOIN:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PREFAB_BRONZE_COIN, transform.position);
            break;
            case DropOption.VIAL:
            break;
        }
    }

    internal void BeginHurt(){
        animator.SetBool(Constants.ANIM_BOOL_HURT, true);
        GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_PLAYER_HIT, transform.position);
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
            && (mustPatrol || !EnemyIsCloserToTarget());
    }

}
