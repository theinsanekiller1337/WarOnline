using System.Collections.Generic;

namespace Playerize
{
	/// <summary>
	/// Holds information about a direct offer.
	/// </summary>
	public class DirectOffer : SuperRewardsOffer
	{
		public struct PricePoint
		{
			public string description;
			public float amount;
		}

		public readonly string shortName;
		public readonly string shortDescription;
		public readonly string longDescription;
		public readonly string[] paymentMethods;
		public readonly PricePoint[] pricePoints;

		internal DirectOffer (XMLNode data) : base(data)
		{
			if (data["short_name"] != null) {
				this.shortName = data["short_name"].value;
			}

			this.shortDescription = data["short_description"].value;
			this.longDescription = data["long_description"].value;
			this.paymentMethods = ParsePaymentMethods(data["types"].children);
			this.pricePoints = ParsePricePoints(data["price_points"].children);
			this.url = data["click_url"].value;
		}

		static string[] ParsePaymentMethods (List<XMLNode> data)
		{
			var methods = new string[data.Count];

			for (int i = 0; i < data.Count; i++) {
				methods[i] = data[i].value;
			}

			return methods;
		}

		static PricePoint[] ParsePricePoints (List<XMLNode> data)
		{
			var points = new PricePoint[data.Count];

			for (int i = 0; i < data.Count; i++) {
				points[i] = new PricePoint() {
					description = data[i].attr["us_text"],
					amount = System.Convert.ToSingle(data[i].attr["amount"])
				};
			}

			return points;
		}
	}
}
