using Playerize;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SuperRewardsGUI : MonoBehaviour
{
	/// <summary>
	/// True if we're using a mobile device.
	/// This is used to implement appropriate scrolling behaviour.
	/// </summary>
	#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY)
	static readonly bool isMobile = true;
	#else
	static readonly bool isMobile = false;
	#endif

	static readonly GUIContent closeButtonLabel = new GUIContent("Close", "Close the SuperRewards offer window.");
	static readonly GUIContent[] tabLabels = new GUIContent[] {
		new GUIContent("Earn", "Complete offers."),
		new GUIContent("Buy", "Purchase directly.")
	};

	string urlToOpen;
	int _tabIndex;
	int tabIndex {
		get {
			return _tabIndex;
		}
		set {
			if (value != _tabIndex) {
				urlToOpen = null;
				currentPage = 1;
				calculatedPage = false;
			}

			_tabIndex = value;
		}
	}

	DirectOffer selectedOffer;
	DirectOffer.PricePoint selectedPrice;
	bool[] toggleIndex;
	int lastToggleIndex = 0;

	// Scrolling:
	Vector2 scrollPosition;
	float inertiaDuration = 0.5f;
	int scrollVelocity;
	float timeTouchPhaseEnded;
	float timeTouchPhaseStarted;
	const float minimumButtonPressTime = 0.15f;

	// Paging
	int pageLimit = 10;
	int startingIndex = 0;
	int pageSize = 0;
	bool canPageNext;
	bool calculatedPage;
	SuperRewardsOffer[] page;
	int _currentPage = 1;
	int currentPage {
		get {
			return _currentPage;
		}
		set {
			_currentPage = value;
			scrollPosition = Vector2.zero;
		}
	}

	/// <summary>
	/// Checks whether a button press is legitimate.
	/// This is important on mobile where a scroll can be confused for a click.
	/// </summary>
	bool canPressButton
	{
		get {
			if (Application.isEditor || !isMobile) {
				return true;
			} else {
				return (Time.time - timeTouchPhaseStarted) < minimumButtonPressTime;
			}
		}
	}

	// Scaling:

	const float nativeWidth = 2048;
	float scale;
	Matrix4x4 matrix;
	float scaledWidth;
	float scaledHeight;
	Rect fullScreenRect;
	Rect windowRect;

	// Layout constants:

	const float padding = 40;
	const float scaledImageWidth = 480;
	const float horizontalSpacer = 40;
	const float verticalMargin = 40;
	const float currencyIconWidth = 193;
	const float currencyIconHeight = 200;
	const float arrowIconWidth = 53;
	const float arrowIconHeight = 100;
	const float minimumImageHeight = 100;

	// Assets:

	GUISkin guiSkin;
	Texture2D logo;
	Texture2D overlayTexture;
	Texture2D arrowTexture;
	public Texture2D currencyTexture { get; set; }

	/// <summary>
	/// Performs setup steps for the offer wall.
	/// </summary>
	void Awake ()
	{
		// Load resources.
		guiSkin = LoadResource<GUISkin>("skin");
		logo = LoadResource<Texture2D>("logo");
		overlayTexture = LoadResource<Texture2D>("overlay");
		arrowTexture = LoadResource<Texture2D>("arrow");
		currencyTexture = SuperRewards.currencyIcon == null ? LoadResource<Texture2D>("coins") : SuperRewards.currencyIcon;

		SetUISizes();
	}

	void Update ()
	{
		if (selectedOffer != null && toggleIndex != null) {
			for (int i = 0; i < toggleIndex.Length; i++) {
				if (toggleIndex[i] && i != lastToggleIndex) {
					toggleIndex[lastToggleIndex] = false;
					lastToggleIndex = i;
				}
			}
		}
	}

	/// <summary>
	/// Displays the SuperRewards offer wall.
	/// </summary>
	void OnGUI ()
	{
		GUI.depth = -1000;
		var newScale = Screen.width / nativeWidth;

		// Update the scaling if the screen size changes.
		if (newScale != scale) {
			scale = newScale;
			SetUISizes();
		}

		// Store current GUI skin and temporarily switch to SuperRewards theme.
		var gameGUIMatrix = GUI.matrix;
		var gameGUISkin = GUI.skin;

		GUI.matrix = matrix;
		GUI.skin = guiSkin;

		// Display background overlay.
		GUI.DrawTexture(fullScreenRect, overlayTexture);

		// Display offer wall window.
		GUILayout.BeginArea(windowRect);
		DisplayOfferWall();
		GUILayout.EndArea();

		if (isMobile) {
			MobileScroll();
		}

		// Set the GUI skin back to what the game was using.
		GUI.matrix = gameGUIMatrix;
		GUI.skin = gameGUISkin;
	}

	/// <summary>
	/// Displays the offer wall window.
	/// </summary>
	void DisplayOfferWall ()
	{
		DisplayHeader();

		GUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandHeight(true));

			switch (tabIndex) {
				case 0: // Earn
					if (urlToOpen != null) {
						DisplayOpenBrowserConfirmation();
					} else {
						switch (SuperRewards.status) {
							case OfferWallStatus.Loaded:
								DisplayOffers();
								break;
							case OfferWallStatus.Loading:
								DisplayLoadingScreen();
								break;
							case OfferWallStatus.CannotLoad:
								GUILayout.Label("Unable to load offers.", GUI.skin.GetStyle("centeredLabel"));
								break;
						}
					}

					break;
				case 1: // Buy:
					if (urlToOpen != null) {
						DisplayOpenBrowserConfirmation();
					} else if (selectedOffer != null) {
						DisplayPricePoints(selectedOffer);
					} else {
						DisplayDirectPayments();
					}
					break;
			}

		GUILayout.EndVertical();

		// Display close button.
		AlignHorizontal(delegate {
			if (GUILayout.Button(closeButtonLabel)) {
				SuperRewards.HideOfferWall();
			}
		}, TextAlignment.Right);
	}

	/// <summary>
	/// Draws the SuperRewards banner and tabbed navigation.
	/// </summary>
	void DisplayHeader ()
	{
		AlignHorizontal(delegate {
			GUILayout.Label(logo);
		});

		tabIndex = GUILayout.Toolbar(tabIndex, tabLabels, GUI.skin.GetStyle("tab"));
	}

	/// <summary>
	/// Draws a loading screen.
	/// </summary>
	void DisplayLoadingScreen ()
	{
		AlignVertical(delegate {
			GUILayout.Label("Loading offers...", GUI.skin.GetStyle("centeredLabel"));
		});
	}

	/// <summary>
	/// Draws a scrollable list of available offers.
	/// </summary>
	void DisplayOffers ()
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		if (SuperRewards.offers.Length > 0) {
			PageContent(SuperRewards.offers, () => {
				for (int i = 0; i < page.Length; i++) {
					if (i > 0) {
						DisplayDivider();
					}
					
					DisplayOffer(page[i] as Offer);
				}
			});
		} else {
			AlignVertical(delegate {
				GUILayout.Label("Sorry, no offers are currently available.", GUI.skin.GetStyle("centeredLabel"));
			});
		}

		GUILayout.EndScrollView();
	}

	/// <summary>
	/// Draws a single offer.
	/// </summary>
	/// <param name="offer">Offer to display.</param>
	void DisplayOffer (Offer offer)
	{
		GUILayout.BeginHorizontal();

			var height = GetScaledImageHeight(offer.image, scaledImageWidth);
			var imageRect = GetRect(scaledImageWidth, height);

			if (offer.image != null) {
				GUI.DrawTexture(imageRect, offer.image);
			}

			GUILayout.Space(horizontalSpacer);

			GUILayout.BeginVertical(GUILayout.ExpandWidth(true));

				GUILayout.Label(offer.name, GUI.skin.GetStyle("title"));
				GUILayout.Label(offer.description);

			GUILayout.EndVertical();

			GUILayout.Space(horizontalSpacer);

			AlignVertical(delegate {
				AlignHorizontal(delegate {
					var currencyRect = GetRect(currencyIconWidth, currencyIconHeight);
					GUI.DrawTexture(currencyRect, currencyTexture);
				}, TextAlignment.Right);
				GUILayout.Space(10);
				GUILayout.Label("+" + offer.payout, GUI.skin.GetStyle("currency"));
			}, currencyIconWidth);

		GUILayout.EndHorizontal();

		var lastRect = GUILayoutUtility.GetLastRect();

		// Make offer clickable.
		if (GUI.Button(lastRect, GUIContent.none, GUIStyle.none) && canPressButton) {
			urlToOpen = offer.url;
		}
	}

	void DisplayDirectPayments ()
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		if (SuperRewards.offers != null && SuperRewards.offers.Length > 0) {
			for (int i = 0; i < SuperRewards.directOffers.Length; i++) {
				if (i > 0) {
					DisplayDivider();
				}

				DisplayDirectPayment(SuperRewards.directOffers[i]);
			}
		} else {
			AlignVertical(delegate {
				GUILayout.Label("Sorry, no offers are currently available.", GUI.skin.GetStyle("centeredLabel"));
			});
		}

		GUILayout.EndScrollView();
	}

	void DisplayDirectPayment (DirectOffer payment)
	{
		GUILayout.BeginHorizontal();

		var height = GetScaledImageHeight(payment.image, scaledImageWidth);
		var imageRect = GetRect(scaledImageWidth, height);

		if (payment.image != null) {
			GUI.DrawTexture(imageRect, payment.image);
		}

		GUILayout.Space(horizontalSpacer);

		GUILayout.Label(payment.name, GUI.skin.GetStyle("title"));

		GUILayout.Space(horizontalSpacer);

		AlignVertical(delegate {
			GUILayout.Space(10);
			var arrowRect = GetRect(arrowIconWidth, arrowIconHeight);
			GUI.DrawTexture(arrowRect, arrowTexture);
		}, arrowIconWidth);

		GUILayout.EndHorizontal();

		var lastRect = GUILayoutUtility.GetLastRect();

		// Make offer clickable.
		if (GUI.Button(lastRect, GUIContent.none, GUIStyle.none) && canPressButton) {
			selectedOffer = payment;
			scrollPosition = Vector2.zero;
		}
	}

	void DisplayPricePoints (DirectOffer payment)
	{
		GUILayout.BeginHorizontal();
			// Display back button.
			AlignHorizontal(delegate {
				if (GUILayout.Button("Back")) {
					selectedOffer = null;
					scrollPosition = Vector2.zero;
				}
			}, TextAlignment.Left);

			GUILayout.FlexibleSpace();

			// Display payment provider image
			if (payment.image != null) {
				var height = GetScaledImageHeight(payment.image, scaledImageWidth);
				var imageRect = GetRect(scaledImageWidth, height);
				GUI.DrawTexture(imageRect, payment.image);
			}

			// Display payment provider name
			GUILayout.Label(payment.name, GUI.skin.GetStyle("centeredLabel"));

			GUILayout.FlexibleSpace();

			// Display buy button
			AlignHorizontal(delegate {
				if (GUILayout.Button("Buy")) {
					selectedOffer.url += "&amount=" + selectedPrice.amount;
					urlToOpen = selectedOffer.url;
				}
			}, TextAlignment.Right);
		GUILayout.EndHorizontal();

		if (selectedOffer == null) {
			return;
		}

		GUILayout.Space(horizontalSpacer);

		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		var prices = payment.pricePoints;

		if (toggleIndex == null || payment.pricePoints.Length != toggleIndex.Length) {
			toggleIndex = new bool[prices.Length];
			toggleIndex[0] = true;
		}

		// Display pricing options
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			GUILayout.BeginVertical();
				for (int i = 0; i < prices.Length; i++) {
					GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
						toggleIndex[i] = GUILayout.Toggle(toggleIndex[i], prices[i].description);

						if (toggleIndex[i]) {
							selectedPrice = prices[i];
						}

					GUILayout.EndHorizontal();
				}
			GUILayout.EndVertical();

			GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.EndScrollView();
	}


	/// <summary>
	/// Draws a horizontal dividing line.
	/// </summary>
	void DisplayDivider ()
	{
		GUILayout.Space(verticalMargin);
		GUILayout.Label(GUIContent.none, GUI.skin.GetStyle("divider"));
		GUILayout.Space(verticalMargin);
	}

	/// <summary>
	/// Displays a confirmation dialog for opening the browser.
	/// </summary>
	void DisplayOpenBrowserConfirmation ()
	{
		AlignVertical(delegate {
			GUILayout.Label("This offer will open in your web browser.", GUI.skin.GetStyle("centeredLabel"));

			GUILayout.Space(verticalMargin);

			AlignHorizontal(delegate {
				if (GUILayout.Button("Cancel")) {
					urlToOpen = null;
				}

				if (GUILayout.Button("Open in Browser", GUI.skin.GetStyle("buttonPrimary"))) {
					Application.OpenURL(urlToOpen);
					urlToOpen = null;
				}
			});
		});
	}

	#region Utility Functions

	/// <summary>
	/// Sets up the sizes for the UI.
	/// </summary>
	void SetUISizes ()
	{
		matrix = Matrix4x4.Scale(Vector3.one * scale);

		scaledWidth = Screen.width / scale;
		scaledHeight = Screen.height / scale;

		fullScreenRect = new Rect(0, 0, scaledWidth, scaledHeight);
		windowRect = new Rect(padding, padding, scaledWidth - (padding * 2),  scaledHeight - (padding * 2));
	}

	/// <summary>
	/// Get the scaled height of a given image based.
	/// </summary>
	/// <param name="image">Image.</param>
	/// <param name="scaledImageWidth">Scaled width of the image.</param>
	/// <returns>The scaled height of the image.</returns>
	float GetScaledImageHeight (Texture2D image, float scaledImageWidth)
	{
		if (image == null) {
			return minimumImageHeight;
		}

		var ratio = image.width / image.height;
		var scaledImageHeight = scaledImageWidth / ratio;
		return scaledImageHeight;
	}

	/// <summary>
	/// Returns the width or the height, whichever is larger.
	/// </summary>
	/// <returns>The largest dimension.</returns>
	float GetLargestDimension (bool inverse=false)
	{
		float smaller, larger;

		if (Screen.width > Screen.height) {
			smaller = scaledHeight;
			larger = scaledWidth;
		} else {
			smaller = scaledWidth;
			larger = scaledHeight;
		}

		var largest = inverse ? smaller : larger;
		return largest;
	}

	/// <summary>
	/// Facilitates scrolling on mobile touchscreen devices.
	/// </summary>
	void MobileScroll ()
	{
		if (Input.touches == null || Input.touches.Length == 0) {
			return;
		}

		var touch = Input.GetTouch(0);

		switch (touch.phase) {
			case TouchPhase.Began:
				timeTouchPhaseStarted = Time.time;
				break;
			case TouchPhase.Moved:
				// Scroll via touch.
				scrollPosition.y += touch.deltaPosition.y;
				break;
			case TouchPhase.Ended:
				// Continue scrolling for a bit after touch ends.
				// Impart momentum, using last delta as the starting velocity.
				// Ignore delta < 10; rounding issue can cause ultra-high velocity.
				if (Mathf.Abs(touch.deltaPosition.y) >= 10) {
					scrollVelocity = (int)(touch.deltaPosition.y / touch.deltaTime);
					timeTouchPhaseEnded = Time.time;
				}

				break;
		}

		if (Input.touchCount != 1 && scrollVelocity != 0) {
			// Slow down over time.
			float t = (Time.time - timeTouchPhaseEnded) / inertiaDuration;
			float frameVelocity = Mathf.Lerp(scrollVelocity, 0, t);
			scrollPosition.y += frameVelocity * Time.deltaTime;

			// After N seconds, we've stopped.
			if (t >= inertiaDuration) {
				scrollVelocity = 0;
			}
		}
	}

	/// <summary>
	/// Aligns a GUI element horizontally.
	/// </summary>
	/// <param name="gui">Callback that draws GUI elements.</param>
	/// <param name="alignment">Which side to align the element on.</param>
	void AlignHorizontal (Action gui, TextAlignment alignment = TextAlignment.Center)
	{
		GUILayout.BeginHorizontal();

			if (alignment != TextAlignment.Left)  { GUILayout.FlexibleSpace(); }
			gui();
			if (alignment != TextAlignment.Right) { GUILayout.FlexibleSpace(); }

		GUILayout.EndHorizontal();
	}

	/// <summary>
	/// Centers a GUI element horizontally.
	/// </summary>
	/// <param name="gui">Callback that draws GUI elements.</param>
	/// <param name="width">Width of the surrounding area.</param>
	void AlignVertical (Action gui, float? width = null)
	{
		if (width.HasValue) {
			GUILayout.BeginVertical(GUILayout.MinWidth(width.Value), GUILayout.ExpandHeight(true));
		} else {
			GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
		}

		GUILayout.FlexibleSpace();
		gui();
		GUILayout.FlexibleSpace();

		GUILayout.EndVertical();
	}

	void PageContent(SuperRewardsOffer[] offers, Action gui)
	{
		if (offers == null) {
			return;
		}

		// Reduces calculations
		if (!calculatedPage) {
			DoPagingCalculations(offers);
		}

		if (startingIndex > offers.Length) {
			currentPage--;
			PageContent(offers, gui);
			return;
		}

		// Display paged content
		gui();

		GUILayout.BeginHorizontal();

		if (currentPage <= 1) {
			GUI.enabled = false;
		}

		if (GUILayout.Button("Prev")) {
			currentPage--;
			calculatedPage = false;
		}

		GUI.enabled = true;

		if (canPageNext) {
			GUI.enabled = false;
		}

		if (GUILayout.Button("Next")) {
			currentPage++;
			calculatedPage = false;
		}

		GUI.enabled = true;

		GUILayout.EndHorizontal();
	}

	void DoPagingCalculations (SuperRewardsOffer[] offers)
	{
		startingIndex = pageLimit * (currentPage - 1);
		pageSize = pageLimit;
		
		if (pageSize * currentPage > offers.Length) {
			pageSize = offers.Length - ((currentPage - 1) * pageLimit);
		}

		page = new SuperRewardsOffer[pageSize];
		Array.Copy(offers, startingIndex, page, 0, pageSize);

		canPageNext = currentPage * pageLimit >= offers.Length ? true : false;
		calculatedPage = true;
	}

	/// <summary>
	/// Wrapper for GUILayoutUtility.GetRect.
	/// The GetRect function that accepts a width and height returns a width different than the one supplied, which this remedies.
	Rect GetRect (float width, float height)
	{
		var rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(width), GUILayout.Height(height));
		return rect;
	}

	/// <summary>
	/// Convenience function to load a resource with generics type checking.
	/// </summary>
	/// <param name="name">Name of the asset.</param>
	/// <returns>The asset.</returns>
	static T LoadResource<T> (string name) where T : UnityEngine.Object
	{
		var resource = Resources.Load("superrewards_" + name, typeof(T)) as T;
		return resource;
	}

	#endregion
}
