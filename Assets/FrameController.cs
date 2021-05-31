using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        float framerateFloat = PlayerPrefs.GetFloat("Period");
        int framerate = (int) Mathf.Round(1 / framerateFloat);
        //Debug.Log(framerateFloat);
        //Debug.Log(framerate);
        Application.targetFrameRate = framerate; 

        
    }

    // Update is called once per frame
    void Update()
    {

    }
}
