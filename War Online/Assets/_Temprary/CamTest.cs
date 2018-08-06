 using UnityEngine;
 using System.Collections;
 
 public class CamTest : Photon.PunBehaviour
 {
     [SerializeField]
     private Transform target;
 
     [SerializeField]
     private Vector3 offsetPosition;
 
     [SerializeField]
     private Space offsetPositionSpace = Space.Self;
 
     [SerializeField]
     private bool lookAt = true;

    private void Start()
    { /*
        this.gameObject.SetActive(photonView.isMine);*/
    }

    private void LateUpdate()
     {
         Refresh();
     }
 
     public void Refresh()
     {
         if(target == null)
         {
             Debug.LogWarning("Missing target ref !", this);
 
             return;
         }
 
         // compute position
         if(offsetPositionSpace == Space.Self)
         {
             transform.position = target.TransformPoint(offsetPosition);
         }
         else
         {
             transform.position = target.position + offsetPosition;
         }
 
         // compute rotation
         if(lookAt)
         {
             transform.LookAt(target);
         }
         else
         {
             transform.rotation = target.rotation;
         }
     }
 }
