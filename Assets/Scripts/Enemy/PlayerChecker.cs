using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChecker : MonoBehaviour
{
    private CircleCollider2D circleCollider2D;

    internal Transform playerDetected = null;
    void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(Constants.TAG_PLAYER.Equals(other.gameObject.tag))
            playerDetected = other.transform;
    }

}
