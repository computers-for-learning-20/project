using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Atmosphere : MonoBehaviour
{
    MeshRenderer mesh;
    public GameBehavior gameManager;
    public ParticleSystem ParticleCollision;

    private List<string> particles
        = new List<string> { "methane_2(Clone)", "h2o_2(Clone)", "o2_2(Clone)",
            "n2_2(Clone)", "argon_2(Clone)", "co2_2(Clone)"};

    // interator for preventing multiple overlapping collisions
    public int count = 0;

    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material.color = Color.white;
        gameManager = GameObject.Find("GameManager")
            .GetComponent<GameBehavior>();
    }

    void OnCollisionEnter(Collision collision){
        //Change Earth's balance if it collides with any particle
        if (particles.Contains(collision.gameObject.name) && count == 0)
        {
            switch (collision.gameObject.name)
            {
                // issue note: since multiple collisions on moving
                // targets were causing multiple inventory adds,
                // limiting to one of each particle type.

                case ("o2_2(Clone)"):
                    gameManager.BalanceEarth +=10;
                    break;
                case ("co2_2(Clone)"):
                    gameManager.BalanceEarth -=15 ;
                    break;
                case ("h2o_2(Clone)"):
                    gameManager.BalanceEarth -=10;
                    break;
                case ("n2_2(Clone)"):
                    gameManager.BalanceEarth +=15;
                    break;
                case ("methane_2(Clone)"):
                    gameManager.BalanceEarth -=20;
                    break;
                case ("argon_2(Clone)"):
                    gameManager.BalanceEarth +=10;
                    break;
            }
            change_athmosphere_colors();
            ParticleCollision.Play();
            count += 1;
        }
    }

    void change_athmosphere_colors(){
        mesh.material.color = gameManager.EarthGradient.Evaluate(gameManager.BalanceEarth/100f);
        float metallic_val = gameManager.BalanceEarth/100f;

        if(metallic_val < 0.33){
            metallic_val = 1- metallic_val;
        }
        else if (metallic_val > (float) 0.43 && metallic_val < (float) 0.56){
            metallic_val = (float) 0.14;
        }
        mesh.material.SetFloat("_Metallic", metallic_val);
    }
    
}
