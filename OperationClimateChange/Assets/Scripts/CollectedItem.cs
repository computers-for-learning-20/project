using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedItem : MonoBehaviour
{

    public ParticleSystem ItemCollision;
    public GameBehavior gameManager;

    // interator for preventing multiple overlapping collisions
    private int count = 0;


    void Start()
    {
        gameManager = GameObject.Find("GameManager")
            .GetComponent<GameBehavior>();

    }
    void OnCollisionEnter(Collision collision)
    {
        //Collision code
        if (collision.gameObject.name == "Spy" && count == 0)
        {
            ItemCollision.Play();
            count += 1;

            // removed the collected item
            Destroy(this.transform.gameObject);
        }
    }
}