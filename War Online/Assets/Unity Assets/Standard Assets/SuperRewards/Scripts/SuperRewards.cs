using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Playerize
{
	enum OfferWallStatus { NotLoaded, Loading, Loaded, CannotLoad }


	/// <summary>
	/// Functions to show and hide the SuperRewards Offer Wall.
	/// </summary>
	public static class SuperRewards
	{
		/// <summary>
		/// The SuperRewards Unity client version.
		/// </summary>
		public const string version = "1.0.1";

		private const string click_source = "45";

		public static bool offerWallVisible
		{
			get { return gui == null ? false : gui.enabled; }
		}

		public static DirectOffer[] directOffers { get; private set; }
		public static Offer[] offers { get; private set; }
		public static PayOffer[] payOffers { get; private set; }
		public static bool debug { internal get; set; }
		public static Texture2D currencyIcon;

		static string appCode;
		static string userID;
		static Action<int> pointsChangedCallback;

		const string superRewardsInstallPref = "Super_Rewards_Install_Tracked";
		const string superRewardsPointsPref = "Super_Rewards_Total_Points_";

		internal static OfferWallStatus status { get; private set; }
		static SuperRewardsGUI gui;

		static int connectionAttempts;
		const int maxConnectionAttempts = 3;

		public static void Initialize (string superRewardsAppCode, string customUserID, Action<int> onUserPointsChanged, bool preloadOffers=false)
		{
			// Ensure the App Code has been set.
			if (string.IsNullOrEmpty(superRewardsAppCode)) {
				Debug.LogError("[SuperRewards] An App Code must be supplied.");
				return;
			}

			// Ensure the user ID has been set.
			if (string.IsNullOrEmpty(customUserID)) {
				Debug.LogError("[SuperRewards] A non-empty user ID must be supplied.");
				return;
			}

			// Ensure a callback for user points changed exists
			if (onUserPointsChanged == null) {
				Debug.LogError("[SuperRewards] A callback method for onUserPointsChanged must be supplied.");
				return;
			}

			appCode = superRewardsAppCode;
			userID = customUserID;
			pointsChangedCallback = onUserPointsChanged;

			// Load offers in background if preload offers is true
			if (preloadOffers && status == OfferWallStatus.NotLoaded) {
				LoadOffers();
			}

			// Check if we have tracked this install
			if (PlayerPrefs.GetInt(superRewardsInstallPref, 0) == 0) {
				TrackInstall();
			}

			// Check if the player is owed any outstanding rewards
			GetUserRewards();
		}

		/// <summary>
		/// Displays the offer wall.
		/// </summary>
		/// <param name="appCode">Your app's unique identifier.</param>
		/// <param name="userID">A unique identifier for the player.</param>
		public static void ShowOfferWall ()
		{
			EnsureGUIExists();

			// Loads offers if we haven't done so yet.
			if (status == OfferWallStatus.NotLoaded) {
				LoadOffers();
			}

			gui.enabled = true;
		}

		/// <summary>
		/// Removes the offer wall from view.
		/// </summary>
		public static void HideOfferWall ()
		{
			gui.enabled = false;
		}

		/// <summary>
		/// Executes a Unity coroutine.
		/// </summary>
		/// <param name="routine">The routine to execute.</param>
		/// <returns>A Coroutine object that allows suspension of execution.</returns>
		internal static Coroutine RunRoutine (IEnumerator routine)
		{
			EnsureGUIExists();
			return gui.StartCoroutine(routine);
		}

		/// <summary>
		/// Fetches the SuperRewards Offer Wall.
		/// </summary>
		/// <param name="appCode">Your app's unique identifier.</param>
		/// <param name="userID">A unique identifier for the player.</param>
		static void LoadOffers ()
		{
			status = OfferWallStatus.Loading;
			var url = "https://www.superrewards-offers.com/super/offers?h=" + appCode + "&uid=" + userID + "&xml=1&n_offers=36&ip=0.0.0.0&v=3&cs=" + click_source + "&tags=mobile%20-%20unity&sdkv=" + version;

			connectionAttempts += 1;

			Requests.MakeRequest(url, null,
				(xml) => { // Success
					var result = xml as XMLNode;
					connectionAttempts = 0;

					var data = result["offers"].children;
					var offerList = new List<Offer>();
					var payOfferList = new List<PayOffer>();
					var directOfferList = new List<DirectOffer>();

					// Reward offers
					foreach (XMLNode offerData in data) {
						// Free Offers
						if (offerData["image"].value != null) {
							var offer = new Offer(offerData);
							offerList.Add(offer);
						}
						// Pay Offers
						else {
							var payOffer = new PayOffer(offerData);
							payOfferList.Add(payOffer);
						}
					}

					// Direct pay offers
					foreach (var dp in result["directpay"]["providers"].children) {
						var directOffer = new DirectOffer(dp);
						directOfferList.Add(directOffer);
					}

					directOffers = directOfferList.ToArray();
					payOffers = payOfferList.ToArray();
					offers = offerList.ToArray();
					status = OfferWallStatus.Loaded;
				},
				() => { // Error
					var message = "[SuperRewards] Unable to load offer wall.";

					if (connectionAttempts < maxConnectionAttempts) {
						message += " Trying again.";
						LoadOffers();
					} else {
						status = OfferWallStatus.CannotLoad;
					}

					Debug.LogError(message);
				});
		}

		/// <summary>
		/// Tracks the install of this game.
		/// </summary>
		static void TrackInstall ()
		{
			var url = "https://www.superrewards-offers.com/mobile/rpc";
			var json = GetJSON("install");
			var postData = getPostData(json);

			Requests.MakeRequest(url, postData,
			    (result) => { // Success
					PlayerPrefs.SetInt(superRewardsInstallPref, 1);
					connectionAttempts = 0;
					Debug.Log("[SuperRewards] Install successfully tracked.");
				},
				() => { // Error
					var message = "[SuperRewards] Unable to track install.";

					if (connectionAttempts < maxConnectionAttempts) {
						message += " Trying again.";
						connectionAttempts++;
						TrackInstall();
					}

					Debug.LogError(message);
				}
			);
		}

		public static void GetUserRewards ()
		{
			// Ensure that SuperRewards has been initialized
			if (pointsChangedCallback == null) {
				Debug.LogError("[SuperRewards] You must initialize SuperRewards before calling this method.");
				return;
			}

			var url = "https://www.superrewards-offers.com/mobile/rpc";
			var json = GetJSON("check_points");
			var postData = getPostData(json);

			Requests.MakeRequest(url, postData,
				(data) => { // Success
					var result = data as JSONNode;
					var newPoints = result["response"]["data"]["new"].AsInt;
					var totalPoints = result["response"]["data"]["total"].AsInt;

					if (debug) {
						Debug.Log("[SuperRewards] Received user rewards: " + newPoints + " new, " + totalPoints + " total.");
					}

					var prefsKey = superRewardsPointsPref + userID;

					// Check if we already awarded these points
					if (PlayerPrefs.GetInt(prefsKey, 0) == totalPoints) {
						newPoints = 0;
					// Save the new total points
					} else {
						PlayerPrefs.SetInt(prefsKey, totalPoints);
					}

					connectionAttempts = 0;

					pointsChangedCallback(newPoints);
				},
				() => { // Error
					var message = "[SuperRewards] Unable to retrieve user's reward points.";

					if (connectionAttempts < maxConnectionAttempts) {
						message += " Trying again.";
						GetUserRewards();
					}

					Debug.LogError(message);
				}
			);
		}

		static string GetJSON (string method)
		{
			// Seperate OS type and version info
			var os = SystemInfo.operatingSystem;
			var versionIndex = os.LastIndexOf(' ');
			var osVersion = os.Substring(versionIndex + 1);
			var osType = os.Substring(0, versionIndex);

			// Device Unique Identifier
			var uid = SystemInfo.deviceUniqueIdentifier.ToString();

			// Country Code
			var cc = "US";

			var json = new JSONClass();

			json["method"] = method;
			json["response_type"] = "json";
			json["params"]["device"]["os"] = osType;
			json["params"]["device"]["model"] = "sdk";
			json["params"]["device"]["device_id"] = uid;
			json["params"]["device"]["type"] = "Unity";
			json["params"]["device"]["alt_id"] = "";
			json["params"]["device"]["sim_serial"] = "";
			json["params"]["device"]["cc"] = cc;
			json["params"]["device"]["h"] = appCode;
			json["params"]["device"]["os_version"] = osVersion;
			json["params"]["device"]["ip"] = "0.0.0.0";
			json["params"]["device"]["version"] = version;
			json["params"]["h"] = appCode;
			json["params"]["check_points"]["uid"] = userID;
			json["params"]["geo"]["cc"] = cc;
			json["params"]["geo"]["ip"] = "0.0.0.0";
			json["params"]["cs"] = click_source;

			return json.ToString();
		}

		static WWWForm getPostData (string json)
		{
			var postData = new WWWForm();
			postData.AddField("h", appCode);
			postData.AddField("uid", userID);
			postData.AddField("xml", 1);
			postData.AddField("v", 3);
			postData.AddField("cc", "US");
			postData.AddField("ip", "0.0.0.0");
			postData.AddField("json", json);
			return postData;
		}

		/// <summary>
		/// Creates a SuperRewards game object if one doesn't exist yet.
		/// </summary>
		static void EnsureGUIExists ()
		{
			if (gui == null) {
				var go = new GameObject("SuperRewards", typeof(SuperRewardsGUI));
				go.hideFlags = HideFlags.HideInHierarchy;
				gui = go.GetComponent<SuperRewardsGUI>();
				gui.enabled = false;
			}
		}
	}
}
