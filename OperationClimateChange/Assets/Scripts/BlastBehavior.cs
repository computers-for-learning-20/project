using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastBehavior : MonoBehaviour
{
    //public ParticleSystem BlastCollision;
    public float onscreenDelay = 4f;

    void Start()
    {
        Destroy(this.gameObject, onscreenDelay);
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision collision)
    {
       //Put collision code here
       if (collision.gameObject.name == "atmosphere")
        {

            //BlastCollision.Play();
            Destroy(this.transform.gameObject);
        }
    }
}
