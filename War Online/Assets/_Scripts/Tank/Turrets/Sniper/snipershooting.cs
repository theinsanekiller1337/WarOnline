using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snipershooting : MonoBehaviour {
    #region variables
    public Camera camera;
    public GameObject sniper;
    public float zoom_speed = 1f;
    public float zoom_limit = 5f;
    public string buttontozoom;
    public LineRenderer linerenderer;
    public Transform shootfrom;
    public Animator animcontroller;
    [Header("parameter in anim controller:" + " has to be bool" + " only the name of a parameter")]
    public string nameofaparametertoedit;
    [Tooltip("has to be bool" + "only the name of a parameter")]
    
    private Ray ray;
    float camerastartingfov;
    #endregion
    private void Start()
    {
        camerastartingfov = camera.fieldOfView;

        if(linerenderer == null)
        {
            linerenderer = GetComponent<LineRenderer>();
        }

    }
    private void Update()
    {
        bool zoomheld = Input.GetKey(buttontozoom);
        bool zoomreleased = Input.GetKeyUp(buttontozoom);

        if (zoomheld)
        {
            if (camera.fieldOfView > zoom_limit)
            {
                camera.fieldOfView = camera.fieldOfView - zoom_speed;
            }
            else if (camera.fieldOfView <= zoom_limit) 
            {
                camera.fieldOfView = zoom_limit;
            }
            animcontroller.SetBool(nameofaparametertoedit, true);
            sniper.SetActive(false);
        }

        if (zoomreleased)
        {
            camera.fieldOfView = camerastartingfov;
            shoot();
            animcontroller.SetBool(nameofaparametertoedit, false);
            sniper.SetActive(true);
        }
        

    }
    public void shoot() //code for shooting
    {
        Debug.Log("Shot fired");

        

        linerenderer.enabled = true;
        ray.origin = shootfrom.position;
        ray.direction = transform.forward;
        linerenderer.SetPosition(0, shootfrom.position);


    }

}
