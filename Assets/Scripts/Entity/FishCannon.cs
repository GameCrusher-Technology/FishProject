using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class FishCannon : MonoBehaviour
{
	float tarRot = 0;
	float distance ;
	PetData _petData ;

	Vector2 targetPos;
	Vector2 moveS;

	public void init(PetData data){
		_petData = data;
		gameObject.name = "FishCannon";
		transform.Find ("Fish").GetComponent<Animator> ().Play (data.fishName);
		transform.Find ("Fish").transform.localScale = new Vector3 (data.getCannonScale(),data.getCannonScale(),1);
		setDefault ();
	}

	// Use this for initialization
	void Start ()
	{
	
	}

	void setDefault(){

		Vector3 v = (new Vector2(0,1)).normalized;

		float t = Mathf.Atan2 ( v.y,  v.x) * Mathf.Rad2Deg - 180f;

		transform.rotation = Quaternion.Slerp (Quaternion.identity, Quaternion.Euler (0, 0, t), 5f);
	}
	// Update is called once per frame


	void FixedUpdate () {
		if (Ismoving ()) {
			float curDistance = Vector2.Distance (targetPos, transform.position);
			if (curDistance > distance) {
				arriveToTarget ();
			} else {
				distance = curDistance;
			}

			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tarRot), 3f * Time.deltaTime);
		}
	}

	public void fire(Vector3 tarPos){
		if(!Ismoving()){
			Vector3 curPos = new Vector3( transform.position.x,transform.position.y,0);
			Vector3 v = (tarPos - curPos).normalized;

			if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
				LoginController.GetInstance ().creatBullet (curPos,v,_petData);
			}
			else if(GameController.GetInstance().CurrentScene == GameController.BATTLESCENE){
				BattleController.GetInstance ().creatBullet (curPos,v,_petData);
			}

			float tar = Mathf.Atan2 (v.y, v.x)  * Mathf.Rad2Deg - 180;
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tar), 3f );
		}

	}

	public void BeAttack(int hurt){
		_petData.curHeart -= hurt;
		if(!isActive()){
			GetComponent<Image> ().color = Color.gray;
			transform.Find("Fish").GetComponent<SpriteRenderer> ().color = Color.gray;
		}
	}

	public void move(){
		//PetData petData = PlayerData.getPet (int.Parse (gameObject.name));
		targetPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector2	curDirection = (targetPos - (Vector2)transform.position).normalized;

		GetComponent<Rigidbody2D> ().Sleep ();
		GetComponent<Rigidbody2D> ().AddForce (new Vector2 (curDirection.x * _petData.getCurSpeed(), curDirection.y *  _petData.getCurSpeed()));
		tarRot = Mathf.Atan2 (curDirection.y, curDirection.x) * Mathf.Rad2Deg - 180f;
		distance = Vector2.Distance (targetPos, transform.position);
		moveS = (targetPos - (Vector2)transform.position).normalized;

		_petData.posx = targetPos.x;
		_petData.posy = targetPos.y;

		PlayerData.updatePetData (_petData);
	}
	public int getDataId(){
		return _petData.data_id;
	}

	public bool isActive(){
		return _petData.curHeart > 0;
	}

	void arriveToTarget(){
		targetPos = Vector2.zero;
		GetComponent<Rigidbody2D> ().Sleep ();
		setDefault ();
	}
	bool Ismoving(){
		return targetPos != Vector2.zero;
	}
	public void Click(){
		
		DialogController.GetInstance ().showPetToggle ( _petData.data_id,Camera.main.WorldToScreenPoint(transform.position));
	}
}

