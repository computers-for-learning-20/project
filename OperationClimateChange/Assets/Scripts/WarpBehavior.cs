using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WarpBehavior : MonoBehaviour
{
    private Text NextYear;
    private Text NextPlace;

    public GameObject WhiteBox;

    // Start is called before the first frame update
    void Start()
    {
        NextPlace = GameObject.Find("PlaceText").GetComponent<Text>();
        NextYear = GameObject.Find("YearText").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadNextPlace()
    {
        string LevelName = "TimeTravelInterface";

        if (NextYear.text == "2100"
            && NextPlace.text == "Rachel's Lab")
        {
            LevelName = "Lab";
        }
        else if (NextYear.text == "2020"
            && NextPlace.text == "California")
        {
            LevelName = "ca2020";
        }

        SceneManager.LoadScene(LevelName);
        Time.timeScale = 1f;
    }

    public void LoadWhiteOut()
    {
        WhiteBox.SetActive(true);
    }
}
