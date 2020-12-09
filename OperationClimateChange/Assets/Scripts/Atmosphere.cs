using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Atmosphere : MonoBehaviour
{
    MeshRenderer mesh;
    public GameBehavior gameManager;
    public ParticleSystem ParticleCollision;
    private List<string> particles
        = new List<string> { "methane", "h2o", "o2",
            "n2", "argon_2", "co2"};
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material.color = Color.white;
        gameManager = GameObject.Find("GameManager")
            .GetComponent<GameBehavior>();
    }

    void OnCollisionEnter(Collision collision){
        //Change Earth's balance if it collides with any particle
        if (particles.Contains(collision.gameObject.name))
        {
            switch (collision.gameObject.name)
            {
                // issue note: since multiple collisions on moving
                // targets were causing multiple inventory adds,
                // limiting to one of each particle type.

                case ("o2"):
                    gameManager.BalanceEarth +=10 ;
                    break;
                case ("co2"):
                    gameManager.BalanceEarth -=15 ;
                    break;
                case ("h2o"):
                    gameManager.BalanceEarth -=10;
                    break;
                case ("n2"):
                    gameManager.BalanceEarth +=15;
                    break;
                case ("methane"):
                    gameManager.BalanceEarth -=20;
                    break;
                case ("argon"):
                    gameManager.BalanceEarth +=10;
                    break;
            }
            change_athmosphere_colors();
            ParticleCollision.Play();
        }
    }

    void change_athmosphere_colors(){
        mesh.material.color = gameManager.EarthBalanceColor.color;
        float metallic_val = gameManager.EarthBalanceSlider.normalizedValue;

        if(metallic_val < 0.33){
            metallic_val = 1- metallic_val;
        }
        else if (metallic_val > (float) 0.43 && metallic_val < (float) 0.56){
            metallic_val = (float) 0.14;
        }
        mesh.material.SetFloat("_Metallic", metallic_val);
    }
    
}
