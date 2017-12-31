#pragma strict

var fireRate = .1;  // ajust this variable to increase/decrease the fire rate
private var nextFire = 1.0;
var Bullet: Transform;

function Update () {
if(Time.time > nextFire)
{
   nextFire = Time.time + fireRate;
   var bullet = Instantiate(Bullet,gameObject.Find("BSpawn").transform.position,Random.rotation);
   bullet.GetComponent.<Rigidbody>().AddForce(transform.forward * 2000);

}
}