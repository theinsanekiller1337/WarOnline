using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snipershooting : MonoBehaviour {
    #region variables
    public Camera camera;
    public float zoom_speed = 1;
    public GameObject rocket;
    public string buttontozoom;
    float camerastartingfov;
    #endregion
    private void Start()
    {
        camerastartingfov = camera.fieldOfView;

    }
    private void Update()
    {
        bool zoomheld = Input.GetKey(buttontozoom);
        bool zoomreleased = Input.GetKeyUp(buttontozoom);

        if (zoomheld)
        {
            camera.fieldOfView = camera.fieldOfView - zoom_speed;
        }

        if (zoomreleased)
        {
            camera.fieldOfView = camerastartingfov;
            shoot();
        }
        

    }
    public void shoot() //code for shooting
    {
        Debug.Log("Shot");

    }

}
