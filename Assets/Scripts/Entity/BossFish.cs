using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class BossFish : MonoBehaviour {

	float tarRot = 0;
	WorldFishData bossData;
	int speed;
	Rigidbody2D colliderRig;
	Rigidbody2D mainRig;

	// Use this for initialization
	void Start () {
		BattleController.GetInstance ().creatFishHole (new Vector3(0,0,0),"10015",50);
	}
	int attackCount = 50;
	void FixedUpdate () {
		if (bossData != null) {
		//	transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tarRot), 10f * Time.deltaTime);

			if (attackCount <= 0) {
				attackCount = bossData.getAttackSpeed ();
				attack ();
			} else {
				attackCount--;
			}
		}


	}


	public void init(WorldFishData data){
		bossData = data;
		speed = bossData.getCurSpeed();

		if(transform.Find("collider")){
			colliderRig = transform.Find ("collider").GetComponent<Rigidbody2D> ();
		}

		mainRig = GetComponent<Rigidbody2D> ();
		tarRot = Mathf.Atan2 (bossData.direction.y, bossData.direction.x) * Mathf.Rad2Deg - 180f;
		//transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tarRot), 3f*Time.deltaTime);

		//addForce (new Vector2 (bossData.direction.x * speed, bossData.direction.y * speed));

	}

	bool IsOutRange(){
		Vector2 WorldRange = GameController.WorldBound;
		if (Mathf.Abs(transform.position.x) > WorldRange.x*1.5f ||Mathf.Abs(transform.position.y) > WorldRange.y*1.5f ) {
			return true;
		} else {
			return false;
		}
	}



	void attack(){
		GameObject curAttackTarget = BattleController.GetInstance ().getCurBossAttackTarget ();
		Vector3 curDirection = (curAttackTarget.transform.position - transform.position).normalized;
		BattleController.GetInstance ().creatBossBullet (transform.position,curDirection,bossData);	
	}

	void getNewForce(){
		Vector3 vec = (new Vector3 (Random.Range (-8f, 8f), Random.Range (-8f, 8f), 0) - transform.position).normalized;
		if (vec.x == 0 && vec.y == 0) {
			vec = new Vector3 (1,1,0).normalized;
		}
		bossData.direction = vec;
		addForce(new Vector2 (bossData.direction.x * speed, bossData.direction.y * speed));
		tarRot = Mathf.Atan2 (bossData.direction.y, bossData.direction.x) * Mathf.Rad2Deg - 180f;
	}

	void addForce(Vector2 vec){
		mainRig.Sleep ();
		mainRig.AddForce (vec);

		if(colliderRig){
			colliderRig.Sleep ();
			colliderRig.AddForce (vec);
		}

	}


	void OnTriggerEnter2D(Collider2D col){
		Debug.Log (col.gameObject.name + " " + System.DateTime.Now.Second);
		switch(col.gameObject.name){
		case "PetBullet":
			BeAttackedByPet (col.gameObject.GetComponent<Bullet> ().petData);
			col.gameObject.GetComponent<Bullet> ().explode (false);
			break;
		case "Bullet":
			col.gameObject.GetComponent<Bullet> ().explode (false);
			break;
		case "Net":
			Net net = col.gameObject.GetComponent <Net> ();
			BeAttacked (net.attack,net.attackRadit,null);
			break;
		}
	}



	void BeAttacked(float attack, float cannonLevel,PetData pdata){

		float bulletA = attack;
		float probability = 0.5f +(cannonLevel - bossData.getRadits ())*0.1f + GameController.GetInstance().ContributionValue;
		Debug.Log ("radits " + probability );
		if(probability >= Random.value ){
			bossData.curBlood -= attack;
			if(bossData.curBlood <=0 ){
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

	public void Click(){
		BattleController.GetInstance ().fire ();
	}



	void deadByHurt(PetData data){
		//BattleController.GetInstance ().catchFish (fishData,transform.position,data);
		mainRig.Sleep ();
		deploy ();
	}
	void deploy(){
		BattleController.GetInstance ().removeFish (gameObject);
		Destroy (gameObject);
	}

}

