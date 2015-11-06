////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

public class SocialPlatfromMenu : EditorWindow {
	


	#if UNITY_EDITOR

	//--------------------------------------
	//  GENERAL
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Edit Settings")]
	public static void Edit() {
		Selection.activeObject = SocialPlatfromSettings.Instance;
	}

	//--------------------------------------
	// GETTING STARTED
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Getting Started/Plugin setup")]
	public static void SPGTPluginSetup() {
		string url = "https://unionassets.com/mobile-social-plugin/plugin-setup-161";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Getting Started/Updates")]
	public static void SPGTUpdates() {
		string url = "https://unionassets.com/mobile-social-plugin/updates-162";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Getting Started/Compatibility")]
	public static void SPGTCompatibility() {
		string url = "https://unionassets.com/mobile-social-plugin/compatibility-163";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Getting Started/Manifest Requirements")]
	public static void SPGTManifestRequirements() {
		string url = "https://unionassets.com/mobile-social-plugin/apis-and-requirements-191";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  TWITTER
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Twitter/Setup")]
	public static void SPTWSetup() {
		string url = "https://unionassets.com/mobile-social-plugin/setup-146";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Twitter/Coding Guidelines")]
	public static void SPTWCodingGuidelines() {
		string url = "https://unionassets.com/mobile-social-plugin/coding-guidelines-147";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Twitter/General Features")]
	public static void SPTWGeneralFeatures() {
		string url = "https://unionassets.com/mobile-social-plugin/general-features-148";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Twitter/Application-only API")]
	public static void SPTWApplicationOnlyAPI() {
		string url = "https://unionassets.com/mobile-social-plugin/application-only-api-149";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Twitter/Retrieving OAuth Tokens")]
	public static void SPTWRetrievingOAuthTokens() {
		string url = "https://unionassets.com/mobile-social-plugin/retrieving-oauth-tokens-427";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  FACEBOOK
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Facebook/SetUp")]
	public static void SPFBSetup() {
		string url = "https://unionassets.com/mobile-social-plugin/setup-133";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Facebook/Coding Guidelines")]
	public static void SPFBCodingGuidelines() {
		string url = "https://unionassets.com/mobile-social-plugin/coding-guidelines-134";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Facebook/General Features")]
	public static void SPFBGeneralFeatures() {
		string url = "https://unionassets.com/mobile-social-plugin/general-features-135";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Facebook/Likes API")]
	public static void SPFBLikesAPI() {
		string url = "https://unionassets.com/mobile-social-plugin/likes-api-136";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Facebook/Scores API")]
	public static void SPFBScoresAPI() {
		string url = "https://unionassets.com/mobile-social-plugin/scores-api-137";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Facebook/Game Gifting API")]
	public static void SPFBGameGiftingAPI() {
		string url = "https://unionassets.com/mobile-social-plugin/game-gifting-api-290";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Facebook/Turn-Based Games")]
	public static void SPFBTurnBasedGames() {
		string url = "https://unionassets.com/mobile-social-plugin/turn-based-games-291";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Facebook/Invite Friends API")]
	public static void SPFBInviteFriendsAPI() {
		string url = "https://unionassets.com/mobile-social-plugin/invite-friends-api-367";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Facebook/Analytics")]
	public static void SPFBAnalytics() {
		string url = "https://unionassets.com/mobile-social-plugin/analytics-138";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  Additional Social API
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Additional Social API/Native Sharing")]
	public static void SPASNativeSharing() {
		string url = "https://unionassets.com/mobile-social-plugin/native-sharing-188";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/Additional Social API/Instagram")]
	public static void SPASInstagram() {
		string url = "https://unionassets.com/mobile-social-plugin/instagram-189";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  More
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/More/Released Apps with the plugin")]
	public static void SPMRReleasedAppsWithThePlugin() {
		string url = "https://unionassets.com/mobile-social-plugin/released-apps-with-the-plugin-186";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/More/Platform Behavior differences")]
	public static void SPMRPlatformBehaviorDifferences() {
		string url = "https://unionassets.com/mobile-social-plugin/platform-behavior-differences-153";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/More/PlayMaker Actions")]
	public static void SPMRPlayMakerActions() {
		string url = "https://unionassets.com/mobile-social-plugin/playmaker-actions-187";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/More/Troubleshooting")]
	public static void SPMRTroubleshooting() {
		string url = "https://unionassets.com/mobile-social-plugin/troubleshooting-190";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/More/Using Plugins with Java Script")]
	public static void SPMRUsingPluginsWithJavaScript() {
		string url = "https://unionassets.com/android-native-plugin/using-plugins-with-java-script-201";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Mobile Social Plugin/Documentation/More/Migrating to Unity5")]
	public static void SPMRMigratingToUnity5() {
		string url = "https://unionassets.com/mobile-social-plugin/migrating-to-unity5-364";
		Application.OpenURL(url);
	}

	#endif

}
