using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RewardsPanel : MonoBehaviour {
	private List<OwnedItem> list;
	private string message;
	// Use this for initialization
	void Start () {
	
		GameObject listItemRender =  transform.Find ("ItemList").Find  ("ItemRender").gameObject;
		GameObject	listGrid = transform.Find ("ItemList").Find  ("Grid").gameObject;
		foreach (OwnedItem item in list) {
			
			GameObject bt = Instantiate(listItemRender);
			bt.GetComponent<RectTransform>().SetParent(listGrid.GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			bt.SetActive (true);

			bt.transform.Find ("CountText").GetComponent<Text> ().text = "×" +  item.count.ToString();
			if(item.item_id == "gem"){
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  "gemIcon");
				PlayerData.setgem (PlayerData.getgem() + item.count);
			}else if(item.item_id == "coin"){
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  "CoinIcon");
				PlayerData.setcoin (PlayerData.getcoin() + item.count);
			}
			else if(item.item_id == "exp"){
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  "ExpIcon");
				PlayerData.setFishExp (PlayerData.getFishExp() + item.count);
			}
			else if(item.item_id == "bullet"){
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  "BulletShopIcon");
				PlayerData.addBulletCount (item.count);
			}else
			{
				ItemSpec spec = SpecController.getItemById (item.item_id);
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" + spec.name + "Icon");
			}
		}

		transform.Find ("Button").Find ("Text").GetComponent<Text> ().text = LanController.getString ("confirm").ToUpper();
		transform.Find ("MessagePanel").Find ("Text").GetComponent<Text> ().text = message;

		int t = list.Count ;
		float h =Mathf.Max( t * 100 + (t - 1) * 10 + 20,400);
		listGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(h, 146);
		listGrid.transform.Translate (new Vector3(-h/2,0,0));


	}
	System.Action callBackAction;
	public void init(List<OwnedItem> _list,string _message,System.Action _callBackAction = null){
		list = _list;
		message = _message;
		callBackAction = _callBackAction;
	}
	// Update is called once per frame
	void Update () {
	
	}
	public void onClickConfirm(){
		if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
			LoginController.GetInstance ().initTreasure ();
		}
		Destroy (gameObject);

		AudioController.PlaySound (AudioController.SOUND_FULLSKILL);

		if(callBackAction != null){
			callBackAction ();
		}
	}
}
