using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolItem : MonoBehaviour
{
    ParticleSystem particle;
    public bool value = false;
    void Start()
    {
        particle = GetComponent<ParticleSystem>();
        particle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        value = particle.isStopped;
        if(particle.isStopped)
            GameManager.Instance.ReturnToPool(gameObject.name, this.gameObject);
    }
}
