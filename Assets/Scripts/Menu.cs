using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

 

public class Menu : MonoBehaviour {

	void awake(){
 
	}

	// Use this for initialization
	void Start () {
 
	}
	
	public void onPlay(){
		Application.LoadLevel("Game");
	}

	public void onRateNow(){
		Application.OpenURL ("http://play.google.com/store/apps/details?id=com.abhijit.colorblocks");
	}


	public void onLeaderboard(){
		Social.localUser.Authenticate (
			(bool success)=>{
				Social.ShowLeaderboardUI ();
			}
		);
	}

	public void onAchievements(){
		Social.localUser.Authenticate (
			(bool success)=>{
				Social.ShowAchievementsUI ();
			}
		);
	}

}
