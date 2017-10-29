using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Island : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}


	// Update is called once per frame
	void Update () {
		
	}

	//void OnCollisionEnter2D(Collision2D collision){
	//	if (collision.gameObject.GetComponent ("WorldShip")) {
	//		collision.gameObject.GetComponent<WorldShip> ().getCollider (gameObject.GetComponent<RectTransform>().anchoredPosition,gameObject.name);
	//	}
	//}

	void OnTriggerEnter2D(Collider2D collision){
		if (collision.gameObject.GetComponent ("WorldShip")) {
			collision.gameObject.GetComponent<WorldShip> ().getCollider (gameObject.GetComponent<RectTransform>().anchoredPosition,gameObject.name);
		}
	}

}
