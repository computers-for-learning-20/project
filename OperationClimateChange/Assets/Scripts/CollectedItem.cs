using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedItem : MonoBehaviour
{

    public ParticleSystem ItemCollision;
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
            ItemCollision.Play();
            // removed the collected item
            Destroy(this.transform.gameObject);
        }
    }
}