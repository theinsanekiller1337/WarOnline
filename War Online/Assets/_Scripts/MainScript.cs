using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour {

		public void LoadScene() {

			SceneManager.LoadScene (1);

		} 

	public void LoadScene0() {
		SceneManager.LoadScene (0);
	}

	public void LoadScene2 () {
		SceneManager.LoadScene (2);
	}

	public void LoadScene3 () {
		SceneManager.LoadScene (3);
	}

	}
