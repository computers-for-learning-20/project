using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectParticle : MonoBehaviour
{
    public GameBehavior gameManager;
    public GameObject methane;
    public GameObject h2o;
    public GameObject o2;
    public GameObject n2;
    public GameObject argon;
    public GameObject co2;

    public float blastSpeed = 50f;
    public Atmosphere atm;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    public void Project_particleItems(string name){
        
        GameObject particle_item = o2;
        switch (name)
            {
                case ("o2"):
                    if(gameManager.O2 == 0){return;}
                    particle_item = o2;
                    gameManager.O2 = 0;
                    break;
                case ("co2"):
                    if(gameManager.CO2 == 0){return;}
                    particle_item = co2;
                    gameManager.CO2 = 0;
                    break;
                case ("h2o"):
                    if(gameManager.H2O == 0){return;}
                    particle_item = h2o;
                    gameManager.H2O = 0;
                    break;
                case ("n2"):
                    if(gameManager.N2 == 0){return;}
                    particle_item = n2;
                    gameManager.N2 = 0;
                    break;
                case ("methane"):
                    if(gameManager.Methane == 0){return;}
                    particle_item = methane;
                    gameManager.Methane = 0;
                    break;
                case ("argon"):
                    if(gameManager.Argon == 0){return;}
                    particle_item = argon;
                    gameManager.Argon = 0;
                    break;
            }

        GameObject newParticle_item = Instantiate(particle_item, 
                            this.transform.position + new Vector3(1, 0, 0),
                            this.transform.rotation) as GameObject;

        Rigidbody particleRB = newParticle_item.GetComponent<Rigidbody>();
        particleRB.velocity = this.transform.forward * blastSpeed;

        atm = GameObject.Find("atmosphere").GetComponent<Atmosphere>();
        atm.count = 0;
    }

}
