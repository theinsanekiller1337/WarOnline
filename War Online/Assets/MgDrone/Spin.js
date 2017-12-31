//This script controls how fast the barel spins

var TurnSpeed = 8; // ajust this speed to increase/decreas the RPM's
static var rotate = true;;

function Update () {
if(rotate == true)
{
  transform.Rotate(0,1 * Time.deltaTime * TurnSpeed,0);
}
}