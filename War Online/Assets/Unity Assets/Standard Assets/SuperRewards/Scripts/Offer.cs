using UnityEngine;
using System;

namespace Playerize
{
	/// <summary>
	/// Holds information about an offer.
	/// </summary>
	public class Offer : SuperRewardsOffer
	{
		public readonly double payout;
		public readonly string requirements;

		internal Offer (XMLNode data) : base(data)
		{
			this.payout = Math.Round(Convert.ToSingle(data["payout"].value), 2);
			this.requirements = data["requirements"].value;
		}
	}
}
