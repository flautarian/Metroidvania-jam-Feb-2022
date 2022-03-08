using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SignController : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;

    private Animator animator;

    [SerializeField]
    private DialogObject signDialog;
    public static event Action<DialogObject> OnOpenDialogue = delegate { };

    private void Start() {
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    private void Update() {
        if(animator.GetBool("playerDetected")){
            if(Input.GetButtonDown("Fire1") && GameManager.Instance.IsIngame()){
                OnOpenDialogue(signDialog);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(Constants.TAG_PLAYER.Equals(other.gameObject.tag))
            animator.SetBool("playerDetected", true);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(Constants.TAG_PLAYER.Equals(other.gameObject.tag))
            animator.SetBool("playerDetected", false);
    }
}
