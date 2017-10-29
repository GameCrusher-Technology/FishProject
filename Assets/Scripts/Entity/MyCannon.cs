using UnityEngine;
using System.Collections;

public class MyCannon : MonoBehaviour {

	Animator animator;
	CannonItemSpec itemSpec;
	// Use this for initialization
	void Start () {
	}
	// Update is called once per frame
	void Update () {
	
	}
	public void init(CannonItemSpec spec){
		itemSpec = spec;
		getAnimator().Play (itemSpec.name);
	}

	public void shot(float tar){
		getAnimator().Rebind ();
		getAnimator().Play (itemSpec.name);

		getCannon().rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tar), 3f );

	}

	private Animator getAnimator(){
		if (!animator) {
			animator = getCannon().GetComponent<Animator> ();
		}
		return animator;
	}
	Transform _cannnonTrans;
	Transform getCannon(){
		if(!_cannnonTrans){
			_cannnonTrans = transform.Find ("Cannon");
		}
		return _cannnonTrans;
	}
	public void clickHigh(){
		if(GameController.GetInstance().CurrentScene == GameController.BATTLESCENE){
			BattleController.GetInstance ().onClickCannonAdd ();
		}else if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
			LoginController.GetInstance ().onClickCannonAdd ();
		}
	}

	public void clickLow(){
		if(GameController.GetInstance().CurrentScene == GameController.BATTLESCENE){
			BattleController.GetInstance ().onClickCannonSuntract ();
		}else if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
			LoginController.GetInstance ().onClickCannonSuntract ();
		}
	}

	public void BeAttack(int hurt){
	
	}

}
