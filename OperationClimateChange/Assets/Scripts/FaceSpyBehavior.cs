using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceSpyBehavior : MonoBehaviour
{
    // player location attribute
    private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        // finding the player
        target = GameObject.Find("Spy").transform;
    }

    // turn towards target.
    private void LateUpdate()
    {
        this.transform.LookAt(target);
    }
}
