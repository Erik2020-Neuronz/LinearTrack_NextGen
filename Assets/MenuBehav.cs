using UnityEngine;
using UnityEngine.UI;

public class MenuBehav : MonoBehaviour
{

    public string port;
    public string mazeChoice;
    public string filePath;
    public string fileName;
    public float gain;
    public float period;

    public int turnScrollOn; // I know techincally it is improper to name an int like a bool, but it acts like a bool
    //because of how PlayerPrefs works
    public int turnUnroundPosOn;
    public int turnLoopCountOn;
     

    //The way Valero setup, these game objects are used to reference the text object
    //that is attacehd to each input field
    //used to update playerprefs once the maze loads
    public GameObject inputGain;
    public GameObject inputPort; 
    public GameObject inputPeriod;
    public GameObject inputPath;
    public GameObject inputFileName;
    public string mazeChoice2;
    Camera m_MainCamera;
    public Camera m_CameraTwo;
    //in order to change the defaults and make them appear in the UI,
    //we need to refrence the inputfield itself and then access its text component
    //that's what these further public inputfields are for
    //the text-object seems to be different than than the text of the input field
    public InputField inputField_Port;
    public InputField inputField_Gain;
    public InputField inputField_Period;
    public InputField inputField_FilePath;
    public InputField inputField_FileName;
    
    public Toggle scrollToggle; //for toggling scroll wheel
    public Toggle posUnroundToggle; //for toggling if position readout to text will be rounded
    public Toggle loopCountToggle;

    public Button additionalOptions;
    public Button back2Menu;

    void Start()
    {
        Cursor.visible = true;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        //*** PlayerPrefs is a memory space built into unity to store
        // small bits of data between player sessions
        // here we use it to store default menu values

        //inputPort.GetComponent<InputField>().text = PlayerPrefs.GetString("Port");
        //here we reference the text component of the input field and set it to 
        //the previous preferences
        //that way they will display and be set next time the program is opened
        //setting default menu values based on previous prefs
        inputField_Port.GetComponent<InputField>().text = PlayerPrefs.GetString("Port");
        //Debug.Log(PlayerPrefs.GetString("Port"));

        inputField_Gain.GetComponent<InputField>().text = PlayerPrefs.GetFloat("Gain").ToString();

        inputField_Period.GetComponent<InputField>().text = PlayerPrefs.GetFloat("Period").ToString();

        inputField_FilePath.GetComponent<InputField>().text = PlayerPrefs.GetString("FilePath");

        inputField_FileName.GetComponent<InputField>().text = PlayerPrefs.GetString("FileName");

        //sets the scroll on or off based on previous preferences
        // toggles -----------------------------------------------------------------------------------
        if (PlayerPrefs.GetInt("Scroll") == 1)
        {
            scrollToggle.isOn = true;

        }
        else if ((PlayerPrefs.GetInt("Scroll")) == 0)
        {
            scrollToggle.isOn = false;
        }

        if (PlayerPrefs.GetInt("posUnround") == 1)
        {
            posUnroundToggle.isOn = true;

        }
        else if ((PlayerPrefs.GetInt("posUnround")) == 0)
        {
            posUnroundToggle.isOn = false;
        }

        if (PlayerPrefs.GetInt("loopCount") == 1)
        {
            loopCountToggle.isOn = true;

        }
        else if ((PlayerPrefs.GetInt("loopCount")) == 0)
        {
            loopCountToggle.isOn = false;
        }



        Button additOpt = additionalOptions.GetComponent<Button>();
        additOpt.onClick.AddListener(TaskOnAdditOptClick);

        Button b2menu = back2Menu.GetComponent<Button>();
        b2menu.onClick.AddListener(TaskOnB2MenuClick);


        m_MainCamera = Camera.main;
        m_MainCamera.enabled = true;
        m_CameraTwo.enabled = false;
    }

    void TaskOnAdditOptClick()
    {
        //Debug.Log("You have clicked the button!");
        //m_MainCamera.transform.position = new Vector3(2510, 314, -993);
        m_CameraTwo.enabled = true;
        m_MainCamera.enabled = false;
    }

    void TaskOnB2MenuClick()
    {
        //Debug.Log("You have clicked the button!");
        
        m_CameraTwo.enabled = false;
        m_MainCamera.enabled = true;
    }

    public void Maze1() {

        //player prefs are updated once Maze1 is selected
        //making the current values the new defaults
        port = inputPort.GetComponent<Text>().text;
        PlayerPrefs.SetString("Port", port);

        gain = float.Parse(inputGain.GetComponent<Text>().text);
        PlayerPrefs.SetFloat("Gain", gain);

        period = float.Parse(inputPeriod.GetComponent<Text>().text);
        PlayerPrefs.SetFloat("Period", period);

        filePath = inputPath.GetComponent<Text>().text;
        PlayerPrefs.SetString("FilePath", filePath);

        fileName = inputFileName.GetComponent<Text>().text;
        PlayerPrefs.SetString("FileName", fileName);

        mazeChoice = "squares_welft";
        PlayerPrefs.SetString("Maze", mazeChoice);

        if (scrollToggle.isOn == true)
        {
            turnScrollOn = 1;
            PlayerPrefs.SetInt("Scroll", turnScrollOn);
        }

        if (scrollToggle.isOn == false)
        {
            turnScrollOn = 0;
            PlayerPrefs.SetInt("Scroll", turnScrollOn);
        }

        if (posUnroundToggle.isOn == true)
        {
            turnUnroundPosOn = 1;
            PlayerPrefs.SetInt("posUnround", turnUnroundPosOn);
        }

        if (posUnroundToggle.isOn == false)
        {
            turnUnroundPosOn = 0;
            PlayerPrefs.SetInt("posUnround", turnUnroundPosOn);
        }

        if (loopCountToggle.isOn == true)
        {
            turnLoopCountOn = 1;
            PlayerPrefs.SetInt("loopCount", turnLoopCountOn);
        }

        if (loopCountToggle.isOn == false)
        {
            turnLoopCountOn = 0;
            PlayerPrefs.SetInt("loopCount", turnLoopCountOn);
        }


        Application.LoadLevel("Pause");
    }
}
