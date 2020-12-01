using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedItem : MonoBehaviour
{

    //public ParticleSystem GoalParticles;
    //public ParticleSystem GoalCollision;
    public GameBehavior gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager")
            .GetComponent<GameBehavior>();

    }
    void OnCollisionEnter(Collision collision)
    {
        //Put collision code here
        if (collision.gameObject.name == "Spy")
        {
            // triggers the burst of white
            // particles and stops the small yellow ones
            //GoalCollision.Play();
            //Destroy(GoalParticles);

            //Update goals colected
            //gameManager.Goals += 1;

            // removed the collected goal
            Destroy(this.transform.gameObject);
        }
    }
}