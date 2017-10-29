using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Pet : MonoBehaviour {

	float tarRot = 0;
	private PetData petData;
	int speed;
	GameObject petskin;
	Vector2 WorldRange ;
	Animator petAni;
	GameObject effect ;
	// Use this for initialization
	void Start () {
		petAni = gameObject.GetComponent<Animator> ();
		effect = (GameObject)Instantiate(GameController.GetInstance ().getPrefab ("BulletEffect"),Vector3.zero,Quaternion.identity,transform);
		effect.SetActive (false);

	}

	int attackCount = 10000;
	int newForceTimer = 30;
	void FixedUpdate () {
		if (petData != null) {
			if (IsOutRange ()) {
				if(newForceTimer >0){
					newForceTimer--;
				}else{
					newForceTimer = 30;
					getNewForce ();
				}
			} else {
				newForceTimer = 0;
			}
			//transform.Translate (new Vector3(fishData.direction.x/10,fishData.direction.y/10,0));
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tarRot), 3f * Time.deltaTime);

			if (attackCount <= 0) {
				attackCount = petData.getCurASpeed ();
				petAttack ();
			} else {
				attackCount--;
			}
		}
	}

	public void init(PetData data){
		petData = data;
		speed = petData.getCurSpeed();
		WorldRange = GameController.WorldBound;
		getNewForce ();
		attackCount = petData.getCurASpeed();
		petskin = (GameObject)Instantiate(GameController.GetInstance ().getPrefab("PetSkin"),new Vector3(0,0,0),Quaternion.identity,transform);
	}

	void petAttack(){
		if(GameController.GetInstance().CurrentScene == GameController.BATTLESCENE){
			BattleController.GetInstance ().creatBullet (transform.position,curDirection,petData);	
		}
	}
	bool IsOutRange(){
		if (transform.position.x > WorldRange.x*1.2 || transform.position.x < -WorldRange.x*1.2 || transform.position.y > WorldRange.y*1.2 || transform.position.y <- WorldRange.y*1.2) {
			return true;
		} else {
			return false;
		}
	}
		
	Vector3 curDirection;
	void getNewForce(){
		curDirection = (new Vector3 (Random.Range (-4000, 4000)/1000, Random.Range (-4000, 4000)/1000, 0) - transform.position).normalized;
		if (curDirection.x == 0 && curDirection.y == 0) {
			curDirection = new Vector3 (1,1,0).normalized;
		}
		GetComponent<Rigidbody2D> ().Sleep ();
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (curDirection.x * speed, curDirection.y * speed));
		tarRot = Mathf.Atan2 (curDirection.y, curDirection.x) * Mathf.Rad2Deg - 180f;
	}

	public void getForce(Vector3 pos){
		curDirection = (transform.position - pos).normalized;
		if (curDirection.x == 0 && curDirection.y == 0) {
			curDirection = new Vector3 (1,1,0).normalized;
		}
		GetComponent<Rigidbody2D> ().Sleep ();
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (curDirection.x * speed, curDirection.y * speed));
		tarRot = Mathf.Atan2 (curDirection.y, curDirection.x) * Mathf.Rad2Deg - 180f;
	}

	public void MoveToWorldPos(Vector3 tarPos){
		curDirection = (tarPos - transform.position).normalized;
		if (curDirection.x == 0 && curDirection.y == 0) {
			curDirection = new Vector3 (1,1,0).normalized;
		}
		GetComponent<Rigidbody2D> ().Sleep ();
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (curDirection.x * speed, curDirection.y * speed));
		tarRot = Mathf.Atan2 (curDirection.y, curDirection.x) * Mathf.Rad2Deg - 180f;
	}

	void OnMouseDown(){
		if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
			if(!EventSystem.current.IsPointerOverGameObject ()){
				DialogController.GetInstance ().showFishInfoPanel ();
			}
		}
	}

	public void setSkillUse(){
		petskin.GetComponent<PetSkin> ().fullSkill ();
	}

	public void useSkill(){
		petAni.SetBool ("Skill",true);
		StartCoroutine (endSkill (10.0f));
		effect.SetActive (true);
		petskin.GetComponent<PetSkin> ().useSkill ();

		AudioController.PlaySound (AudioController.SOUND_USESKILL,10.0f);
	}

	IEnumerator endSkill(float time){
		yield return new WaitForSeconds (time);
		petAni.SetBool ("Skill",false);
		effect.SetActive (false);
	}

}
