using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class c_turret_script : MonoBehaviour {

    //The distance to carry the bullet
    public int distance;

    //The reload duration what time in between reloading
    public int reloadduration;

    //How many bullets
    public int ammo;

    //How many magazines
    public int ammoclips;

    //How many bullets a second will be shooting
    public int bulletssec;

    //the velocity the speed of the head
    private float speed = 10.0F;

    //The character controller to add collision
    CharacterController controller;

    //The direction of the ammo head
    //Dont worry about this
    Vector3 moveDirection = Vector3.zero;

    //Bullet to shoot
    public GameObject bulletToShoot;

    //Location to spawn
    public GameObject locationToSpawn;

    //to help check if the gun is reloading
    bool reloading;

    //Add a timer to the current reloading state
    int timer;

    //Used to store the full ammo amount
    int fullAmmo;

    //Sounds for each different state
    public AudioClip reloadingSound;
    public AudioClip shootingSound;
    public AudioClip outofbulletsSound;

    //types
    public bool flamethrower;
    public bool semiautovisualgun;
    public bool semiautogun;

    //Apply a muzzle flash
    public GameObject muzzleFire;

    //Gets the source to apply the audio clips to!
    AudioSource source;

    //The particleeffect for a flamethrower
    public GameObject flame;

    //Add a rotating barrel effect!
    public GameObject rotatinbarrel;

    //options to go up and down
    public bool pivotupdown;
    public int maxY;
    public int minY;

    public void Start()
    {
        fullAmmo = ammo;

        if(source == null)
        {
            this.gameObject.AddComponent<AudioSource>();
        }

        source = GetComponent<AudioSource>();
        source.playOnAwake = false;

        //some verification method
        if(flame == null)
        {
            flamethrower = false;
            semiautogun = true;
        }
    }

    void Update()
    {
        //Check if the mouse button is down
        if (Input.GetKey(KeyCode.Mouse0))
        {
            shooting();
        }
        else
        {
            stopShooting();
        }
        movement();
        reloadingMech();
    }

    public void stopShooting()
    {
        if (!flamethrower)
        {
            //Disable the muzzleflash
            muzzleFire.SetActive(false);

        }
        //set the flame to false
        flame.SetActive(false);
    }

    public void shooting()
    {
        //check if the player has enough ammo clips
        //the reason the ammo clips is on the outside of a know bug which would cause the gun to well i dont really know
        if (ammoclips >= 1)
        {
            //check if the player is not reloading
            if (reloading == false)
            {
                if (ammo >= 1)
                {
                    for (int i = 0; i < bulletssec; i++)
                    {
                        //Play the shooting sound
                        source.clip = shootingSound;
                        source.Play();
                        if (semiautovisualgun)
                        {
                            //Shoot the ammo
                            var bulletPrefab = Instantiate(bulletToShoot, locationToSpawn.transform.position, rotatinbarrel.transform.rotation);
                            bulletPrefab.AddComponent<TimeOut>();
                            // Add velocity to the bullet
                            bulletPrefab.GetComponent<Rigidbody>().velocity = -transform.forward * 18 * speed * distance;
                            //decreasing the ammo count
                            //to get the attacked thing like to apply damage to another object you would have to setup a trigger on the semi auto visual bullet which is a cube for testing
                        }
                        else if (semiautogun)
                        {
                            RaycastHit hit;

                            if (Physics.Raycast(transform.position, -Vector3.up, out hit, distance))
                                if (hit.collider.gameObject != null)
                                {
                                    //do whatever
                                    //like for example
                                    if (hit.collider.gameObject.tag == "Player")
                                    {
                                        Destroy(hit.collider.gameObject);
                                    }
                                    else
                                    {
                                        //Friendly
                                    }
                                }
                        }
                        else if (flamethrower)
                        {
                            flame.SetActive(true);
                            RaycastHit hit;

                            if (Physics.Raycast(transform.position, -Vector3.up, out hit, distance))
                                if (hit.collider.gameObject != null)
                                {
                                    //do whatever
                                    //like for example
                                    if (hit.collider.gameObject.tag == "Player")
                                    {
                                        Destroy(hit.collider.gameObject);
                                    }
                                    else
                                    {
                                        //Friendly
                                    }
                                }
                        }
                        else
                        {
                        }

                        //check if flame thrower if not set the muzzle flash to true
                        if (flamethrower == false)
                        {
                            if (ammo > 0)
                            {
                                //set it to true
                                muzzleFire.SetActive(true); 
                            }
                        }

                        //rotate the barrel if it is available
                        if (rotatinbarrel != null)
                        {
                            //barrel avaliable so rotate
                            //Times 20 added to give a more speed
                            rotatinbarrel.transform.Rotate(transform.up * Time.deltaTime * speed * 20);
                        }

                        ammo--;
                    }
                }
                else
                {
                    //enter a reload state
                    reloading = true;
                }
            }
        }
    }

    public void movement()
    {
        //to add movement like rotation
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (pivotupdown)
        {
            if(vertical >= maxY)
            {
                vertical = maxY;
            }
            else if(vertical <= minY)
            {
                vertical = minY;
            }
        }
        moveDirection = new Vector3(0, -horizontal, 0);
        moveDirection *= speed;
        transform.Rotate(moveDirection * Time.deltaTime);
    }

    public void reloadingMech()
    {
        //magazine count is greater than zero continue!
        if (ammoclips >= 1)
        {
            //Reloading is true soo reload!
            if (reloading == true)
            {
                //Play the reloading sound
                source.clip = reloadingSound;
                source.Play();
                //Increasing the timer
                timer++;
                //if timer is greater than reloadduration then we want to continue
                //the times 60 is there to make it into seconds instead of milliseconds
                if (timer >= reloadduration * 60)
                {
                    //setting the settings back to normal decreasing mag count
                    ammo = fullAmmo;
                    ammoclips -= 1;
                    //setting state back to reloading false to return to default state
                    reloading = false;
                }
            }
        }
        else
        {
            //out of ammo
            source.clip = outofbulletsSound;
            source.Play();
        }
    }
}

public class TimeOut : MonoBehaviour
{
    //This script is used to remove the objects that are laying around in the scene like the bullets and flames
    int timer;

    public void Update()
    {
        timer++;
        if(timer >= 300)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "")
        {
            //Bullet hit something that does not have a tag
            Destroy(gameObject);
        }
    }
}