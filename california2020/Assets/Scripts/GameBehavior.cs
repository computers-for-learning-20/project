using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameBehavior : MonoBehaviour
{
    string CurrentYear = "2100";
    string CurrentPlace = "LAB";

    public Text PlaceDisplay;
    public Text YearDisplay;

    public Slider YearSlider;
    public Toggle Place01;
    public Toggle Place04;

    public Color activeColor;
    public Color inactiveColor;

    public Button exploreButton;
    public Text exploreText;

    public Button warpButton;
    public Text warpText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((CurrentPlace == "LAB"
            && CurrentYear == "2020")
            || (CurrentPlace == "CA"
            && CurrentYear == "2100")
            )
        {
            exploreButton.interactable = false;
            exploreText.color = inactiveColor;

            warpButton.interactable = false;
            warpText.color = inactiveColor;
        }
        else
        {
            exploreButton.interactable = true;
            exploreText.color = activeColor;

            warpButton.interactable = true;
            warpText.color = activeColor;
        }
    }

    public void GoToLevel()
    {
        string LevelName = "TimeTravelInterface";

        if (CurrentYear == "2100"
            && CurrentPlace == "LAB")
        {
            LevelName = "Lab";
        }
        else if (CurrentYear == "2020"
            && CurrentPlace == "CA")
        {
            LevelName = "ca2020_lvl1";
        }

        SceneManager.LoadScene(LevelName);

        if (LevelName == "TimeTravelInterface")
        {
            if (CurrentYear == "2020")
            {
                SceneManager.LoadScene("2020places",
                    LoadSceneMode.Additive);
                SceneManager.LoadScene("CA_current",
                    LoadSceneMode.Additive);
            }
            else if (CurrentYear == "2100")
            {
                SceneManager.LoadScene("2100places",
                    LoadSceneMode.Additive);
                SceneManager.LoadScene("LAB_current",
                    LoadSceneMode.Additive);
            }
        }

        Time.timeScale = 1f;
    }

    public void ChangePlace()
    {
        Debug.Log("change place method triggered!");

        if (SceneManager.GetSceneByName("2020places").isLoaded)
        {
            SceneManager.UnloadSceneAsync("2020places");
            Debug.Log("unloading 2020");
        }

        if (SceneManager.GetSceneByName("2100places").isLoaded)
        {
            SceneManager.UnloadSceneAsync("2100places");
            Debug.Log("unloading 2100");
        }

        if (CurrentPlace == "LAB")
        {
            if (Place01.isOn && Place04.isOn)
            { Place01.isOn = false; }

            else if (Place04.isOn == false
                && Place01.isOn == false)
            {
                Place04.isOn = true;
            }
            else if (Place04.isOn && Place01.isOn == false)
            {
                CurrentPlace = "CA";
                PlaceDisplay.text = "California";

                SceneManager.LoadScene("2020places",
                        LoadSceneMode.Additive);
                Debug.Log("loading 2020");
                
            }
        }
        
        else if (CurrentPlace == "CA")
        {
            if (Place04.isOn && Place01.isOn)
            { Place04.isOn = false; }

            else if (Place01.isOn == false
                && Place04.isOn == false)
            { Place01.isOn = true; }

            else if (Place01.isOn && Place04.isOn == false)
            {
                CurrentPlace = "LAB";
                PlaceDisplay.text = "Rachel's Lab";

                SceneManager.LoadScene("2100places",
                    LoadSceneMode.Additive);
                Debug.Log("loading 2100");
            }

        }
    }

    public void ChangeYear()
    {
        string Year;

        if (YearSlider.value == 0)
        {
            Year = "2020";
        }
        else
        {
            Year = "2100";
        }

        CurrentYear = Year;
        YearDisplay.text = Year;
    }

    public void warp()
    {
        Debug.Log("warp animation");
    }
}