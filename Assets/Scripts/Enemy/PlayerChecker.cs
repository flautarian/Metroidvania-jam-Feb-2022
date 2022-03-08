using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChecker : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;

    [SerializeField]
    internal bool canUnFollow = false;

    internal Transform playerDetected = null;
    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(Constants.TAG_PLAYER.Equals(other.gameObject.tag))
            playerDetected = other.transform;
    }
    void OnTriggerExit2D(Collider2D other) {
        if(Constants.TAG_PLAYER.Equals(other.gameObject.tag) && canUnFollow){
            Debug.Log("Out trigger");
            playerDetected = null;
        }
    }

    public void StartShot(){
        var parent = transform.parent;
        if(TryGetComponent<EnemyController>(out EnemyController enemy)){
            enemy.Shot();
        }
    }

}
