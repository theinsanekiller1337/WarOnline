using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotation : MonoBehaviour {
  public float turnLeft = 2.0f;
  public float turnRight = 2.0f;
 void  Update (){

  if(Input.GetKey("z")){
  transform.Rotate(0,-turnLeft,0);
 }

  if(Input.GetKey("x")){
  transform.Rotate(0,turnRight,0);
 }

    }
}

