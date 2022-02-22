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

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(Constants.TAG_PLAYER.Equals(other.gameObject.tag))
            playerDetected = other.transform;
    }
}
