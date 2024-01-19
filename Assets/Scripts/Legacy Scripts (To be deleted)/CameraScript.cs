using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPos = transform.position;
        float currX = cameraPos.x;
        float currY = cameraPos.y;
        float currZ = cameraPos.z;

        /// Camera Movement code given player input
        if (Input.GetKey(KeyCode.LeftArrow)) {
            currX -= 0.05f;
            transform.position = new Vector3(currX, currY, currZ);
        }

        if (Input.GetKey(KeyCode.RightArrow)) {
            currX += 0.05f;
            transform.position = new Vector3(currX, currY, currZ);
        }

        if (Input.GetKey(KeyCode.UpArrow)) {
            currZ += 0.05f;
            transform.position = new Vector3(currX, currY, currZ);
        }

        if (Input.GetKey(KeyCode.DownArrow)) {
            currZ -= 0.05f;
            transform.position = new Vector3(currX, currY, currZ);
        }

        
    }
}
