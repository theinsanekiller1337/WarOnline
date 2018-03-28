using Playerize;
using UnityEngine;

public class SuperRewardsDemo : MonoBehaviour
{
	public string appCode;
	public string userID;
	public bool debug;

	Rect buttonRect;

	void Awake ()
	{
		SuperRewards.debug = debug;
		SuperRewards.Initialize(appCode, userID, OnUserPointsChanged, true);
		const int padding = 100;
		buttonRect = new Rect(padding, padding, Screen.width - (padding * 2), Screen.height - (padding * 2));
	}

	void OnUserPointsChanged (int pointsEarned)
	{
		if (pointsEarned > 0) {
			Debug.Log("The user earned " + pointsEarned + " of our game's virtual currency, give them this amount now.");
		}
	}

	void OnGUI ()
	{
		if (GUI.Button(buttonRect, "Show SuperRewards Offer Wall")) {
			SuperRewards.ShowOfferWall();
		}
	}
}
