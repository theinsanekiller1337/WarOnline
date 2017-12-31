// This script controls the shell ejection.
var fireRate = .1;
private var nextFire = 0.0;
var Bullet: Transform;

function Update () {
if(Time.time > nextFire)
{
   nextFire = Time.time + fireRate;
   var bullet = Instantiate(Bullet,gameObject.Find("Spawn").transform.position,Random.rotation);
   bullet.GetComponent.<Rigidbody>().AddForce(transform.forward * 100);

}
}