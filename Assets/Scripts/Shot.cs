using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    // false left, true right
    internal bool orientation = false;
    [SerializeField]
    internal float timeAlive;
    internal float time;

    [SerializeField]
    internal int attack;

    [SerializeField]
    internal int velocity = 7;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    internal Vector2 initialPos = Vector2.zero;

    internal void PrepareShot(){
        initialPos = transform.position;
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        if(spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        rb.velocity = new Vector2(velocity * (!orientation ? 1 : -1), 0);   
        time = timeAlive;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        spriteRenderer.flipX = orientation;
        time-=Time.deltaTime;
        if(time <= 0f)
            DisposeShot();
    }

    internal void DisposeShot(){
        GameManager.Instance.RequestAndExecuteGameObject(Constants.PARTICLE_CAST_SHOT, transform.position);
        GameManager.Instance.ReturnToPool(gameObject.name, this.gameObject);
    }
}
