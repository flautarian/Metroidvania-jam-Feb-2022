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
    internal int maxLife;
    [SerializeField]
    internal int currentLife;

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
        currentLife = maxLife;
    }

    // Update is called once per frame
    void Update()
    {
        ManagePlayerMovement();
        ManageAnimations();
    }

    private void FixedUpdate() {
        grounded = IsGrounded();
        animator.SetBool(Constants.PLAYER_ANIM_BOOL_FALL, !grounded);
        if(slashForceValue != 0){
            slashForceValue = Mathf.Abs(slashForceValue) < 0.1f ? 0 : Mathf.Lerp(slashForceValue, 0, 8f * Time.deltaTime);
        }
        if(!animator.GetBool(Constants.ANIM_BOOL_HURT)) rb.velocity = new Vector2( (horizontal * velocity) + slashForceValue, rb.velocity.y);
    }

    private void ManagePlayerMovement(){
        horizontal = CanMove() 
            ? Input.GetAxisRaw("Horizontal") 
            : 0f;
        vertical = Input.GetAxisRaw("Vertical");
        if(vertical >= 0f){
            // de pie
            if(Input.GetButtonDown("Jump") 
                && !animator.GetBool(Constants.PLAYER_ANIM_BOOL_JUMP)) {
                if(!grounded && jumpTimeCounter > 0){
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
            if(Input.GetButtonDown("Fire1") && !animator.GetBool(Constants.PLAYER_ANIM_BOOL_DUCK_SLASH)){
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
        if(Constants.TAG_ENEMY.Equals(other.gameObject.tag) &&
            !animator.GetBool(Constants.ANIM_BOOL_HURT)){
            if(other.gameObject.TryGetComponent<EnemyController>(out EnemyController e)){
                currentLife -= e.attack;
                if(currentLife <= 0)
                    animator.SetBool(Constants.ANIM_BOOL_DIE, true);
                else{
                    animator.SetBool(Constants.ANIM_BOOL_HURT, true);
                    rb.AddForce( new Vector2(other.transform.position.x > transform.position.x ? -5 : 5, 10), ForceMode2D.Impulse );
                }
                
            }
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
