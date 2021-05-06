using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingScript : MonoBehaviour
{

    float speed = 1f;
    //adjust this to change how high it goes
    float height = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        /*
        float z = transform.position.z;
        //get the objects current position and put it in a variable so we can access it later with less code
        Vector3 pos = transform.position;
        //calculate what the new Y position will be
        float newY = Mathf.Sin(Time.time * speed);
        //set the object's Y to the new calculated Y
        transform.position = new Vector3(pos.x, newY, z) * height;
        */
        transform.position = new Vector3(transform.position.x, 0.3f * Mathf.Sin(Time.time * speed), transform.position.z );
        
        this.transform.Rotate(0, 50 * Time.deltaTime, 50 * Time.deltaTime);
    }
    
}
