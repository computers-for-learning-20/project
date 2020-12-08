using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class TimeMachineBehavior : MonoBehaviour
{
    private string CurrentYear = "2100";
    private string CurrentPlace = "LAB";

    public Text PlaceDisplay;
    public Text YearDisplay;
    public Text SystemMessageDisplay;

    public Slider YearSlider;
    public Toggle Place01;
    public Toggle Place04;

    public Color activeColor;
    public Color inactiveColor;
    public Color errorColor;
    public Color okColor;

    public Button warpButton;
    public Text warpText;

    public Canvas ui;
    public GameObject screen;

    public string Now
    {
        get { return CurrentYear; }
        set { CurrentYear = value; }
    }

    public string Here
    {
        get { return CurrentPlace; }
        set { CurrentPlace = value; }
    }

    // Use this for initialization
    void Start()
    {
        LoadWhenAndWhere(CurrentPlace, CurrentYear);
        Debug.Log("start load triggered");
    }

    // Update is called once per frame
    void Update()
    {
        string messageText = ": : : : MESSAGE : : : : "
            + "All Systems Ready";

        if ((CurrentPlace == "LAB"
            && CurrentYear == "2020")
            || (CurrentPlace == "CA"
            && CurrentYear == "2100"))
        {
            warpButton.interactable = false;
            warpText.color = inactiveColor;
            SystemMessageDisplay.color = errorColor;
            messageText = ": : : : ERROR : : : : "
                + "Place not available for Year";
        }
        else
        {
            warpButton.interactable = true;
            warpText.color = activeColor;
            SystemMessageDisplay.color = okColor;
        }

        SystemMessageDisplay.text = messageText;
    }

    public void ChangePlace()
    {
        string NewPlace = CurrentPlace;

        Debug.Log("change place method triggered!");

        if (Place04.isOn)
        {
            NewPlace = "CA";
        }
        else if (Place01.isOn)
        {
            NewPlace = "LAB";
        }

        if (CurrentPlace != NewPlace)
        { LoadWhenAndWhere(NewPlace, CurrentYear); }
        
    }

    public void ChangeYear()
    {
        string Year;

        if (YearSlider.value == 0)
        { Year = "2020"; }
        else
        { Year = "2100"; }

        if (CurrentYear != Year)
        { LoadWhenAndWhere(CurrentPlace, Year); }
    }

    public void warp()
    {
        UnloadExtras();
        ui.enabled = false;
        screen.SetActive(false);

        Debug.Log("warp animation");
        SceneManager.LoadScene("WarpSequence", LoadSceneMode.Additive);
    }

    private void LoadWhenAndWhere(string Place, string Year)
    {
        bool PreWarp = true;

        if (CurrentPlace == Place && CurrentYear == Year)
        { PreWarp = false; }

        CurrentPlace = Place;
        CurrentYear = Year;
        YearDisplay.text = Year;

        UnloadExtras();

        if (Year == "2100" )
        {
            SceneManager.LoadScene("2100places",
                LoadSceneMode.Additive);
            Debug.Log("loading 2100");
        }
        else if (Year == "2020")
        {
            SceneManager.LoadScene("2020places",
                        LoadSceneMode.Additive);
            Debug.Log("loading 2020");
        }

        if (Place == "LAB")
        {
            PlaceDisplay.text = "Rachel's Lab";

            if ( PreWarp )
            {
                SceneManager.LoadScene("LAB_selected",
                    LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.LoadScene("LAB_current",
                    LoadSceneMode.Additive);
            }

            Debug.Log("loading lab marker");
        }
        else
        {
            PlaceDisplay.text = "California";
            
            if ( PreWarp )
            {
                SceneManager.LoadScene("CA_selected",
                        LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.LoadScene("CA_current",
                        LoadSceneMode.Additive);
            }

            Debug.Log("loading cali marker");
        }
    }

    private void UnloadExtras()
    {
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

        if (SceneManager.GetSceneByName("LAB_selected").isLoaded)
        {
            SceneManager.UnloadSceneAsync("LAB_selected");
            Debug.Log("unloading LAB_selected");
        }

        if (SceneManager.GetSceneByName("LAB_current").isLoaded)
        {
            SceneManager.UnloadSceneAsync("LAB_current");
            Debug.Log("unloading LAB_current");
        }

        if (SceneManager.GetSceneByName("CA_selected").isLoaded)
        {
            SceneManager.UnloadSceneAsync("CA_selected");
            Debug.Log("unloading CA_selected");
        }

        if (SceneManager.GetSceneByName("CA_current").isLoaded)
        {
            SceneManager.UnloadSceneAsync("CA_current");
            Debug.Log("unloading CA_current");
        }
    }
}