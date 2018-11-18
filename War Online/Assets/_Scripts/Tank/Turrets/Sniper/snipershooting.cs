using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class snipershooting : MonoBehaviour {
    #region GameObjects
    public new Camera camera;
    public GameObject scope;
    public Image ScopeImage;
    #endregion
    #region Floats
    public float zoom_speed = 1f;
    public float zoom_limit = 5f;
    float camerastartingfov = 48f;
    public float zoomedRotateSpeed = 0.8f;
    public float damage = 150f;
    #endregion
    #region Others
    public string buttontozoom;
    public LineRenderer linerenderer;
    public Transform shootfrom;

    [SerializeField]
    private RaycastHit LookAtRayCast;
    public RaycastHit lookAtRaycast
    {
        get { return LookAtRayCast; } set { LookAtRayCast = value; } 
    }
    #endregion
    #region Animators
    public Animator animcontroller;
    [Header("Parameter in Anim Controller:" + " has to be bool" + " only the name of a parameter")]
    public string nameofaparametertoedit;
    [Tooltip("has to be bool" + "only the name of a parameter")]
    private Ray ray;
    private TurretRotation turretRotation;
    private float rotateSpeed;
   
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
            GameObject scope = GameObject.Find("ScopeOverlay");
            ScopeImage = scope.GetComponent<Image>();
        }
        turretRotation = GetComponent<TurretRotation>();
        rotateSpeed = turretRotation.KeyRotateSpeed;
    }
    public void Update()
    {

        ScopeImage.enabled =false;
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
           

            // Raycast for Line Renderer
            RaycastHit hit1;
            if (Physics.Raycast(shootfrom.transform.position, shootfrom.transform.forward, out hit1))
            {
                
            }

            LookAtRayCast = hit1;

            //linerender 
            linerenderer.enabled = true;
            linerenderer.SetPosition(0, scope.transform.position);
            linerenderer.SetPosition(1, hit1.point);

            //scopeOverlay
            if (camera.fieldOfView <= 46f)
            {
                ScopeImage.enabled = true;
            }
            //disabling object(s)
            gameObject.GetComponentInChildren<CamTest>().enabled = false;
            
            //shootAnime.enabled = false;
            

            turretRotation.KeyRotateSpeed = zoomedRotateSpeed;
        }

        if (zoomreleased)
        {   //enabling Object(s)
            linerenderer.enabled = false;
            gameObject.GetComponentInChildren<CamTest>().enabled = true;
            gameObject.GetComponentInChildren<Animator>().enabled = true;

            camera.fieldOfView = camerastartingfov;
            shoot();
            animcontroller.SetBool(nameofaparametertoedit, false);
           
            ScopeImage.enabled = false;

            turretRotation.KeyRotateSpeed = rotateSpeed;
        }
        

    }

    private void LateUpdate()
    {
         //   AnimatorClipInfo[] m_CurrentClipInfo = animcontroller.GetCurrentAnimatorClipInfo(1);
         //   AnimationClip animeClip = m_CurrentClipInfo[1].clip;

            animcontroller.SetLookAtPosition(lookAtRaycast.point);

    }

    public void shoot() //code for shooting
    {
       

        TankHealth targetHealth = LookAtRayCast.transform.gameObject.GetComponent<TankHealth>();

        if (targetHealth)
        {
            // Deal this damage to the tank.
            targetHealth.TakeDamage(damage);
        }



    }

}
