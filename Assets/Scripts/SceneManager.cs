using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//yield return new WaitForSeconds(2.0);

		Application.LoadLevel("Game");
	}

//	IEnumerator Wait(float duration)
//	{
//		yield return new WaitForSeconds(duration);   //Wait
//	}
// 
}
