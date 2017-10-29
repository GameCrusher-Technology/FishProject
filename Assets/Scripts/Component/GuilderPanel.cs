using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuilderPanel : MonoBehaviour {
	public Image Left;
	public Image Right;
	public Image Top;
	public Image Bottom;
	public Image Arrow;
	public Image Center;
	public GameObject MessagePart;

	public const string Step01 = "step01";
	public const string Step02 = "step02";
	public const string Step03 = "step03";
	public const string Step04 = "step04";
	public const string Step05 = "step05";

	Vector3 position = new Vector3(0,0,0);
	string curStep = "step01";
	// Use this for initialization
	void Start () {
		
	}

	public void initStep(string step ,Vector3 vecPos){
		curStep = step;
		position = vecPos;
		configPos ();
	}

	void configPos(){
		//Vector3 position =new Vector3(230,380,0);
		string message  = LanController.getString ("guild"+curStep);

		float s = 1280f / Screen.height;

		Center.transform.position = new Vector2(position.x,position.y) ;

	//	Center.rectTransform.sizeDelta = new Vector2 (Center.rectTransform.sizeDelta.x*s,Center.rectTransform.sizeDelta.y* s);

		Vector2 centerBund = new Vector2 (Center.rectTransform.sizeDelta.x,Center.rectTransform.sizeDelta.y);

		Top.rectTransform.sizeDelta = new Vector2 (Screen.width*s,Screen.height* s  -  (centerBund.y/2)  - position.y*s  );
		Top.transform.position = new Vector3(Screen.width/2,position.y + centerBund.y/2/s,0);

		Bottom.rectTransform.sizeDelta = new Vector2 (Screen.width*s,position.y*s -( centerBund.y/2) );
		Bottom.transform.position = new Vector3(Screen.width/2,position.y - centerBund.y/2/s,0);

		Left.rectTransform.sizeDelta = new Vector2 (position.x*s - ( centerBund.x/2) , centerBund.y);
		Left.transform.position = new Vector3(position.x -( centerBund.x/2/s) ,position.y  );


		Right.rectTransform.sizeDelta = new Vector2 (Screen.width*s -( centerBund.x/2) - position.x*s  , centerBund.y);
		Right.transform.position = new Vector3(position.x  +( centerBund.x/2/s),position.y ,0);

		if (position.x > Screen.width / 2) {
			Arrow.transform.position = new Vector2 (position.x - centerBund.x/2/s, position.y);
			Arrow.transform.localScale  = new Vector3(-1,1,1);
		} else {
			Arrow.transform.position = new Vector2 (position.x + centerBund.x/2/s, position.y);
		}

		if (message == null) {
			MessagePart.SetActive (false);
		} else {
			MessagePart.GetComponent<RectTransform> ().position = new Vector3(Screen.width/2,Screen.height*4/5,0);
			MessagePart.transform.Find ("Text").GetComponent<Text> ().text = message;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void beClickHandle(){
		gameObject.SetActive (false);

		switch (curStep) {
		case GuilderPanel.Step01:
			GameController.GetInstance ().addPet (GameController.initPetId);
			LoginController.GetInstance ().removeBox ();
			DialogController.GetInstance ().showFishInfoPanel ();
			DialogController.GetInstance ().showMessagePanel (LanController.getString("remindTofight"));
			break;
		case GuilderPanel.Step02:
			DialogController.GetInstance ().showTaskPanel (PlayerData.getCurTask());
			break;
	
		case GuilderPanel.Step03:
			DialogController.GetInstance ().showFishEggsInfoPanel ();
			break;
		
		case GuilderPanel.Step04:
			LoginController.GetInstance ().onClickFight ();
			break;
		
	
		}
	}
}
