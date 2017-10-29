using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopItemRender : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	ItemSpec spec;
	public void init(ItemSpec _spec){
		spec = _spec;
		transform.Find ("Text").GetComponent<Text> ().text = LanController.getString (spec.name).ToUpper();
		transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" + spec.name + "Icon");
		GameObject buyBut = transform.Find ("BuyButton").gameObject;
		GameObject useBut = transform.Find ("UseButton").gameObject;
		useBut.transform.Find ("Text").GetComponent<Text> ().text =  LanController.getString ("Use").ToUpper();

		int cost = 10000;
		if (spec.gem > 0) {
			cost = spec.gem;
			transform.Find("BuyButton").Find("Image").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/gemIcon");
		} else {
			cost = spec.coin;
		}
		useBut.SetActive (false);

		switch(spec.getItemType()){
		case SpecController.TreasureType:
			transform.Find ("StarImage").gameObject.SetActive (true);
			useBut.SetActive (true);
			buyBut.SetActive (false);

			TreasureData treasure = GameController.GetInstance ().treasureDatas [spec.item_id]; 
			if (treasure == null || !GameController.GetInstance().IsPaymentInitialized()) {
				useBut.transform.Find ("Text").GetComponent<Text> ().text = LanController.getString("buy").ToUpper();
			} else {
				useBut.transform.Find ("Text").GetComponent<Text> ().text = treasure.priceString;
			}
			break;
		case SpecController.BulletType:
			List<string> bullets = PlayerData.getBullets ();
			if (bullets.Contains (spec.item_id)) {
				buyBut.SetActive (false);
				useBut.SetActive (true);
				if(PlayerData.getCurrentBullet() == spec.item_id){
					useBut.transform.Find("Text").GetComponent<Text>().text = LanController.getString ("using").ToUpper();
				}
			} else {
				buyBut.transform.Find ("Text").GetComponent<Text> ().text = cost.ToString();
			}
			break;

		case SpecController.CannonType:
			List<string> cannons = PlayerData.getCannons ();
			if (cannons.Contains (spec.item_id)) {
				buyBut.SetActive (false);
				useBut.SetActive (true);
				if(PlayerData.getCurrentCannon() == spec.item_id){
					useBut.transform.Find("Text").GetComponent<Text>().text = LanController.getString ("using").ToUpper();
				}
			} else {
				buyBut.transform.Find ("Text").GetComponent<Text> ().text = cost.ToString();
			}
			break;
		default:
			buyBut.transform.Find ("Text").GetComponent<Text> ().text = cost.ToString();
			buyBut.SetActive (true);
			break;
		}
	}



}
