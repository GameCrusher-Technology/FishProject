using UnityEngine;
using System.Collections;

public class HeartTip : MonoBehaviour {

	// Use this for initialization
	Vector3 targetPos = Vector3.zero;
	Vector2 moveS = Vector2.zero;
	void Start () {

	}

	public void init(Vector3 pos){
		targetPos = pos;
		StartCoroutine (moveHeart());
	}
	
	// Update is called once per frame

	float distance = float.NaN;
	void FixedUpdate () {
		if(!float.IsNaN(distance)){
			if(moveS !=  Vector2.zero){
				transform.Translate( moveS);
			}
			float curDis = Vector2.Distance (targetPos, transform.position);
			if (curDis > distance) {
				Destroy (gameObject);
			} else {
				distance = curDis;
			}
		}

	}

	IEnumerator moveHeart(){
		yield return new WaitForSeconds (0.5f);
		if (targetPos == Vector3.zero) {
			Destroy (gameObject);
		} else {
			transform.Find ("Number").gameObject.SetActive (false);
			float dis =  Vector2.Distance (targetPos, transform.position);
			moveS = (targetPos -transform.position ).normalized * (dis/20);

			distance = Vector2.Distance (targetPos, transform.position);
		}
	
	}




}
