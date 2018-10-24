using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snipershooting : MonoBehaviour {
    #region GameObjects
    public new Camera camera;
    public GameObject sniper;
    public GameObject scope;
    public GameObject ScopeImage;
    #endregion
    #region Floats
    public float zoom_speed = 1f;
    public float zoom_limit = 5f;
    float camerastartingfov = 48f;
    #endregion
    #region Others
    public string buttontozoom;
    public LineRenderer linerenderer;
    public Transform shootfrom;
    #endregion
    #region Animators
    public Animator animcontroller;
    [Header("Parameter in Anim Controller:" + " has to be bool" + " only the name of a parameter")]
    public string nameofaparametertoedit;
    [Tooltip("has to be bool" + "only the name of a parameter")]
    private Ray ray;
    #endregion


    private void Start()
    {
        camerastartingfov = camera.fieldOfView;

        if(linerenderer == null)
        {
            linerenderer = GetComponent<LineRenderer>();
        }
        if (ScopeImage == null)
        {
            Canvas canvas = GetComponent<Canvas>();
            GameObject scopeOverlay = canvas.transform.Find("ScopeOverlay").gameObject;
           // ScopeImage = canvas.GetComponent<ScopeOverlay>
        }
    }
    private void Update()
    {
        ScopeImage.SetActive(false);
        linerenderer.enabled = false;
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

            // Raycast for Line Renderer
            RaycastHit hit1;
            if (Physics.Raycast(shootfrom.transform.position, shootfrom.transform.forward, out hit1))
            {
                Debug.Log(hit1.point);
            }

            //linerender 
            linerenderer.enabled = true;
            linerenderer.SetPosition(0, scope.transform.position);
            linerenderer.SetPosition(1, hit1.point); 

            //scopeOverlay
            if (camera.fieldOfView <= 46f)
            {
                ScopeImage.SetActive(true);
            }
        }

        if (zoomreleased)
        {
            linerenderer.enabled = false;
            camera.fieldOfView = camerastartingfov;
            shoot();
            animcontroller.SetBool(nameofaparametertoedit, false);
            sniper.SetActive(true);
            ScopeImage.SetActive(false);
        }
        

    }
    public void shoot() //code for shooting
    {
        Debug.Log("Shot fired");


    }

}
