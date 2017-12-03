#pragma strict

 enum Axes {MouseXandY, MouseX, MouseY}
    var Axis : Axes = Axes.MouseXandY;
 
    var sensitivityX = 15.0;
    var sensitivityY = 15.0;
 
    var minimumX = -360.0;
    var maximumX = 360.0;
 
    var minimumY = -60.0;
    var maximumY = 60.0;
 
    var rotationX = 0.0;
    var rotationY = 0.0;
 
    var lookSpeed = 2.0;
 
    function Update ()
    {
        if (Axis == Axes.MouseXandY)
        {
            // Read the mouse input axis
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
 
            // Call our Adjust to 360 degrees and clamp function
            Adjust360andClamp();
 
            // Most likely you wouldn't do this here unless you're controlling an object's rotation.
            // Call our look left and right function.
            KeyLookAround();
 
            // Call our look up and down function.
            KeyLookUp();
 
            // If the user isn't pressing a key to look up, transform our X angle to the mouse.
            if (!Input.GetAxis("LookAround"))
            {
                // If you don't want to allow a key to affect X, keep this line but take it out of the if
                transform.localRotation = Quaternion.AngleAxis (rotationX, Vector3.up);
            }
 
            // If the user isn't pressing a key to look up, transform our Y angle to the mouse.
            if (!Input.GetAxis("LookUp"))
            {
                // Multiply the Quaterion so we don't loose our X we just transformed
                // If you don't want to allow a key to affect Y, keep this line but take it out of the if
                transform.localRotation *= Quaternion.AngleAxis (rotationY, Vector3.left);
            }
        }
        else if (Axis == Axes.MouseX)
        {
            // Read the mouse input axis
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
 
            // Call our Adjust to 360 degrees and clamp function
            Adjust360andClamp();
 
            // if you're doing a standard X on object Y on camera control, you'll probably want to 
            // delete the key control in MouseX. Also, take the transform out of the if statement.
            // Call our look left and right function.
            KeyLookAround();
 
            // Call our look up and down function.
            KeyLookUp();
 
            // If the user isn't pressing a key to look up, transform our X angle to the mouse.
            if (!Input.GetAxis("LookAround"))
            {
                //If you don't want to allow a key to affect X, keep this line but take it out of the if
                transform.localRotation = Quaternion.AngleAxis (rotationX, Vector3.up);
            }
 
        }
        else
        {
            // Read the mouse input axis
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
 
            // Call our Adjust to 360 degrees and clamp function
            Adjust360andClamp();
 
            // Call our look left and right function.
            KeyLookAround();
 
            // Call our look up and down function.
            KeyLookUp();
 
            // If the user isn't pressing a key to look up, transform our Y angle to the mouse
            if (!Input.GetAxis("LookUp"))
            {
                // If you don't want to allow a key to affect Y, keep this line but take it out of the if
                transform.localRotation = Quaternion.AngleAxis (rotationY, Vector3.left);
            }
 
        }
    }
 
    function KeyLookAround ()
    {
//      If you're not using it, you can delete this whole function. 
//      Just be sure to delete where it's called in Update.
 
        // Read the mouse input axis
        rotationX += Input.GetAxis("LookAround") * lookSpeed;
 
        // Call our Adjust to 360 degrees and clamp function
        Adjust360andClamp();
 
        // Transform our X angle
        transform.localRotation = Quaternion.AngleAxis (rotationX, Vector3.up);
    }
 
    function KeyLookUp ()
    {
//      If you're not using it, you can delete this whole function. 
//      Just be sure to delete where it's called in Update.
 
        // Read the mouse input axis
        rotationY += Input.GetAxis("LookUp") * lookSpeed;
 
        // Adjust for 360 degrees and clamp
        Adjust360andClamp();
 
        // Transform our Y angle, multiply so we don't loose our X transform
        transform.localRotation *= Quaternion.AngleAxis (rotationY, Vector3.left);
    }
 
    function Adjust360andClamp ()
    {
//      This prevents your rotation angle from going beyond 360 degrees and also 
//      clamps the angle to the min and max values set in the Inspector.
 
        // During in-editor play, the Inspector won't show your angle properly due to 
        // dealing with floating points. Uncomment this Debug line to see the angle in the console.
 
        // Debug.Log (rotationX);
 
        // Don't let our X go beyond 360 degrees + or -
        if (rotationX < -360)
        {
            rotationX += 360;
        }
        else if (rotationX > 360)
        {
            rotationX -= 360;
        }   
 
        // Don't let our Y go beyond 360 degrees + or -
        if (rotationY < -360)
        {
            rotationY += 360;
        }
        else if (rotationY > 360)
        {
            rotationY -= 360;
        }
 
        // Clamp our angles to the min and max set in the Inspector
        rotationX = Mathf.Clamp (rotationX, minimumX, maximumX);
        rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
    }
 
    function Start ()
    {
        // Make the rigid body not change rotation
        if (GetComponent.<Rigidbody>())
        {
            GetComponent.<Rigidbody>().freezeRotation = true;
        }
    }