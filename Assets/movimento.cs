using UnityEngine;
using System.Collections;
using System.IO.Ports;
using UnityEngine.UI;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;

public class movimento : MonoBehaviour {
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

    private StreamWriter writer;
    private Stream stream;
    private List<string> ts;

    SerialPort sp; //= new SerialPort("COM6", 9600);

    void CreateText(ref string path)
    {
        //Path of the file
        //*** remember *** need "/" not "\"
        //Create File if it doesn't exist
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "\n\n");
        }
        //Content of the file
        //a function that adds a number to the filename if it already exists
        else
        {
            int num = 1;
            string origPath = path;
            while (File.Exists(path))
            {

                string toTrim = origPath;
                int n = 4;

                toTrim = toTrim.Remove(toTrim.Length - n);

                string s;
                s = string.Concat(toTrim, "(" + (num++) + ")" + ".txt");
                path = s;
                
            }
            File.WriteAllText(path, "\n\n");
        }

    }


    // Use this for initialization
    void Start()
    {
        Cursor.visible = false; // so the cursor is not flying all around the screen

        //-----------------------------------------------------------------------------
        // instantiates serial port inputting port# and baud rate
        try
        {
            sp = new SerialPort(PlayerPrefs.GetString("Port"), 9600);
            sp.Open();
            sp.ReadTimeout = 50;
            Application.targetFrameRate = -1;
            textDisplay.GetComponent<Text>().text = "Connected";
        }
        catch (System.Exception)
        {
            textDisplay.GetComponent<Text>().text = "Disconnected";
        }

        //-----------------------------------------------------------------------------
        // applies all the menu settings such as gain, sampling frequency, etc. 

        speed_factor = PlayerPrefs.GetFloat("Gain", 0.9f);
        period = PlayerPrefs.GetFloat("period", 0.01f);
        Time.fixedDeltaTime = period;

        path = PlayerPrefs.GetString("FilePath", "C:/Users/EnglishLab WS3/Documents/Test2") + "/" + PlayerPrefs.GetString("FileName", "PleaseWork.txt") + ".txt";
        CreateText(ref path); // was previously an empty void 2_16_21

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

        //-----------------------------------------------------------------------------
        // creates a list to be used later 

        List<string> myList = new List<string>();
        
        ts = myList;
        
        PlayerPrefs.SetInt("lapCounter", lapCounter);
    }

    // Update is called once per frame
    void Update () 
    {
        //moved everything to fixed update to maintain regularity
    }

     void FixedUpdate()
    {


        //-----------------------------------------------------------------------------
        // movement script
        // selects different movement type depending on if the user chose "scroll wheel"
        // or motion tracker on the menu

        if (PlayerPrefs.GetInt("Scroll") == 1 ) //checks if the scroll toggle is active
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
        else if(isScrollOn) //function for using the scroll wheel to measure movement
        {
            float x_scroll = Input.GetAxis("Mouse ScrollWheel");
            position = Mathf.RoundToInt(this.transform.position.z * (float)3.9216);
            movement = 40.0f * speed_factor * x_scroll * Time.deltaTime;
            speed = movement / Time.deltaTime;
            this.transform.position += new Vector3(0, 0, movement);
        }

        //-----------------------------------------------------------------------------
        // script writing to arduino
        // if arduino is not connected, text on the bottom right will say so

        try
        {
            if (sp.IsOpen && Time.time > nextActionTime)
            {
                nextActionTime += period;
                byte[] data = new byte[] { (byte)position };
                sp.Write(data, 0, data.Length);
                
            }
        }
        catch (System.Exception)
        {
            textDisplay.GetComponent<Text>().text = "Disconnected"; // displays text of whether port is connected or not
        }


        //-----------------------------------------------------------------------------
        //checkpoint to help determine if a full lap has been completed or if the camera is oscillating
        //around the end/beginning of the maze

        loopCheck = backtreshold + 0.65 * (treshold - backtreshold);
        loopCheckEnd = backtreshold + 0.75 * (treshold - backtreshold);
       

        if ((this.transform.position.z >= loopCheck) && (this.transform.position.z <= loopCheckEnd))
        {
            isCheckpointReached = true;
        }


        didLapComplete = 0;
        if (this.transform.position.z > treshold)
        {
            this.transform.position -= new Vector3(0, 0, (float)flashback);

            if (isCheckpointReached)
            {
                isCheckpointReached = false;
                didLapComplete = 1;
                lapCounter = lapCounter + 1;
                PlayerPrefs.SetInt("lapCounter", lapCounter);
            }
            
        }
        if (this.transform.position.z < backtreshold) 
        {
            this.transform.position += new Vector3(0, 0, (float)flashback);
        }

        //-----(position reset if something really goes wrong)---------------
        // in case the mouse's avatar travels far out of bounds or veers off the linear path
        // note that the outer z bound is set to 50. The editor currently has the 
        // the track extend from 0 -> 25.5 arbitrary units in the editor space
        // While the editor works with these arbitrary units when calculating position, 
        // this number gets multiplied by 3.9216 before going to arduino or file
        float zz = this.transform.position.z;
        float xx = this.transform.position.x;
        float yy = this.transform.position.y;
            
        if (this.transform.position.z > 50 )
        {
            this.transform.position = new Vector3(xx, yy, 0);
        }
        if (this.transform.position.z < -15f)
        {
            this.transform.position = new Vector3(xx, yy, 0);
        }
        if (Mathf.Abs(this.transform.position.y) - 0.3f > 0.01)
        {
            this.transform.position = new Vector3(0, -0.3f, 0);
        }
        if (Mathf.Abs(this.transform.position.x) > 0.01)
        {
            this.transform.position = new Vector3(0, yy, zz);
        }

        

        if (Input.GetKeyDown("w"))
        {
            print(this.transform.position.z);
        }


        previous_position = Mathf.RoundToInt(this.transform.position.z * (float)3.9216);

       
        //-----------------------------------------------------------------------------
        // This section compiles the data into a string named "content"
        // That string is what gets written to file 
        currentTime += Time.deltaTime;

            time2Text = currentTime.ToString("F3");

        if (isUnroundOn)
        {
            unroundPosition = this.transform.position.z * (float)3.9216;
            string unroundPosText = unroundPosition.ToString("F4");


            content = time2Text + " " + unroundPosText; //+ " " + "\n"

            
        }
        else
        {
            content = time2Text + " " + position;
           
        }
         
        if (isLoopCountOn)
        {
            content = content + " " + didLapComplete;
        }

        content = content + " " + "\n";

        //-----------------------------------------------------------------------------
        // code where the data gets written to file
        // stores data in a list until the "listCounter" number of recordings has been made

        newPath = "C:/Users/Erik/Documents/TestFolder/streamTester.txt";
        
        ts.Add(content);


        listCounter = listCounter + 1;

        if(listCounter == 99)
        {

                   
            using (FileStream tw = File.Open(path, FileMode.Append)) //appends file referenced in "path"
            {
                foreach (string strData in ts)
                {
                    
                    byte[] info = new UTF8Encoding(true).GetBytes(strData);
                    tw.Write(info, 0, info.Length);
                }
            }
                        
            listCounter = 0;
            ts.Clear();
        }


        //-----------------------------------------------------------------------------
        //application exit

        if (Input.GetKeyDown("space"))
        {
            Application.LoadLevel("Pause");
            
        }

        if (Input.GetKeyDown("escape"))
        {
            Application.LoadLevel("Menu");
            
        }
    }

}