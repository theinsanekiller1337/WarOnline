using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TankHealth : Photon.PunBehaviour
{
    public float m_StartingHealth = 100f;
    public Slider m_Slider;
    public Image m_FillImage;
    public Color m_FullHealthColor;
    public Color m_ZeroHealthColor;
    public GameObject actualHull;
    public GameObject actualTurret;

    [HideInInspector]
    public GameObject destroyedTurret;
   
    public GameObject warCanvas;


    //public GameObject m_ExplosionPrefab;

    private bool spawnCalled;
    private GameObject destroyedHull;
    private string playerTankName;
    private AudioSource m_ExplosionAudio;
    private ParticleSystem m_ExplosionParticles;
    public float m_CurrentHealth;
    private bool m_Dead;
    private gameManager photonScript;
    private bool destroyCalled;
    


    private void Awake()
    {
        GameObject canvas = gameObject.transform.Find("WarCanvas").gameObject;
        canvas.SetActive(true);
        warCanvas = canvas;
        
        GameObject warslider = canvas.transform.Find("TankHealthUI").gameObject;
        
        Slider slider = warslider.GetComponent<Slider>();
        m_Slider = slider;
        GameObject health = warslider.transform.Find("Health").gameObject;
        Image healthImage = health.GetComponentInChildren<Image>();
        m_FillImage = healthImage;

        m_Dead = false;

        m_CurrentHealth = m_StartingHealth;
        photonScript = GameObject.Find("GameManager").GetComponent<gameManager>();
       
        //getting destroyed tank prefabs
        destroyedHull = gameObject.transform.Find(actualHull.name + "_D").gameObject;
        

        destroyCalled = false;
        GameObject mainCanvas = GameObject.Find("MainWarCanvas");
        canvas.transform.parent = mainCanvas.transform;

        m_Dead = false;
    }




    private void Update()
    {

        SetHealthUI(m_CurrentHealth);

        if (m_CurrentHealth <= 0)
        {

            OnDeath();

        }
        
        if (Input.GetKeyDown("u"))
        {
            m_CurrentHealth = 0f;
        }
    }

    [PunRPC]
    public void TakeDamage(float damage)
    {
        // Adjust the tank's current health, update the UI based on the new health and check whether or not the tank is dead.

        m_CurrentHealth -= damage;

        SetHealthUI(m_CurrentHealth);

        photonView.RPC("UpdateHealth", PhotonTargets.Others, m_CurrentHealth);

        if (m_CurrentHealth <= 0)
        {

            OnDeath();

        }

    }

    [PunRPC]
    private void SetHealthUI(float health)
    {
        // Adjust the value and colour of the slider.

        m_Slider.value = health;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, health / m_StartingHealth);

    }


    void OnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.

        m_Dead = true;


        if (!destroyCalled)
        {
            actualHull.SetActive(false);
            actualTurret.SetActive(false);

            destroyedTurret.SetActive(true);
            destroyedHull.SetActive(true);
            destroyCalled = true;
            RTCTankController tankController = gameObject.GetComponent<RTCTankController>();
            tankController.engineRunning = false;
        }

        StartCoroutine(Destroying());

    }

    private IEnumerator Destroying()
    {
        yield return new WaitForSeconds(1.8f);

        PhotonNetwork.Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        if (!spawnCalled)
        {
            photonScript.SpawnTank();
            spawnCalled = true;
            Destroy(warCanvas);
        }
    }
}