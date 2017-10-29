using UnityEngine;
using System.Collections;

public class MissionFish : MonoBehaviour {

	float tarRot = 0;
	MissionFishData fishData;
	Rigidbody2D mainRig;
	Vector3 landPos;
	// Use this for initialization
	void Start () {

	}
	//	int checkT = 10;
	void FixedUpdate () {
		if (fishData != null) {
			if (IsOutRange ()) {
				getNewForce ();

			}

			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tarRot), 10f * Time.deltaTime);
		}
	}


	public void init(MissionFishData data,Vector3 _landPos){
		fishData = data;
		FishItemSpec spec = SpecController.getItemById (data.item_id) as FishItemSpec;
		transform.Find ("Fish").GetComponent<Animator> ().Play (spec.name);
		landPos = _landPos;
		mainRig = GetComponent<Rigidbody2D> ();
		getNewForce ();

	}

	bool IsOutRange(){
		Vector2 WorldRange = GameController.WorldBound;
		if (Mathf.Abs(transform.position.x - landPos.x) > WorldRange.x/2*1.2f ||Mathf.Abs(transform.position.y-landPos.y) > WorldRange.y/2*1.2f ) {
			return true;
		} else {
			return false;
		}
	}



	void getNewForce(){
		Vector3 vec = (landPos + new Vector3 (Random.Range (-2f, 2f), Random.Range (-2f,2f), 0) - transform.position).normalized;
		if (vec.x == 0 && vec.y == 0) {
			vec = new Vector3 (1,1,0).normalized;
		}

		int speed =Random.Range( 1000,5000);

		addForce(new Vector2 (vec.x * speed, vec.y * speed));
		tarRot = Mathf.Atan2 (vec.y, vec.x) * Mathf.Rad2Deg - 180f;

	}

	void addForce(Vector2 vec){
		mainRig.Sleep ();
		mainRig.AddForce (vec);

	}


	public void onClick(){
		DialogController.GetInstance ().showMissionFishPanel (fishData);
	}
}

