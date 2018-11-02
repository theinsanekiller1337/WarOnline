using UnityEngine;
using System.Collections;

public class LaunchURL : MonoBehaviour {

	public string URLi; 

	public void urlLinkOrWeb() 
	{
		Application.OpenURL(URLi);
	}
}
