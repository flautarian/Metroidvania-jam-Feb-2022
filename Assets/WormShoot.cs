using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormShoot : MonoBehaviour
{
    // false left, true right
    internal bool orientation = false;
    [SerializeField]
    internal float timeAlive;
    internal float time;


    [SerializeField]
    internal int attack;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    internal Vector2 initialPos = Vector2.zero;

    internal void PrepareShoot(){
        initialPos = transform.position;
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        rb.velocity = new Vector2(7 * (!orientation ? 1 : -1), 0);   
        time = timeAlive;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        spriteRenderer.flipX = orientation;
        time-=Time.deltaTime;
        if(time <= 0f)
            DisposeShoot();
    }

    private void OnTriggerEnter(Collider other) {
        DisposeShoot();
    }

    internal void DisposeShoot(){
        transform.position = initialPos;
        this.gameObject.SetActive(false);
    }
}
