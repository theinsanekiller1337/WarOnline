 #pragma strict
 
 var walkSpeed = 150;
 var rotateSpeed = 100;
 var jumpSpeed : float = 8.0;
 var gravity : float = 20.0;
 
 private var moveDirection : Vector3 = Vector3.zero;
 
 function Update () {
 var controller : CharacterController = GetComponent.<CharacterController>();
 
 if (controller.isGrounded) {
     // We are grounded, so recalculate
     // move direction directly from axes
     moveDirection = Vector3(0, 0, Input.GetAxis("Vertical"));
     moveDirection = transform.TransformDirection(moveDirection);
     moveDirection *= walkSpeed * Time.deltaTime;
 
     //also with jump ;)
     if (Input.GetButton ("Jump")) {
         moveDirection.y = jumpSpeed;
     }
 }
 
 // Apply gravity 

 
 // Move the controller
 controller.Move(moveDirection * Time.deltaTime);
 
 // rotate controller
 var horizontalDir = parseFloat(Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime);
 transform.Rotate(0, horizontalDir, 0);
 }