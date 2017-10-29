using UnityEngine;
using System.Collections;

public class WorldFish : MonoBehaviour
{

	float tarRot = 0;
	WorldFishData fishData;
	int speed;
	Rigidbody2D mainRig;
	// Use this for initialization
	void Start () {

	}
	//	int checkT = 10;
	void FixedUpdate () {
		if (fishData != null) {

			//transform.Translate (new Vector3(fishData.direction.x/10,fishData.direction.y/10,0));
		//	transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tarRot), 10f * Time.deltaTime);
		}
	}


	public void init(WorldFishData data){
		fishData = data;
		speed = fishData.getCurSpeed();

		mainRig = GetComponent<Rigidbody2D> ();
		tarRot = Mathf.Atan2 (fishData.direction.y, fishData.direction.x) * Mathf.Rad2Deg - 180f;
		//transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tarRot), 3f*Time.deltaTime);
	//	addForce (new Vector2 (fishData.direction.x * speed, fishData.direction.y * speed));

	}

	bool IsOutRange(){
		Vector2 WorldRange = GameController.WorldBound;
		if (transform.position.x > WorldRange.x*2 || transform.position.x < -WorldRange.x*2 || transform.position.y > WorldRange.y*2 || transform.position.y <- WorldRange.y*2) {
			return true;
		} else {
			return false;
		}
	}

	public void outing(){
		Vector3 vec;
		if (transform.position.x > 0) {
			vec = new Vector3 (1,0,0).normalized;
		} else {
			vec = new Vector3 (-1,0,0).normalized;
		}
		fishData.direction = vec;
		addForce(new Vector2 (fishData.direction.x * 100, fishData.direction.y * 100));
		tarRot = Mathf.Atan2 (fishData.direction.y, fishData.direction.x) * Mathf.Rad2Deg - 180f;
	}

	void getNewForce(){
		Vector3 vec = (new Vector3 (Random.Range (-8f, 8f), Random.Range (-8f, 8f), 0) - transform.position).normalized;
		if (vec.x == 0 && vec.y == 0) {
			vec = new Vector3 (1,1,0).normalized;
		}
		fishData.direction = vec;
		addForce(new Vector2 (fishData.direction.x * speed, fishData.direction.y * speed));
		tarRot = Mathf.Atan2 (fishData.direction.y, fishData.direction.x) * Mathf.Rad2Deg - 180f;

	}

	void addForce(Vector2 vec){
		mainRig.Sleep ();
		mainRig.AddForce (vec);
	}


	//void OnCollisionEnter2D(Collision2D collision){}

	void OnTriggerEnter2D(Collider2D col){
		Debug.Log ("triggle");
	}

	void OnMouseDown(){
		//DialogController.GetInstance ().showWorldFishPanel (fishData);
		DialogController.GetInstance ().showMessagePanel (LanController.getString("comingsoon"));

	}


	void deadByHurt(bool ispet){
		mainRig.Sleep ();
		deploy ();
	}
	void deploy(){
		BattleController.GetInstance ().removeFish (gameObject);
		Destroy (gameObject);
	}
}

