using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FishInfoTip : MonoBehaviour {

	// Use this for initialization
	Image icon;
	Text level;
	Text attack;
	Text aspeed;
	Text speed;

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void init(PetData data){
		icon = transform.Find ("Icon").GetComponent<Image> ();
		icon.sprite = GameController.GetInstance().getSpByName("Pic/ui/" + data.fishName+"Icon");

		transform.Find ("LText").GetComponent<Text> ().text = data.level.ToString();
		transform.Find ("DamageText").GetComponent<Text> ().text = data.getCurAttack().ToString();
		transform.Find ("AttackSText").GetComponent<Text> ().text = data.getAttackSpeed().ToString();
		transform.Find ("MoveSText").GetComponent<Text> ().text = data.getCurSpeed().ToString();

		transform.Find ("Id").GetComponent<Text> ().text = "ID: ";
		transform.Find ("IdText").GetComponent<Text> ().text = data.data_id.ToString();


	}
}
