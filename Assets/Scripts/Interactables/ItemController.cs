using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    private void Awake() {
        rb.velocity = new Vector2(Random.Range(-4, 4), Random.Range(4, 7));
    }
}
