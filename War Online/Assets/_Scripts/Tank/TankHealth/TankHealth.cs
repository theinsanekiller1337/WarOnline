using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : Photon.PunBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;
    

    //public GameObject m_ExplosionPrefab;

    private string playerTankName;
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;   
    public float m_CurrentHealth;  
    private bool m_Dead;
    private gameManager photonScript;
    private bool destroyCalled;
    

    private void Start()
    {
        GameObject warslider = GameObject.Find("WarSlider");
        Slider slider = warslider.GetComponent<Slider>();
        m_Slider = slider;
        GameObject health = GameObject.Find("Health");
        Image healthImage = health.GetComponentInChildren<Image>();
        m_FillImage = healthImage;

        m_Dead = false;

        m_CurrentHealth = m_StartingHealth;
        photonScript = GameObject.Find("GameManager").GetComponent<gameManager>();

        destroyCalled = false;
    }

    private void Awake()
    {
        //   m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        // m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        // m_ExplosionParticles.gameObject.SetActive(false);
        m_Dead = false;
        
    }
	

    private void Update()
    {

        SetHealthUI();

        if (m_CurrentHealth <= 0)
        {

            OnDeath();

        }

        if (Input.GetKeyDown("j"))
        {
            TakeDamage(25);
        }
        else if (Input.GetKeyDown("u"))
        {
            m_CurrentHealth = 0f;
            OnDeath();
        }
    }
    

    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

		m_CurrentHealth -= amount;
		SetHealthUI();

		if(m_CurrentHealth <= 0 ){
		
		OnDeath();

		}

    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
		
		m_Slider.value = m_CurrentHealth;
		m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth/m_StartingHealth);

    }


     void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.

		m_Dead = true;

        Vector3 newVelocity = this.GetComponentInChildren<Rigidbody>().velocity;
        Vector3 newAngularVelocity = this.GetComponentInChildren<Rigidbody>().angularVelocity;
        Vector3 newDesPos = this.GetComponentInParent<Transform>().position;
        Quaternion newDesRos = this.GetComponentInParent<Transform>().rotation;
        Destroy(this.gameObject);

        if (!destroyCalled)
        {
            playerTankName = photonScript.newPlayerPrefab;
            GameObject playerDes = (GameObject)PhotonNetwork.Instantiate(Path.Combine("Destroyed", playerTankName + "D"), newDesPos, newDesRos, 0);
            playerDes.GetComponent<Rigidbody>().velocity = newVelocity;
            playerDes.GetComponent<Rigidbody>().angularVelocity = newAngularVelocity;
            playerDes.transform.Find("MainCamera").gameObject.SetActive(true);
            Destroy(playerDes, 1.3f);
            destroyCalled = true;
            //  Physics.IgnoreCollision(this.GetComponentInParent<Transform>().gameObject.GetComponentInChildren<Collider>(), playerDes.GetComponent<Collider>());
        }
        
      }
      
   
    /*
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionParticles.gameObject.SetActive (true);

        m_ExplosionParticles.Play();*/

    //m_ExplosionAudio.Play();
}