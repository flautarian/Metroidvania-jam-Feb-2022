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
    private float jumpTimeCounter;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    [SerializeField]
    private bool grounded = false;
    [SerializeField]
    private Vector2 duckCol, jumpCol, normalCol;
    Animator animator;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
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
        rb.velocity = new Vector2( horizontal * velocity, rb.velocity.y);
    }

    private void ManagePlayerMovement(){
        horizontal = !animator.GetBool(Constants.PLAYER_ANIM_BOOL_DUCK) 
            ? Input.GetAxisRaw("Horizontal") 
            : 0f;
        vertical = Input.GetAxisRaw("Vertical");

        if(Input.GetButtonDown("Jump") && !animator.GetBool(Constants.PLAYER_ANIM_BOOL_JUMP)) {
            if(jumpTimeCounter > 0){
                animator.SetBool(Constants.PLAYER_ANIM_BOOL_DOUBLE_JUMP, true);
                jumpTimeCounter = 0;
                rb.velocity = new Vector2(rb.velocity.x, forceJump);
            }
            else if(grounded){
                animator.SetBool(Constants.PLAYER_ANIM_BOOL_JUMP, true);
                jumpTimeCounter = jumpTime;
                rb.velocity = new Vector2(rb.velocity.x, forceJump);
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
            if(jumpTimeCounter == 0) animator.SetBool(Constants.PLAYER_ANIM_BOOL_DOUBLE_JUMP, false);
        }

    }

    private void ManageAnimations(){
        animator.SetBool(Constants.PLAYER_ANIM_BOOL_RUN, horizontal != 0);
        if(horizontal != 0)spriteRenderer.flipX = horizontal < 0.0f;
        animator.SetBool(Constants.PLAYER_ANIM_BOOL_DUCK, vertical < 0.0f);
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

        Debug.Log(raycastHit.collider);
        return raycastHit.collider != null;
    }
}
