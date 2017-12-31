var DieTime = 6;

function Update () {

Invoke("Kill",DieTime);
}

function Kill ()
{
 Destroy(gameObject);
}