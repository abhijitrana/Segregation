using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GoogleMobileAds.Api;
 
 
public class GameManger : MonoBehaviour {

	public string bannerId;
	public string interstialId;
 
	public GameObject blocks2x2;
	public GameObject blocks3x3;
	public GameObject blocks4x4;
	public GameObject blocks5x5;

	public GameObject bgCube;

	public Text scoreText;
	public Text timeText;
	public Text highScoreText;

	public Button retryBtn;
	public Button startBtn;
	public AudioSource bgMusic;
	public AudioSource sfxRightClick;
	public AudioSource sfxWrongClick;

	public Text bonusText;
	public Text penaltyText;

	private float timeLeft;
    
	private GameObject currentBlocks;

	private int score;
	private string correct_block_name;
	private bool isGameOver;
	private bool isGameStarted;

	private bool rotateClockwise;

	InterstitialAd interstitial;

	// Use this for initialization
	void Start () {

//		#if UNITY_EDITOR
//		Debug.Log("no ads in unity edtor mode");
//		#elif UNITY_ANDROID
//		Admob.Instance ().initAdmob (bannerId, interstialId);
//		Admob.Instance ().showBannerRelative (AdSize.Banner, AdPosition.BOTTOM_CENTER, 2);
//		Admob.Instance ().loadInterstitial ();
		//Admob.Instance ().setTesting (true);
//		#endif


//		string ismusicOn=PlayerPrefs.GetString("audio");
//		if (ismusicOn == "") {
//			PlayerPrefs.SetInt("audio", "on");
//		}
		RequestInterstitialAds();

		isGameStarted=false;
		setUp ();

	}

	void setUp()
	{
		retryBtn.gameObject.SetActive (false);
		timeText.color = Color.white;

		bonusText.enabled = false;
		penaltyText.enabled = false;
		changeBgColor ();

		blocks2x2.SetActive (false);
		blocks3x3.SetActive (false);
		blocks4x4.SetActive (false);
		blocks5x5.SetActive (false);

		int highScore=PlayerPrefs.GetInt("HighScore");
		highScoreText.text = "HIGH SCORE " + highScore;
	}
	void initGame()
	{
		bgMusic.volume = 0.5f;
		isGameOver = false;
		timeLeft = 60.0f;
		retryBtn.gameObject.SetActive (false);
		timeText.color = Color.white;

		bonusText.enabled = false;
		penaltyText.enabled = false;
		changeBgColor ();

		blocks2x2.SetActive (false);
		blocks3x3.SetActive (false);
		blocks4x4.SetActive (false);
		blocks5x5.SetActive (false);


		//initialize game score
		score = 0;
		updateScore ();
		assignColorToBlocks ();

		int highScore=PlayerPrefs.GetInt("HighScore");
		highScoreText.text = "HIGH SCORE " + highScore;
		rotateClockwise = true;
	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) { 
			Application.Quit();
		}

		Quaternion cubeRotation= bgCube.transform.rotation;   
		Vector3 dir;
		if (rotateClockwise == false) {
			dir = Vector3.forward;
		} else {
			dir = Vector3.back;
		}

		bgCube.transform.rotation = cubeRotation * Quaternion.AngleAxis((100.0f+(score/10.0f))*Time.deltaTime, dir);


		Debug.Log ("isGameStarted" + isGameStarted + "isGameOver" + isGameOver);

		if (isGameStarted==true && isGameOver==false) {
			Quaternion originalRot = currentBlocks.transform.rotation;   
			Vector3 rotateDir;
			if (rotateClockwise == true) {
				rotateDir = Vector3.forward;
			} else {
				rotateDir = Vector3.back;
			}

			currentBlocks.transform.rotation = originalRot * Quaternion.AngleAxis((100.0f+(score/10.0f))*Time.deltaTime, rotateDir);


			if (timeLeft <= 0 ) {
				//game over
				GameOver("Time's Up!");
				timeText.color = new Color(255f/255f, 30f/255f, 30f/255f);
				int highScore=PlayerPrefs.GetInt("HighScore");

				if (highScore <= score) {
					PlayerPrefs.SetInt("HighScore", score);
					highScore=PlayerPrefs.GetInt("HighScore");
					highScoreText.text = "HIGH SCORE " + highScore;
				}

				updateScore ();

			} else {


				//continue game play
				if (isGameOver == false) {

					timeLeft -= Time.deltaTime;
					timeText.text = "" + System.Math.Round (timeLeft, 1)+" s";
					if (timeLeft < 10) {
						timeText.color = new Color(255f/255f, 54f/255f, 54f/255f);
					} else {
						timeText.color = Color.white;
					}


					if (Input.GetMouseButtonUp (0)) {
						RaycastHit hit=new RaycastHit();
						Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
						if(Physics.Raycast(ray,out hit,100)){
							//check weather tap is correct or not

							Debug.Log ("------->"+hit.collider.name);

							if (hit.collider.name == correct_block_name) {
								//increment score for right tap
								sfxRightClick.Play();
								score++;
								Debug.Log ("score: "+score);

								//							int random = Random.Range (0, 2);
								//							if (random == 0) {
								rotateClockwise = !rotateClockwise; 
								//							}

								//every 10 right tap 5s wii be given bonus
								if (score != 0 && score % 20 == 0) {
									bonusText.enabled = true;
									bonusText.text="+5 Secs Added";
									timeLeft += 5.0f;
									StartCoroutine(hideBonus());
									//								yield WaitForSeconds (1f);
									//								bonusText.enabled = false;

								}

								timeText.text = "" + System.Math.Round (timeLeft, 1)+" s";

								changeBgColor ();
								assignColorToBlocks ();

								//	modify highscore if it greater than current score

								 
							int highScore=PlayerPrefs.GetInt("HighScore");
							Debug.Log ("highScore: "+highScore);

							if (highScore <= score) {
								PlayerPrefs.SetInt("HighScore", score);
								highScore=PlayerPrefs.GetInt("HighScore");
								highScoreText.text = "HIGH SCORE " + highScore;
							}
 

								updateScore ();

							} else if(hit.collider.name!="BG CUBE"){
								//reset score
								hit.collider.GetComponent<Renderer> ().material.color = Color.red;
								GameOver("Wrong Tap!\nGame Over.");
								/*
								penaltyText.enabled=true;
								StartCoroutine(hidePenalty());
								penaltyText.text="-"+(int)score/2+"  Points";
								score=(int)score/2;

								//							yield WaitForSeconds (1f);
								//							penaltyText.enabled = false;

								if (score < 0)
									score = 0;

								updateScore ();
								assignColorToBlocks ();
								//							GameOverWrongTap();
								sfxWrongClick.Play();
								Debug.Log ("wrong tap");
                                */
							}
						}
					}
				}
			}
		}
	}

	void assignColorToBlocks()
	{
		float r = Random.Range (0.5f, 1.0f);
		float g = Random.Range (0.5f, 1.0f);
		float b = Random.Range (0.5f, 1.0f);

//		float r = Random.Range (0.25f, 1.0f);
//		float g = Random.Range (0.25f, 1.0f);
//		float b = Random.Range (0.25f, 1.0f);

		int rem = (int)score / 10;

		blocks2x2.SetActive (false);
		blocks3x3.SetActive (false);
		blocks4x4.SetActive (false);
		blocks5x5.SetActive (false);


		switch (rem) {
		case 0:
			currentBlocks = blocks2x2;
			blocks2x2.SetActive (true);
			break;
		case 1:
			currentBlocks = blocks3x3;
			blocks3x3.SetActive (true);
			break;
		case 2:
			currentBlocks = blocks4x4;
			blocks4x4.SetActive (true);
			break;
		case 3:
			currentBlocks = blocks5x5;
			blocks5x5.SetActive (true);
			break;
		default: //after score 40 block collection will displarandom collection
			switch (Random.Range (0, 3)) {
			case 0:
				currentBlocks = blocks2x2;
				blocks2x2.SetActive (true);
				break;
			case 1:
				currentBlocks = blocks3x3;
				blocks3x3.SetActive (true);
				break;
			case 2:
				currentBlocks = blocks4x4;
				blocks4x4.SetActive (true);
				break;
			case 3:
				currentBlocks = blocks5x5;
				blocks5x5.SetActive (true);
				break;
			}
			break;

		}


		int child_count = currentBlocks.transform.childCount;

		//random block which will be colored differently
		int random_block_num = Random.Range (0, child_count);


		for (int i = 0; i < child_count; i++){
			if (random_block_num !=i) {
				Color rndm_color = new Color (r, g, b, 1f);
				currentBlocks.transform.GetChild(i).GetComponent<Renderer> ().material.color = rndm_color;

			} else {
				float color_diff = -0.25f;
				Color rndm_color = new Color (r+color_diff, g+color_diff, b+color_diff, 1f);
				currentBlocks.transform.GetChild(i).GetComponent<Renderer> ().material.color = rndm_color;
				correct_block_name = currentBlocks.transform.GetChild (i).name;
			}

	     }

	}

	public void onClickRetry()
	{
		initGame ();
	}

	public void onClickMusic(){
		string ismusicOn=PlayerPrefs.GetString("audio");

//		if(ismu
//		if (highScore <= score) {
//			PlayerPrefs.SetInt("HighScore", score);
//			highScore=PlayerPrefs.GetInt("HighScore");
//			highScoreText.text = "HIGH SCORE " + highScore;
//		}
	}
		
	void changeBgColor()
	{
		//create random dark color
		float r= Random.Range(0.0f,0.5f);
		float g= Random.Range(0.0f,0.5f);
		float b= Random.Range(0.0f,0.5f);

 		Color random_dark_color=new Color(r,g,b,1f);
 	    
    	//set color to material
 		gameObject.GetComponent<Renderer>().material.color=random_dark_color;
		//gameObject.GetComponent<Renderer>().material.

	}
	void showInterstial()
	{
//		if(Admob.Instance().isInterstitialReady()){
//			Admob.Instance ().showInterstitial ();
//		}
	}
	void updateScore()
	{
		scoreText.text = "" + score;
	}
	void GameOver(string reason)
	{
		interstitial.Show();

		bgMusic.volume = 0.1f;
		timeText.text = reason;
		retryBtn.gameObject.SetActive (true);
		showInterstial ();
		isGameOver = true;
	}

	IEnumerator hidePenalty()
	{

		yield return new WaitForSeconds(2);
		penaltyText.enabled = false;
		//Do Function here...
	}

	IEnumerator hideBonus()
	{

		yield return new WaitForSeconds(2);
		bonusText.enabled = false;
		//Do Function here...
	}



	public void onBack(){
		Application.LoadLevel("MenuScene");
	}

	public void onStartButtonClicked()
	{
		startBtn.gameObject.SetActive (false);
		isGameStarted = true;
		initGame ();
	}

    public void ShowAd()
	{
//			if (Advertisement.IsReady())
//			{
//				Advertisement.Show();
//			}
	}
 
	private void RequestInterstitialAds()
	{
		string adID = "ca-app-pub-7490631007477530/9508602408";

		#if UNITY_ANDROID
		string adUnitId = adID;
		#elif UNITY_IOS
		string adUnitId = adID;
		#else
		string adUnitId = adID;
		#endif

		// Initialize an InterstitialAd.
		interstitial = new InterstitialAd(adUnitId);

		//***Test***
//		AdRequest request = new AdRequest.Builder()
//		.AddTestDevice(AdRequest.TestDeviceSimulator)       // Simulator.
//		.AddTestDevice("2077ef9a63d2b398840261c8221a0c9b")  // My test device.
//		.Build();

		//***Production***
		AdRequest request = new AdRequest.Builder().Build();

		//Register Ad Close Event
		interstitial.OnAdClosed += Interstitial_OnAdClosed;

		// Load the interstitial with the request.
		interstitial.LoadAd(request);

		Debug.Log("AD LOADED XXX");

		}

		//Ad Close Event
		private void Interstitial_OnAdClosed(object sender, System.EventArgs e)
		{
		//Resume Play Sound

		}

}
