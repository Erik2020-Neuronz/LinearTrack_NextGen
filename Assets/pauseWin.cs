/*using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
*/
using UnityEngine;
using System.Collections;
using System.IO.Ports;
using UnityEngine.UI;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class pauseWin : MonoBehaviour     


{


    private IEnumerator lvlLoadCoroutine;
    private bool isSceneLoaded = false;
    private bool isWriteFileOn;
    public Text m_Text;
    public string path; //the text file path

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;

        lvlLoadCoroutine = LoadScene(PlayerPrefs.GetString("Maze", "squares_welft"));

        StartCoroutine(lvlLoadCoroutine);

        if (PlayerPrefs.GetInt("writeFile") == 1) //checks if the write file toggle is active
        {
            isWriteFileOn = true;
        }
        else if (PlayerPrefs.GetInt("writeFile") == 0)
        {
            isWriteFileOn = false;
        }



        //StopCoroutine(AsynchronousLoad(PlayerPrefs.GetString("Maze", "squares_welft")));
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown("escape"))
        {
            Application.LoadLevel("Menu");
        }

    }

    
    IEnumerator LoadScene(string scene)
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        //Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            m_Text.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Change the Text to show the Scene is ready
                m_Text.text = "Level Loaded: Press the space bar to continue";
                //Wait to you press the space key to activate the Scene
                if (Input.GetKeyDown(KeyCode.Space))
                {    //Activate the Scene
                    try
                    {
                        if (isWriteFileOn)
                        {
                            path = PlayerPrefs.GetString("FilePath", "C:/Users/EnglishLab WS3/Documents/Test2") + "/" + PlayerPrefs.GetString("FileName", "PleaseWork.txt") + ".txt";
                            CreateText(ref path); // was previously an empty void 2_16_21
                            PlayerPrefs.SetString("RefPath", path);
                        }
                    }
                    catch
                    {
                        Debug.Log("InvalidPath");
                        path = PlayerPrefs.GetString("FilePath", "C:/Users/EnglishLab WS3/Documents/Test2") + "/" + PlayerPrefs.GetString("FileName", "PleaseWork.txt") + ".txt";
                        PlayerPrefs.SetString("RefPath", path);
                    }
                    m_Text.text = "Starting in: 3";
                    yield return new WaitForSeconds(1);                    
                    m_Text.text = "Starting in: 2";
                    yield return new WaitForSeconds(1);
                    m_Text.text = "Starting in: 1";
                    yield return new WaitForSeconds(1);
                    asyncOperation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

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



}


