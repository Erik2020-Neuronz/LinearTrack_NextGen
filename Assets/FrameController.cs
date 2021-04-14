using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 100;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
