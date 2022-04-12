using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public enum ItemSpecialType{
        LIFE, FORCE, SLASH, DJUMP, NOTHING
    }
    [SerializeField]
    private Rigidbody2D rb;

    private float randomX, timer;
    private Vector2 velocity;

    [SerializeField]
    private DialogObject itemDialog;

    [SerializeField]
    private int id;
    [SerializeField]
    private ItemSpecialType itemSpecialType;

    [SerializeField]
    private bool affectedByGravity = true;
    public static event Action<DialogObject> OnOpenDialogue = delegate { };

    private void Awake() {
        velocity = new Vector2(UnityEngine.Random.Range(4, 7), UnityEngine.Random.Range(4, 7));
    }
    private void Start() {
        StartCoroutine(VerifyExistenceItem());
        if(!affectedByGravity){
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private IEnumerator VerifyExistenceItem(){
        yield return new WaitUntil(() => GameManager.Instance.saveGame == null);
        var deleteObject = false;
        if(itemSpecialType == ItemSpecialType.LIFE && GameManager.Instance.IsHearthTaken(id))
            deleteObject = true;
        else if(itemSpecialType == ItemSpecialType.FORCE && GameManager.Instance.IsForceTaken(id))
            deleteObject = true;
        else if(itemSpecialType == ItemSpecialType.SLASH && GameManager.Instance.CanDuckSlash())
            deleteObject = true;
        else if(itemSpecialType == ItemSpecialType.DJUMP && GameManager.Instance.CanDoubleJump())
            deleteObject = true;
        if(deleteObject){
            Debug.Log("asdasda");
            DestroyItem();
        }
    }

    private void DestroyItem(){
        var parent = transform.parent.gameObject;
        parent.SetActive(false);
        Destroy(parent);
    }

    private void FixedUpdate() {
        if(affectedByGravity){
            if(Time.time > timer + 0.5f){
                if(Mathf.Abs(rb.velocity.y) < 0.1f){
                    rb.velocity = new Vector2(0,0);
                    timer =99999;
                }
                else{                
                    velocity.x *= -1;
                    velocity.y = rb.velocity.y;
                    rb.velocity = velocity;
                    timer = Time.time;
                }
            }
        }
    }

    public void OnCatchItem(){
        if(itemDialog != null && itemDialog.Dialoge.Length > 0)
            OnOpenDialogue(itemDialog);
        switch(itemSpecialType){
            case ItemSpecialType.LIFE:
                GameManager.Instance.AddLifeItemCatch(id);
                DestroyItem();
            break;
            case ItemSpecialType.FORCE:
                GameManager.Instance.AddForceItemCatch(id);
                DestroyItem();
            break;
            case ItemSpecialType.SLASH:
                GameManager.Instance.EnableDuckSlash();
                DestroyItem();
            break;
            case ItemSpecialType.DJUMP:
                GameManager.Instance.EnableDoubleJump();
                DestroyItem();
            break;
        };
    }
    
}
