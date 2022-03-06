using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float length, height, startPos, startPosY;
    public GameObject cam;
    public float parallaxEffect, parallaxYEffect;

    private Vector3 currentPos = Vector3.zero;
    void Start()
    {
        startPos = transform.position.x;
        startPosY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        height = GetComponent<SpriteRenderer>().bounds.size.y;
        cam = Camera.main.gameObject;
    }

    private void FixedUpdate() {
        UpdateCurrentPos(cam.transform.position.x * parallaxEffect, cam.transform.position.y * parallaxYEffect, cam.transform.position.x * (1 - parallaxEffect));
        transform.position = currentPos;
    }

    private void UpdateCurrentPos(float dist, float distY, float temp){
        currentPos.x = startPos + dist;
        currentPos.y = startPosY + distY;
        currentPos.z = transform.position.z;
        
        if(temp > startPos + length) startPos += length;
        else if(temp < startPos - length) startPos -= length;
    }
}
