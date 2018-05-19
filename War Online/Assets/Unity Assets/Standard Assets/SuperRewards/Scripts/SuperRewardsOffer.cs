using System;
using UnityEngine;

namespace Playerize
{
	/// <summary>
	/// Parent class for offers.
	/// </summary>
	public abstract class SuperRewardsOffer
	{
		public enum Type { Pay, Free, Paid, Mobile }

		public readonly Type type;
		public readonly int id;
		public readonly string name;
		public readonly string description;
		public string url;
		public Texture2D image { get; private set; }

		int imageLoadAttempts = 1;

		internal SuperRewardsOffer (XMLNode data)
		{
			id = Convert.ToInt32(data.attr["id"]);

			if (data["name"] != null) {
				name = data["name"].value;

				// Direct offer special cases
				switch (name) {
					case "PaypalEC": 		name = "PayPal"; 			break;
					case "Paymo": 			name = "Mobile Payment"; 	break;
					case "MoneybookersCC":	name = "Moneybookers"; 		break;
				}
			} else {
				name = data["title"].value;
			}

			description = data["description"].value;

			if (description != null && description.IndexOf("<br />") != -1) {
				description = description.Replace("<br />", "\n");
			}

			url = data["click_url"].value;

			// Don't do this for direct offers.
			if (data["price_points"] == null) {
				switch (data.attr["offer_type"]) {
					case "paid":   type = Type.Paid;   break;
					case "mobile": type = Type.Mobile; break;
					case "free":   type = Type.Free;   break;
					default:       type = Type.Pay;    break;
				}
			}

			// Download the image associated with this offer
			if (data["image"] != null) {
				LoadImage(data["image"].value);
			}
		}

		void LoadImage (string imageURI)
		{
			Requests.LoadImage(imageURI, 
			    loadedImage => {
					this.image = loadedImage;
				},
				() => {
					if (imageLoadAttempts < 3) {
						imageLoadAttempts++;
						LoadImage(imageURI);
					} else {
						Debug.LogError("[SuperRewards] Aborting loading image after " + imageLoadAttempts + " from " + imageURI);
					}
				}
			);
		}
	}
}
