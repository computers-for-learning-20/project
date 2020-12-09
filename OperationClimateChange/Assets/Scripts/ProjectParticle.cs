using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectParticle : MonoBehaviour
{
    public GameObject methane;
    public GameObject h2o;
    public GameObject o2;
    public GameObject n2;
    public GameObject argon;
    public GameObject co2;

    public float blastSpeed = 50f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    
    void Project_particleItems(string name){
        
        GameObject particle_item = o2;
        switch (name)
            {
                case ("o2"):
                    particle_item = o2;
                    break;
                case ("co2"):
                    particle_item = co2;
                    break;
                case ("h2o"):
                    particle_item = h2o;
                    break;
                case ("n2"):
                    particle_item = n2;
                    break;
                case ("methane"):
                    particle_item = methane;
                    break;
                case ("argon"):
                    particle_item = argon;
                    break;
            }
        GameObject newParticle_item = Instantiate(particle_item, 
                            this.transform.position + new Vector3(1, 0, 0),
                            this.transform.rotation) as GameObject;
        Rigidbody particleRB = newParticle_item.GetComponent<Rigidbody>();
        particleRB.velocity = this.transform.forward * blastSpeed; 
    }

}
