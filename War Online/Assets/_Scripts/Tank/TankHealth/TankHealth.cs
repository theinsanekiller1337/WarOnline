using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TankHealth : Photon.PunBehaviour
{
    public float m_StartingHealth = 100f;
    public Slider m_Slider;
    public Image m_FillImage;
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    public GameObject actualHull;



    //public GameObject m_ExplosionPrefab;

    private bool spawnCalled;
    private GameObject actualTurret;
    private GameObject destroyedHull;
    private GameObject destoyedTurret;
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

        //gettingTurrets
        if (gameObject.transform.Find("FlameThrower") != null)
        {
            GameObject ftTurret = gameObject.transform.Find("FlameThrower").gameObject;
            actualTurret = ftTurret.transform.Find("FlameThrower_Body").gameObject;
            destoyedTurret = ftTurret.transform.Find("FlameThrower_Body_D").gameObject;

        }
        else if (gameObject.transform.Find("Sniper") != null)
        {
            actualTurret = gameObject.transform.Find("Sniper_Body").gameObject;
            destoyedTurret = gameObject.transform.Find("Sniper_Body_D").gameObject;

        }
        else if (gameObject.transform.Find("MachineGun") != null)
        {
            actualTurret = gameObject.transform.Find("MachineGun_Body").gameObject;
            destoyedTurret = gameObject.transform.Find("MachineGun_Body_D").gameObject;
        }

        //getting destroyed tank prefabs
        destroyedHull = gameObject.transform.Find(actualHull.name + "_D").gameObject;


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
        
        if (Input.GetKeyDown("u"))
        {
            m_CurrentHealth = 0f;
        }
    }


    public void TakeDamage(float amount)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        m_CurrentHealth -= amount;
        SetHealthUI();

        if (m_CurrentHealth <= 0)
        {

            OnDeath();

        }

    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.

        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);

    }


    void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.

        m_Dead = true;


        if (!destroyCalled)
        {
            actualHull.SetActive(false);
            actualTurret.SetActive(false);

            destoyedTurret.SetActive(true);
            destroyedHull.SetActive(true);
            destroyCalled = true;
            RTCTankController tankController = gameObject.GetComponent<RTCTankController>();
            tankController.engineRunning = false;
        }

        StartCoroutine(Destroying());

    }

    private IEnumerator Destroying()
    {
        yield return new WaitForSeconds(2.5f);

        PhotonNetwork.Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if (!spawnCalled)
        {
            photonScript.SpawnTank();
            spawnCalled = true;
        }
    }
}