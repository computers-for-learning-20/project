using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionButton : MonoBehaviour
{
    public GameObject Panel;
    /*
    public GameBehavior gameManager;
    void Start(){
        gameManager = GameObject.Find("GameManager")
            .GetComponent<GameBehavior>();
        if(gameManager.FirstEver == 1){
            Panel.SetActive(true);
        }
        else{
            Panel.SetActive(false);
        }
        
    }
    */
    public void OpenPanel(){
        if(Panel != null){
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}
