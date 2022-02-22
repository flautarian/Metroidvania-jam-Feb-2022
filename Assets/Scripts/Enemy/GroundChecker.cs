using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;
    [SerializeField]
    private LayerMask layerMask;
    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    internal bool TouchesGround(){
        RaycastHit2D groundInfo = Physics2D.Raycast(transform.position, Vector2.down, 2f, layerMask);
        return groundInfo.collider;
    }

    internal bool TouchesWall(){
        return boxCollider2D.IsTouchingLayers(layerMask);
    }
}
