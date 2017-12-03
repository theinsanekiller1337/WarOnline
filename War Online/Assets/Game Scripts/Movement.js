#pragma strict


function Update () {


     var xAxisValue = Input.GetAxis("Horizontal");
     var zAxisValue = Input.GetAxis("Vertical");

      transform.Translate(xAxisValue*8,0.0f,zAxisValue*8);

      var xyAxisValue = Input.GetAxis("Zoom_Out");

      transform.Translate(xyAxisValue*8,xyAxisValue*8,0.0f);

      var rotateValue = Input.GetAxis("Rotate");

      transform.Rotate(0.0f, rotateValue*8,0.0f);
  

 }