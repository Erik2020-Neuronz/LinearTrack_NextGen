using UnityEngine;
using System.Collections;
using System.IO.Ports;
using UnityEngine.UI;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;

public class movimento : MonoBehaviour {
    //-------------------------------------------------------------------------------
    // Key concepts to understand
    // public - the user and other codes can access wheras private is restricted to this file
    // 0 - 100 unity units is defined as 0 - 25.5 units in the unity editor
    // this is why the movement section has a multiplier of 3.9216
    // 
    // floats in unity have an "f" after them so 0.25 is written 0.25f
    // 
    // the object "this" refers to the object which has this script attached
    // or rather the object who inherets this script as a subclass
    // used frequently in the movement code for this.position.transform
    //


    public float speed_factor = 3; //what "gain" in the menu translates to. inititalized at 3 just to be non-zero but gets changed in voidstart()
    public float unroundPosition; 
    public double flashback = 25.5; // "teleport point" or distance where the lap ends and the loop resets
	public double treshold = 25.5; // used for 
    public double backtreshold = 0;// backwards "teleport point" used if the mouse goes backwards throught the beginning of the loop
    public int position; 
    public int lapCounter = 0; // number of laps 

    
    public float speed = 0; //not yet used, but a simply distance traveled in one timestep/time
    private float movement = 0; // used in the movement script as a place holder for distance needed to be travelled
    private int previous_position = -1; 
    private bool isCheckpointReached; //boolean to check if the mouse is progressing through the maze or is hovering over the checkpoint
    private bool isScrollOn; //checks if the scroll wheel option is on (1) or off (0)
    private bool isUnroundOn; //checks if the unrounded position (text) option is on (1) or off (0)
    private bool isLoopCountOn; //checks if the loop counter option is on (1) or off (0)
    private bool isWriteFileOn; // checsk if the write file option is on (1) or off(0)
    private bool isPathErrorLogged = false; //to log path error only once in the debugger
    private int didLapComplete = 0; // boolean to determine the lap has completed. Used in conditional to update lap counter
    private int listCounter = 0;//counts the number of data points in the text file list. 
    

    private float nextActionTime = 0.0f; //a secondary check to ensure the arduino does not receive inputs from unity faster than the user defined recording rate
    private float currentTime = 0.0f; //used for the time component of the text file
    public float period = 0.01f; //works with "nextActionTime" to set the recording rate
    private double loopCheck; //the rising edge of the loop check band
    private double loopCheckEnd;// the falling edge of the loop check band

    public GameObject textDisplay;

    string time2Text; //converts time float into a string to be written to file
    private string content;//the final string that gets written to file
    public string path; //the text file path
    private string tsOut;//no longer in use

    private string newPath;//no longer in use

    private StreamWriter writer;//no longer in use
    private Stream stream;//no longer in use
    private List<string> ts; //list that contains all data points before writing to file. Acts as a buffer so unity doesn't have to write to file every frame

    SerialPort sp; //= new SerialPort("COM6", 9600);

    void CreateText(ref string path)
    {
        //Path of the file
        //*** remember *** need "/" not "\"
        
        if (!File.Exists(path)) //Checks if the file already exists
        {
            File.WriteAllText(path, "\n\n"); //creates the file if not
        }
        //Content of the file
        //a function that adds a number to the filename if it already exists
        //also adds the ".txt" to the end
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
        // serial port is a class which inherits the serial library. This acts as the intermediary between arduino and unity
        try
        {
            sp = new SerialPort(PlayerPrefs.GetString("Port"), 9600); //creates new serial port with the user defined Port# and baud rate of 9600 (a baud of 9600 means unity sends 9600 bits/sec to arduino)
            sp.Open(); //opens the serial port
            sp.ReadTimeout = 50; //after 50 sec without communication, the port closes
            Application.targetFrameRate = -1; //keep this in mind
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
        //********************************
        //path = PlayerPrefs.GetString("FilePath", "C:/Users/EnglishLab WS3/Documents/Test2") + "/" + PlayerPrefs.GetString("FileName", "PleaseWork.txt") + ".txt";
        //CreateText(ref path); // was previously an empty void 2_16_21

        path = PlayerPrefs.GetString("RefPath");
        Debug.Log(path);

        if (PlayerPrefs.GetInt("posUnround") == 1) //checks if the loop count toggle is active
        {
            isUnroundOn = true;
        }
        else if (PlayerPrefs.GetInt("posUnround") == 0)
        {
            isUnroundOn = false;
        }

        if (PlayerPrefs.GetInt("loopCount") == 1) //checks if the loop count toggle is active
        {
            isLoopCountOn = true;
        }
        else if (PlayerPrefs.GetInt("loopCount") == 0)
        {
            isLoopCountOn = false;
        }

        if (PlayerPrefs.GetInt("writeFile") == 1) //checks if the write file toggle is active
        {
            isWriteFileOn = true;
        }
        else if (PlayerPrefs.GetInt("writeFile") == 0)
        {
            isWriteFileOn = false;
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

        if (PlayerPrefs.GetInt("Scroll") == 1 ) //turns the scroll wheel on
        {
            isScrollOn = true;
        }
        else if (PlayerPrefs.GetInt("Scroll") == 0) //keeps the motion tracker on 
        {
            isScrollOn = false;
        }

        if (!isScrollOn) // script for using the motion tracker to measure movement
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            position = Mathf.RoundToInt(this.transform.position.z * (float)3.9216);
            movement = speed_factor * Mathf.Sign(y) * Mathf.Sqrt(x * x + y * y) * Time.deltaTime;
            speed = movement / Time.deltaTime;
            this.transform.position += new Vector3(0, 0, movement);
            
        }
        else if(isScrollOn) //script for using the scroll wheel to measure movement
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
            if (sp.IsOpen && Time.time > nextActionTime) //prevents data from sending too fast. action time defined by "period" on the menu
            {
                nextActionTime += period; 
                byte[] data = new byte[] { (byte)position }; // converts unity position to a bit array to be sent throug the serial port "sp"
                sp.Write(data, 0, data.Length); // actually writing the data to arduino
                
            }
        }
        catch (System.Exception)
        {
            textDisplay.GetComponent<Text>().text = "Disconnected"; // displays text of whether port is connected or not
        }


        //-----------------------------------------------------------------------------
        // checkpoint to help determine if a full lap has been completed or if the camera is oscillating around the end/beginning of the maze
        // this is designed to editable. The threshold is the end of the maze loop, the back threshold is the beginning of the maze loop
        // the loopCheck and loopCheckEnd act as a band of detection that the camera must pass first before the reward is primed or ready to fire
        // this is to ensure that the camera doesn't run past the reward, back up, and run past again to get free rewards without completing the loop
        // So no matter where the threshold and back threshold are, there will be a checkpoint band some where near the middle

        loopCheck = backtreshold + 0.65 * (treshold - backtreshold); 
        loopCheckEnd = backtreshold + 0.75 * (treshold - backtreshold);
       

        if ((this.transform.position.z >= loopCheck) && (this.transform.position.z <= loopCheckEnd))
        {
            isCheckpointReached = true;
        }

        //-----------------------------------------------------------------------------
        // the part of the script that teleports the mouse back if it reaches the end of the loop
        // the end is defined as the threshold
        // flashback is the amount by which the mouse is "teleported" in the z direction on the next frame
        // same occurs if the mouse walks backwards past the backthreshold

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

        if (isUnroundOn) //changes poisiton to unrounded position if selected
        {
            unroundPosition = this.transform.position.z * (float)3.9216;
            string unroundPosText = unroundPosition.ToString("F4");


            content = time2Text + " " + unroundPosText; //+ " " + "\n"

            
        } //integer position if unround is unselected
        else
        {
            content = time2Text + " " + position;
           
        }
         
        if (isLoopCountOn) // adds the loop counter if that is selected
        {
            content = content + " " + didLapComplete;
        }

        content = content + " " + "\n";

        //-----------------------------------------------------------------------------
        // code where the data gets written to file
        // stores data in a list until the "listCounter" number of recordings has been made

        newPath = "C:/Users/Erik/Documents/TestFolder/streamTester.txt"; // I think this was only for testing and no longer does anything
        if (isWriteFileOn)
        {
            try
            {
                ts.Add(content); // remember that the text data is added to a list before being written to file. This list acts as a buffer


                listCounter = listCounter + 1; // counts the number of elements in the list

                if (listCounter == 99) // once there are 100 elements, they are all written to file in 1 frame
                {


                    using (FileStream tw = File.Open(path, FileMode.Append)) //appends file referenced in "path"
                    {
                        foreach (string strData in ts)
                        {

                            byte[] info = new UTF8Encoding(true).GetBytes(strData); //converts the string to a bit array
                            tw.Write(info, 0, info.Length); //the text file is finally written 
                        }
                    }

                    listCounter = 0;
                    ts.Clear();
                }
            }
            catch
            {

                if (isPathErrorLogged == false)
                {
                    Debug.Log("Invalid Path");
                    isPathErrorLogged = true;
                }
            }
        }
        //-----------------------------------------------------------------------------
        //maze exit 

        if (Input.GetKeyDown("space")) // to go to pause screen
        {
            try
            {
                int duinoClose = 105;
                byte[] data = new byte[] { (byte)duinoClose }; // converts unity position to a bit array to be sent throug the serial port "sp"
                sp.Write(data, 0, data.Length); // actually writing the data to arduino
                sp.Close();
            }
            catch 
            { 

            }
            sp.Close();
            Application.LoadLevel("Pause");
            
        }

        if (Input.GetKeyDown("escape")) // to go to main menu
        {
            try
            {
                int duinoClose = 105;
                byte[] data = new byte[] { (byte)duinoClose }; // converts unity position to a bit array to be sent throug the serial port "sp"
                sp.Write(data, 0, data.Length); // actually writing the data to arduino
                sp.Close();
            }
            catch
            {

            }
            sp.Close();
            Application.LoadLevel("Menu");
            
        }
    }

}