using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Atmosphere : MonoBehaviour
{
    MeshRenderer mesh;
    public GameBehavior gameManager;
    void Start()
    {
        mesh = GetComponent<MeshRenderer>();
        mesh.material.color = Color.white;
        gameManager = GameObject.Find("GameManager")
            .GetComponent<GameBehavior>();
    }

    void OnCollisionEnter(Collision collision){
        float metallic_val;
        //Decrease Earth's health if it collides with Methane
        if (collision.gameObject.name == "Spy")
        {
            gameManager.BalanceEarth -=10 ;
            mesh.material.color = gameManager.EarthBalanceColor.color;
            metallic_val = gameManager.EarthBalanceSlider.normalizedValue;
            if(metallic_val < 0.33){
                metallic_val = 1- metallic_val;
            }
            else if (metallic_val > (float) 0.43 && metallic_val < (float) 0.56){
                metallic_val = (float) 0.14;
            }
            mesh.material.SetFloat("_Metallic", metallic_val);
        }
    }

    
}
