using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pauseWin : MonoBehaviour     


{


    private IEnumerator lvlLoadCoroutine;
    private bool isSceneLoaded = false;
    public Text m_Text;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;

        lvlLoadCoroutine = LoadScene(PlayerPrefs.GetString("Maze", "squares_welft"));

        StartCoroutine(lvlLoadCoroutine);


        //StopCoroutine(AsynchronousLoad(PlayerPrefs.GetString("Maze", "squares_welft")));
    }

    // Update is called once per frame
    void Update()
    {
       
        if(Input.GetKeyDown("q"))
        {
            


        }
        
        
        /*
        if (Input.GetKeyDown("space"))
        {
            Application.LoadLevel(PlayerPrefs.GetString("Maze", "squares_welft"));
                        
        }
        */

        if (Input.GetKeyDown("escape"))
        {
            Application.LoadLevel("Menu");
        }

    }

    /*
    IEnumerator AsynchronousLoad(string scene)
    {
        yield return null;
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        ao.allowSceneActivation = false;
        while (ao.isDone == false)
        {
            // [0, 0.9] > [0, 1]
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            Debug.Log("Loading progress: " + (progress * 100) + "%");
            // Loading completed
            if (Mathf.Approximately(ao.progress, 0.9f))
            {
                Debug.Log("Press SPACE to start");
                isSceneLoaded = true;
                if (Input.GetKeyDown("space"))
                    ao.allowSceneActivation = true;
                //StopCoroutine(lvlLoadCoroutine);
            }
            yield return null;
        }


    }
    */

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
                    //Activate the Scene
                    asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }





}


