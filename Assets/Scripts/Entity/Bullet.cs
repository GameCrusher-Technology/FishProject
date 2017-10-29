using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	Animator bulletAnimator;
	// Use this for initialization
	void Start () {
	}
	BulletItemSpec bulletSpec;
	float attack =0 ;
	float attackradit = 0;
	public PetData petData;
	int explodeTimer = 80 ;

	// Update is called once per frame
	void FixedUpdate () {
		if (bulletAnimator != null) {
			if (explodeTimer > 0) {
				explodeTimer--;
			} else if (Mathf.Abs(transform.position.x) > GameController.WorldBound.x ||Mathf.Abs( transform.position.y)>  GameController.WorldBound.y ) {
				explode();
				explodeTimer = 80;
			}

		}
	}


	public void initBullet( Vector3 rot , BulletItemSpec spec , float _speed , float _attack ,float _cannonLevel){
		bulletSpec = spec;
		bulletAnimator = transform.GetComponent<Animator>();
		bulletAnimator.Play (spec.name);
		attack = _attack;
		attackradit = _cannonLevel;
		float tar = Mathf.Atan2 (rot.y, rot.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tar), 3f );
		GetComponent<Rigidbody2D> ().AddForce (new Vector2(rot.x* _speed*2,rot.y* _speed*2));
	}


	public void initPetBullet(Vector3 rot,PetData pData){
		petData = pData;
		bulletAnimator = transform.GetComponent<Animator>();
		bulletAnimator.Play (pData.getBulletName());


		float tar = Mathf.Atan2 (rot.y, rot.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tar), 3f );


		float speed = pData.getBulletSpeed()*2;
		GetComponent<Rigidbody2D> ().AddForce (new Vector2(rot.x* speed*2,rot.y* speed*2));
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.GetComponent ("Fish")) {
			if (petData != null) {
				col.gameObject.GetComponent <Fish> ().BeAttackedByPet (petData);
				explode (false);
			} else {
				explode (false);
			}
		}
	}




	public void explode(bool isDestroy = true){
		if (bulletSpec != null) {
			if(GameController.GetInstance().CurrentScene == GameController.BATTLESCENE){
				BattleController.GetInstance ().creatNet (bulletSpec.net, attack, attackradit,transform.position);
			}


			if (isDestroy) {
				Destroy (gameObject);
			} else if (bulletSpec.item_id != "30006") {
				Destroy (gameObject);
			}
		} else {
			if (GameController.GetInstance ().CurrentScene == GameController.BATTLESCENE) {
				BattleController.GetInstance ().creatBulletEffect (transform.position);
			}
			Destroy (gameObject);
		}


	}
}
