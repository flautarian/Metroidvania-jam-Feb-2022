using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontal = 0.0f;
    private float vertical = 0.0f;

    [SerializeField] 
    private LayerMask platformLayerMask;

    [SerializeField]
    int velocity;
    [SerializeField]
    private float forceJump;
    [SerializeField]
    private float jumpTime;
    internal float jumpTimeCounter;

    [SerializeField]
    private float slashForce;
    [SerializeField]
    private float slashForceValue;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    [SerializeField]
    private bool grounded = false;
    [SerializeField]
    private Vector2 duckCol, jumpCol, normalCol;
    Animator animator;
    SpriteRenderer spriteRenderer;

    [SerializeField]
    private BoxCollider2D leftAttackCollider,rightAttackCollider;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        leftAttackCollider.enabled = false;
        rightAttackCollider.enabled = false;
        if(GameManager.Instance.nextPlayerPosition != Vector2.zero)
            transform.position = GameManager.Instance.nextPlayerPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.IsIngame())
            ManagePlayerMovement();
        ManageAnimations();
    }

    private void FixedUpdate() {
        if(GameManager.Instance.IsIngame()){
            grounded = IsGrounded();
            animator.SetBool(Constants.PLAYER_ANIM_BOOL_FALL, !grounded);
            if(slashForceValue != 0){
                slashForceValue = Mathf.Abs(slashForceValue) < 0.1f ? 0 : Mathf.Lerp(slashForceValue, 0, 8f * Time.deltaTime);
            }
            if(!animator.GetBool(Constants.ANIM_BOOL_HURT)) rb.velocity = new Vector2( (horizontal * velocity) + slashForceValue, rb.velocity.y);
        }
        else{
            horizontal = 0;
            vertical =0;
            rb.velocity = Vector2.zero;
        }
    }

    private void ManagePlayerMovement(){
        horizontal = CanMove() 
            ? Input.GetAxisRaw("Horizontal") 
            : 0f;
        vertical = Input.GetAxisRaw("Vertical");

        if(Input.GetButtonDown("Submit"))
            GameManager.Instance.ChangeState(GameManager.GameState.PAUSE);

        if(vertical >= 0f){
            // de pie
            if(Input.GetButtonDown("Jump") 
                && !animator.GetBool(Constants.PLAYER_ANIM_BOOL_JUMP)) {
                if(!grounded && jumpTimeCounter > 0 && GameManager.Instance.CanDoubleJump()){
                    animator.SetBool(Constants.PLAYER_ANIM_BOOL_DOUBLE_JUMP, true);
                    jumpTimeCounter = 0;
                    rb.velocity = new Vector2(rb.velocity.x, forceJump + 2);
                }
                else if(grounded){
                    animator.SetBool(Constants.PLAYER_ANIM_BOOL_JUMP, true);
                    jumpTimeCounter = jumpTime;
                    rb.velocity = new Vector2(rb.velocity.x, forceJump);
                }
            }

            if(Input.GetButtonDown("Fire1")){
                if(grounded && !animator.GetBool(Constants.PLAYER_ANIM_BOOL_ATTACK)){
                    animator.SetBool(Constants.PLAYER_ANIM_BOOL_ATTACK, true);    
                }
                else if(!grounded && !animator.GetBool(Constants.PLAYER_ANIM_BOOL_AIR_ATTACK)){
                    animator.SetBool(Constants.PLAYER_ANIM_BOOL_AIR_ATTACK, true);    
                }
            }
        }
        else{
            // agachado
            if(Input.GetButtonDown("Fire1") && GameManager.Instance.CanDuckSlash() && !animator.GetBool(Constants.PLAYER_ANIM_BOOL_DUCK_SLASH)){
                animator.SetBool(Constants.PLAYER_ANIM_BOOL_DUCK_SLASH, true);
            } 
        }

        if(Input.GetButton("Jump") && animator.GetBool(Constants.PLAYER_ANIM_BOOL_JUMP)){
            if(jumpTimeCounter > 0){
                rb.velocity = new Vector2(rb.velocity.x, forceJump);
                jumpTimeCounter -= Time.deltaTime;
            }
            else animator.SetBool(Constants.PLAYER_ANIM_BOOL_JUMP, false);
        }

        if(Input.GetButtonUp("Jump")){
            animator.SetBool(Constants.PLAYER_ANIM_BOOL_JUMP, false);
            jumpTimeCounter = grounded ? 0 : jumpTimeCounter;
            if(jumpTimeCounter == 0) animator.SetBool(Constants.PLAYER_ANIM_BOOL_DOUBLE_JUMP, false);
        }

    }

    private void ManageAnimations(){
        animator.SetBool(Constants.PLAYER_ANIM_BOOL_RUN, horizontal != 0);
        if(Input.GetAxisRaw("Horizontal") != 0 && CanMove())spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") < 0.0f;
        animator.SetBool(Constants.PLAYER_ANIM_BOOL_DUCK, vertical < 0.0f);
    }

    private bool CanMove(){
        return !animator.GetBool(Constants.PLAYER_ANIM_BOOL_DUCK) 
            && !animator.GetBool(Constants.PLAYER_ANIM_BOOL_ATTACK)
            && !animator.GetBool(Constants.ANIM_BOOL_HURT);
    }
    private void OnCollisionEnter2D(Collision2D other) {
        if(Constants.TAG_ENEMY.Equals(other.gameObject.tag)){
            if(other.gameObject.TryGetComponent<EnemyController>(out EnemyController e)){
                HurtPlayer(e.attack, other.transform.position);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        switch(other.gameObject.tag){
            case Constants.TAG_ENEMY_SHOOT:
                if(other.gameObject.TryGetComponent<Shot>(out Shot e)){
                    HurtPlayer(e.attack, other.transform.position);
                    e.DisposeShot();
                }
                break;
            case Constants.TAG_MINI_HEARTH:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_PLAYER_TAKES_LIFE, transform.position);
                GameManager.Instance.ModifyLife(5);
                var mHeart = other.transform.parent.name;
                GameManager.Instance.ReturnToPool(mHeart, other.transform.parent.gameObject);
                break;
            case Constants.TAG_HEARTH:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_PLAYER_TAKES_LIFE, transform.position);
                GameManager.Instance.ModifyLife(10);
                var heart = other.transform.parent.name;
                GameManager.Instance.ReturnToPool(heart, other.transform.parent.gameObject);
                break;
            case Constants.TAG_GOLD_COIN:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_PLAYER_TAKES_LIFE, transform.position);
                GameManager.Instance.UpdateCoins(10);
                var gcoin = other.transform.parent.name;
                GameManager.Instance.ReturnToPool(gcoin, other.transform.parent.gameObject);
                break;
            case Constants.TAG_PLATE_COIN:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_PLAYER_TAKES_LIFE, transform.position);
                GameManager.Instance.UpdateCoins(5);
                var pcoin = other.transform.parent.name;
                GameManager.Instance.ReturnToPool(pcoin, other.transform.parent.gameObject);
                break;
            case Constants.TAG_BRONZE_COIN:
                GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_PLAYER_TAKES_LIFE, transform.position);
                GameManager.Instance.UpdateCoins(1);
                var bcoin = other.transform.parent.name;
                GameManager.Instance.ReturnToPool(bcoin, other.transform.parent.gameObject);
                break;
        };
    }

    internal void HurtPlayer(int enemyAttack, Vector2 otherPosition){
        if(!animator.GetBool(Constants.ANIM_BOOL_HURT)){
            GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_PLAYER_HIT, transform.position);
            // player lives after hit?
            if(GameManager.Instance.ModifyLife(-enemyAttack)){
                animator.SetBool(Constants.ANIM_BOOL_HURT, true);
                rb.AddForce( new Vector2(otherPosition.x > transform.position.x ? -5 : 5, 10), ForceMode2D.Impulse );
            }
            else animator.SetBool(Constants.ANIM_BOOL_DIE, true);
        }
    }

    private void SetAttackColliderState(int state){
        if(state == 1){
            if(spriteRenderer.flipX)
                leftAttackCollider.enabled = true;
            else 
                rightAttackCollider.enabled = true; 
        }
        else{
            leftAttackCollider.enabled = false;
            rightAttackCollider.enabled = false;
        }
    }

    private bool IsGrounded() {
        float extraHeightText = 0.05f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, extraHeightText, platformLayerMask);

        Color rayColor;
        if (raycastHit.collider != null) {
            rayColor = Color.green;
        } else {
            rayColor = Color.red;
        }
        Debug.DrawRay(col.bounds.center + new Vector3(col.bounds.extents.x, 0), Vector2.down * (col.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(col.bounds.center - new Vector3(col.bounds.extents.x, 0), Vector2.down * (col.bounds.extents.y + extraHeightText), rayColor);
        Debug.DrawRay(col.bounds.center - new Vector3(col.bounds.extents.x, col.bounds.extents.y + extraHeightText), Vector2.right * (col.bounds.extents.x * 2f), rayColor);

        //Debug.Log(raycastHit.collider);
        return raycastHit.collider != null;
    }

    private void DuckSlash(){
        slashForceValue = slashForce * (spriteRenderer.flipX ? -1 : 1);
    }
}
