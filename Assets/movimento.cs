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
    
    // public bool hideMouse = false;
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

    void CreateText(ref string path) // was just an empty void 2_16_21
    {
        //Path of the file
        //string path = Application.dataPath + "/Log.txt";
        //string path = "C:/Users/EnglishLab WS3/Documents/TestPathway_Akbar" + "/PleaseWork.txt";
        //string path = PlayerPrefs.GetString("FilePath", "C:/Users/EnglishLab WS3/Documents/Test2") + "/" + PlayerPrefs.GetString("FileName", "PleaseWork.txt");
        //this dude was uncommented 2_16_21
        //*** remember *** need "/" not "\"
        //Create File if it doesn't exist
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "\n\n");
        }
        //Content of the file
        //finna boutta create a function that adds a number if a file exists
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
        Cursor.visible = false;
        //Debug.Log(PlayerPrefs.GetString("FilePath"));
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

        

        List<string> myList = new List<string>();
        
        ts = myList;
        //private myList = new List<string>();

        //targetGameobject.GetComponent<UnityEngine.EventSystems.PhysicsRaycaster>().enabled = false;


    }

    // Update is called once per frame
    void Update () 
    {
        //moved everything to fixed update to maintain regularity
    }

     void FixedUpdate()
    {

        

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
            //CreateText();
        }
        else if(isScrollOn) //function for using the scroll wheel to measure movement
        {
            float x_scroll = Input.GetAxis("Mouse ScrollWheel");
            position = Mathf.RoundToInt(this.transform.position.z * (float)3.9216);
            movement = 40.0f * speed_factor * x_scroll * Time.deltaTime;
            speed = movement / Time.deltaTime;
            this.transform.position += new Vector3(0, 0, movement);
        }
        try
        {
            if (sp.IsOpen && Time.time > nextActionTime)
            {
                nextActionTime += period;
                byte[] data = new byte[] { (byte)position };
                sp.Write(data, 0, data.Length);
                //CreateText();
                //Debug.Log("hello bro");
            }
        }
        catch (System.Exception)
        {
            textDisplay.GetComponent<Text>().text = "Disconnected";
        }

      

        loopCheck = backtreshold + 0.65 * (treshold - backtreshold);
        loopCheckEnd = backtreshold + 0.75 * (treshold - backtreshold);
        //a checkpoint
        //to help determine if a full lap has been completed or if the camera is oscillating
        //around the end/beginning of the maze


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

            }
            
        }
        if (this.transform.position.z < backtreshold)
        {
            this.transform.position += new Vector3(0, 0, (float)flashback);
        }

        //-----(position reset if something really goes wrong)---------------
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
        // if (sp.IsOpen) you just commented out this line 2_16_21
        // {  you just commented out this line 2_16_21
        //string path = PlayerPrefs.GetString("FilePath", "C:/Users/EnglishLab WS3/Documents/Test2") + "/" + PlayerPrefs.GetString("FileName", "PleaseWork.txt");
        //was initally a simple local variable you recalled 

        currentTime += Time.deltaTime;

            time2Text = currentTime.ToString("F3");

        if (isUnroundOn)
        {
            unroundPosition = this.transform.position.z * (float)3.9216;
            string unroundPosText = unroundPosition.ToString("F4");


            content = time2Text + " " + unroundPosText; //+ " " + "\n"

            //print("gg");
        }
        else
        {
            content = time2Text + " " + position;
            //print("naaaah fam");
        }
         
        if (isLoopCountOn)
        {
            content = content + " " + didLapComplete;
        }

        content = content + " " + "\n";

        // --------------------------------
        /* bufferedstean thing
        BufferedStream bufferedStream;

        bufferedStream.Write()

        try 
        { 
        
        }
        */
        // --------------------------------
        // use stream writer and do not close the file until escape, close or crash


        newPath = "C:/Users/Erik/Documents/TestFolder/streamTester.txt";
        //File.Open(newPath, FileMode.Create);


        /*
        byte[] datData = Encoding.ASCII.GetBytes(content);
        //byte[] datData = new byte[] { (byte)content };
        //Stream stream;
        StreamWriter writer = new StreamWriter(newPath, true); 
        //writer.WriteLine(datData);
        //MemoryStream ms = new MemoryStream();
        Stream tt = writer.BaseStream { get;};
        BufferedStream bufferedStream = new BufferedStream(tt);
        bufferedStream.Write(datData, 0, datData.Length);

        writer.Close();
        */



        ts.Add(content);


        listCounter = listCounter + 1;

        if(listCounter == 99)
        {

            //string holdingString; //= content;
            //holdingString = myList[1];
            //myList[]
            /*
            File.AppendAllText(path, ts);

            foreach (string dinosaur in ts)
            {
                Debug.Log(dinosaur);
            }
            */

            /*
            using (TextWriter tw = new StreamWriter(path))
            {
                foreach (string strData in ts)
                    tw.WriteLine(strData);
            }
            */

            
            using (FileStream tw = File.Open(path, FileMode.Append)) //
            {
                foreach (string strData in ts)
                {
                    //File.AppendAllText(path, strData);
                    byte[] info = new UTF8Encoding(true).GetBytes(strData);
                    tw.Write(info, 0, info.Length);
                }
            }


            /*
            for (int i = 0; i < 100; i++)
            {
                //tsOut = ts.ToString;
                Debug.Log(ts);
            }
            */

            //Debug.Log("Hello my guy!");
            listCounter = 0;
            ts.Clear();
        }

        //



        /*
        
        
        */

        //File.Close();
        //Add some to text it
        //File.AppendAllText(path, content); // You just turned this off to test stream writer 3/18/21
        // }  you just commented out this line 2_16_21



        // keep this in mind for application ex
        if (Input.GetKeyDown("space"))
        {
            Application.LoadLevel("Pause");
            //writer.Close();
        }

        if (Input.GetKeyDown("escape"))
        {
            Application.LoadLevel("Menu");
            //writer.Close();
        }
    }

}