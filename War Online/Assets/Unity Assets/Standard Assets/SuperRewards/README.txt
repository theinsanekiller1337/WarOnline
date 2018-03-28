SuperRewards by Playerize
Version: 1
Supports: Unity 3.5 and 4.x
=====

Thank you for using SuperRewards. This package enables your game to provide
alternative methods for you to earn money, and for your players to obtain
your virtual currency.


Getting Started
---------------

1. Register an account with www.superrewards.com
2. Complete the "Add an Application" process for this game
3. Use the API calls documented below.

Try the demo located in "Standard Assets/SuperRewards/Demo/"


Using SuperRewards
---------------

-- Initialize --

You must initialize SuperRewards before you can display the offer wall
or check if the player is owed virtual currency from completing an offer.

To initialize SuperRewards, call the following.

    SuperRewards.Initialize(
    	<app code>,
    	<user ID>,
    	<on user points changed callback>,
    	<preload offers>
    );

The app code can be found in your SuperRewards dashboard.
The user ID is a unique value representing the current player (specified by you).
The callback is used when the player is owed virtual currency (specified by you).
The preload offers option loads offers during initialization (optional, false by default).


-- Show Offers --

After initializing SuperRewards, you can show the SuperRewards offer wall at any time.

To show the SuperRewards offer wall, call the following.

	SuperRewards.ShowOfferWall();


-- Check User Rewards --

After initializing SuperRewards, you can check a user's reward balance at any time
to see if they are owed virtual currency. This is automatically done during initialization,
but is optionally up to you to implement any additional checks during their play session.

If a user is owed virtual currency, the callback you supplied during initialization will
be called and you should write code to handle it there. Once this check returns as owing
money to the player, it assumes you handled it and will no longer result in owing the player
virtual currency until they have completed another offer.

To manually check if a player is owed money, call the following.

	SuperRewards.GetUserRewards();


-- Custom Virtual Currency Icon --

Use a custom icon for virtual currency by setting the following variable to your
own Texture2D.

	SuperRewards.currencyIcon = <your image>


Contact Us
----------

If you run into a problem or have a suggestion, let us know by emailing
info@superrewards.com or by visiting our support page at http://support.playerize.com
