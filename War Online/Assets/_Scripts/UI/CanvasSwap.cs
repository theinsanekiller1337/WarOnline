using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSwap : MonoBehaviour {

    public Canvas NewCanvas;
    public Canvas OldCanvas;
    public Button btn;

	// Use this for initialization
	void Start () {
        Button btnclick = btn.GetComponent<Button>();
        btnclick.onClick.AddListener(TaskOnClick);
	}

    void TaskOnClick()
    {
        NewCanvas.gameObject.SetActive(true);
        OldCanvas.gameObject.SetActive(false);
    }
}
