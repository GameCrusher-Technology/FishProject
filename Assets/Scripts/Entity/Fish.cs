using UnityEngine;
using System.Collections;

public class Fish : MonoBehaviour {

	float tarRot = 0;
	FishData fishData;
	int speed;
	Rigidbody2D colliderRig;
	Rigidbody2D mainRig;

	string curMode;
	// Use this for initialization
	void Start () {
	
	}
//	int checkT = 10;
	void FixedUpdate () {
		if (fishData != null) {
			if (IsOutRange ()) {
				if(curMode == BattleFormation.MissionMode ){
					getNewForce ();
				}
				else if (fishData.repeatTime >= 1) {
					getNewForce ();
					fishData.repeatTime--;
				} else {
					deploy ();
				}

			}

			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tarRot), 10f * Time.deltaTime);
		}
	}


	public void init(FishData data,string mode){
		curMode = mode;
		fishData = data;
		speed = fishData.getCurSpeed();

		if(transform.Find("collider")){
			colliderRig = transform.Find ("collider").GetComponent<Rigidbody2D> ();
		}
		mainRig = GetComponent<Rigidbody2D> ();
		tarRot = Mathf.Atan2 (fishData.direction.y, fishData.direction.x) * Mathf.Rad2Deg - 180f;
		addForce (new Vector2 (fishData.direction.x * speed, fishData.direction.y * speed));
		transform.rotation =  Quaternion.Euler (0, 0, tarRot);

	}

	bool IsOutRange(){
		Vector2 WorldRange = GameController.WorldBound;
		if (Mathf.Abs(transform.position.x) > WorldRange.x*1.5f ||Mathf.Abs(transform.position.y) > WorldRange.y*1.5f ) {
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
		fishData.repeatTime = 0;
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

			tarRot = Mathf.Atan2 (fishData.direction.y, fishData.direction.x) * Mathf.Rad2Deg - 180f ;
	}

	void addForce(Vector2 vec){
		mainRig.Sleep ();
		mainRig.AddForce (vec);

		if(colliderRig){
			colliderRig.Sleep ();
			colliderRig.AddForce (vec);
		}

	}


	//void OnCollisionEnter2D(Collision2D collision){}

	void OnTriggerEnter2D(Collider2D col){
		Net net = col.gameObject.GetComponent <Net> ();
		if (net) {
			BeAttacked (net.attack,net.attackRadit,null);
		}
	}

	void BeAttacked(float attack, float cannonLevel,PetData pdata){

		float bulletA = attack*(10f+cannonLevel*2)/10f;
		float probability = 0.5f +(cannonLevel - fishData.getRadits ())*0.05f + GameController.GetInstance().ContributionValue;
		Debug.Log ("radits " + probability );
		if(probability >= Random.value ){
			fishData.curBlood -= attack;
			if(fishData.curBlood <=0 ){
				deadByHurt (pdata);
			}
		}
	}

	public void BeAttackedByPet(PetData data){
		BeAttacked (data.getCurAttack(), data.getCurAttackRadit(),data);
	}

	public void BeAttackedByWeak(float attack, float radit){
		BeAttacked (attack*2, radit+2,null);
	}

	public void beAttackBySkill(){
		//deadByHurt ();
	}



	void deadByHurt(PetData data){
		BattleController.GetInstance ().catchFish (fishData,transform.position,data);
		mainRig.Sleep ();
		deploy ();
	}
	void deploy(){
		BattleController.GetInstance ().removeFish (gameObject);
		Destroy (gameObject);
	}
}
