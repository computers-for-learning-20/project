using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastBehavior : MonoBehaviour
{
    //public ParticleSystem BlastCollision;
    public float onscreenDelay = 4f;

    // interator for preventing multiple overlapping collisions
    private int count = 0;


    void Start()
    {
        Destroy(this.gameObject, onscreenDelay);
    }

    void OnCollisionEnter(Collision collision)
    {
       //Put collision code here
       if (collision.gameObject.name == "atmosphere" && count == 0)
        {
            //BlastCollision.Play();
            Destroy(this.transform.gameObject);
            count += 1;
        }
    }
}
