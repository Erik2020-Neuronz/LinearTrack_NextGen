using UnityEngine;
using System.Collections;
using System.IO.Ports;
using UnityEngine.UI;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;

public class TestScript : MonoBehaviour
{
    public float speed_factor = 3;
    public float unroundPosition;
    public double flashback = 25.5;
    public double treshold = 25.5;
    public double backtreshold = 0;
    public int position;
    public int lapCounter = 0;


    public float speed = 0;
    private float movement = 0;
    private int previous_position = -1;
    private bool isCheckpointReached;
    private bool isScrollOn;
    private bool isUnroundOn;
    private bool isLoopCountOn;
    private int didLapComplete = 0;
    private int listCounter = 0;


    private float nextActionTime = 0.0f;
    private float currentTime = 0.0f;
    public float period = 0.01f;
    private double loopCheck;
    private double loopCheckEnd;

    public GameObject textDisplay;

    string time2Text;
    private string content;
    public string path;
    private string tsOut;

    private string newPath;



    // Start is called before the first frame update
    void Start()
    {

        //-----------------------------------------------------------------------------
        // applies all the menu settings such as gain, sampling frequency, etc. 

        speed_factor = PlayerPrefs.GetFloat("Gain", 0.9f);
        period = PlayerPrefs.GetFloat("period", 0.01f);
        Time.fixedDeltaTime = period;

        //path = PlayerPrefs.GetString("FilePath", "C:/Users/EnglishLab WS3/Documents/Test2") + "/" + PlayerPrefs.GetString("FileName", "PleaseWork.txt") + ".txt";
        //CreateText(ref path); // was previously an empty void 2_16_21

        if (PlayerPrefs.GetInt("posUnround") == 1) //checks if the unround toggle is active
        {
            isUnroundOn = true;
        }
        else if (PlayerPrefs.GetInt("posUnround") == 0)
        {
            isUnroundOn = false;
        }

        if (PlayerPrefs.GetInt("loopCount") == 1) //checks if the unround toggle is active
        {
            isLoopCountOn = true;
        }
        else if (PlayerPrefs.GetInt("loopCount") == 0)
        {
            isLoopCountOn = false;
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        //-----------------------------------------------------------------------------
        // movement script
        // selects different movement type depending on if the user chose "scroll wheel"
        // or motion tracker on the menu

        if (PlayerPrefs.GetInt("Scroll") == 1) //checks if the scroll toggle is active
        {
            isScrollOn = true;
        }
        else if (PlayerPrefs.GetInt("Scroll") == 0)
        {
            isScrollOn = false;
        }

        if (!isScrollOn) //remove if statement condition to make mouse motion tracker default
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            position = Mathf.RoundToInt(this.transform.position.z * (float)3.9216);
            movement = speed_factor * Mathf.Sign(y) * Mathf.Sqrt(x * x + y * y) * Time.deltaTime;
            speed = movement / Time.deltaTime;
            this.transform.position += new Vector3(0, 0, movement);

        }
        else if (isScrollOn) //function for using the scroll wheel to measure movement
        {
            float x_scroll = Input.GetAxis("Mouse ScrollWheel");
            position = Mathf.RoundToInt(this.transform.position.z * (float)3.9216);
            movement = 40.0f * speed_factor * x_scroll * Time.deltaTime;
            speed = movement / Time.deltaTime;
            this.transform.position += new Vector3(0, 0, movement);
        }


    }

}
