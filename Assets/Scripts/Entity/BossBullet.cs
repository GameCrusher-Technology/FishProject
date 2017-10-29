using UnityEngine;
using System.Collections;

public class BossBullet : MonoBehaviour {
	Animator bulletAnimator;
	// Use this for initialization
	void Start () {
	}
	BulletItemSpec bulletSpec;
	float attack =0 ;
	float attackradit = 0;
	public WorldFishData petData;
	int explodeTimer = 80 ;

	// Update is called once per frame
	void FixedUpdate () {
		if (bulletAnimator != null) {
			if (explodeTimer > 0) {
				explodeTimer--;
			} else if (Mathf.Abs(transform.position.x) > GameController.WorldBound.x ||Mathf.Abs( transform.position.y)>  GameController.WorldBound.y ) {
			//	explode();
				explodeTimer = 80;
			}

		}
	}




	public void initBossBullet(Vector3 rot,WorldFishData pData){
		petData = pData;
		bulletAnimator = transform.GetComponent<Animator>();
		bulletAnimator.Play (pData.getBulletName());


		float tar = Mathf.Atan2 (rot.y, rot.x) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tar), 3f );


		float speed = pData.getBulletSpeed()*2;
		GetComponent<Rigidbody2D> ().AddForce (new Vector2(rot.x* speed*2,rot.y* speed*2));
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.name == "FishCannon") {
			col.gameObject.GetComponent<FishCannon> ().BeAttack (50);
			explode ();
		}
		else if (col.gameObject.name == "MyCannon") {
			col.gameObject.GetComponent<MyCannon> ().BeAttack (50);
			explode ();
		}
	}

	public void explode(){
		BattleController.GetInstance ().creatBulletEffect (transform.position);
		Destroy (gameObject);
	}
}
