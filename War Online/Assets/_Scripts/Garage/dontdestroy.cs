using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class dontdestroy : MonoBehaviour
{
    public GameObject getfrom;
    public string[] hulls;
    public string[] turrets;
    public string prefab = "";
    public GameObject game_manager;
    public static dontdestroy instance;
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); }
        else { instance = this; }
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            DontDestroyOnLoad(gameObject);
        }
        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            //game_manager = GameObject.Find("GameManager");
            


        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("name" + (SceneManager.GetActiveScene().name));
        Debug.Log("buildindex" + SceneManager.GetActiveScene().buildIndex);
        if (SceneManager.GetActiveScene().name == "Lobby")
        {
            prefab = hulls[getfrom.GetComponent<HullChange>().selection];
            prefab += turrets[getfrom.GetComponent<TurretChange>().selection];
            Debug.Log(prefab);
            Debug.Log(SceneManager.GetActiveScene().name);
        }
    }
}
