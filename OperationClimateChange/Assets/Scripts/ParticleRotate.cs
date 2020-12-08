using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotate : MonoBehaviour
{
    public float rotateSpeed = 5.0f;
    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        this.startPos = this.transform.position;
       
    }

    // Update is called once per frame
    void FixedUpdate()  
    {
        // rotate behavior for particle spin
        transform.Rotate(0, rotateSpeed, 0, Space.Self);
    }
}
