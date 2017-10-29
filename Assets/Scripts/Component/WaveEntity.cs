using UnityEngine;
using System.Collections;

public class WaveEntity : MonoBehaviour {
	Vector3 moveS = Vector3.zero;
	// Use this for initialization
	void Start () {
		moveS = new Vector3 (0, 1, 0).normalized * Screen.height / 50;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.Translate(moveS );

		if (transform.position.y > Screen.height) {
			gameObject.SetActive (false);

			transform.position = new Vector3 (Screen.width/2,0,0);
		}

	}



}
