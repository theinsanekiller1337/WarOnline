#pragma strict

var camera1 : Camera; 
var camera2 : Camera; 
var camera3 : Camera;
var camera4 : Camera;
public var startCamera : int = 1;
 
function Start () 
{ 
   camera1.enabled = true; 
   camera2.enabled = false; 
   camera3.enabled = false;
   camera4.enabled = false;
   startCamera = 1;
} 
 
function Update () 
{ 
   if (Input.GetKeyDown ("c") && (startCamera == 1))
   { 
	  startCamera = 2;
      camera1.enabled = false; 
      camera2.enabled = true; 
	  camera3.enabled = false;
	  camera4.enabled = false;
   } 
 
   else if (Input.GetKeyDown ("c") && (startCamera == 2))
   { 
	  startCamera = 3;
      camera1.enabled = false; 
      camera2.enabled = false; 
	  camera3.enabled = true;
	  camera4.enabled = false;
   } 
 
   else if (Input.GetKeyDown ("c") && (startCamera == 3))
   { 
	  startCamera = 4;
      camera1.enabled = false; 
      camera2.enabled = false; 
	  camera3.enabled = false;
	  camera4.enabled = true;
   } 
 
   else if (Input.GetKeyDown ("c") && (startCamera == 4))
   { 
	  startCamera = 1;
      camera1.enabled = true; 
      camera2.enabled = false; 
	  camera3.enabled = false;
	  camera4.enabled = false;
   } 
 
}