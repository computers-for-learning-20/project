using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EarthButton : MonoBehaviour
{
    public GameObject Panel;
    private Button button;
    void Start(){

        button = GetComponent<Button>();
        if(SceneManager.GetSceneByName("Lab").isLoaded){
        button.interactable = true;}
        else {
        button.interactable = false;
        }
        
    }
    public void OpenPanel(){
        if(Panel != null){
            bool isActive = Panel.activeSelf;
            Panel.SetActive(!isActive);
        }
    }
}
