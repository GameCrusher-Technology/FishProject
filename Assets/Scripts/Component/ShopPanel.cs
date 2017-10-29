using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class ShopPanel : MonoBehaviour {

	GameObject ItemInfo;
	GameObject listItemRender;
	GameObject itemList;

	public const string TreasureTab = "Treasure";
	public const string BulletItemTab = "BulletItem";
	public const string CannonTab = "Cannon";
	public const string BulletTab = "Bullet";

	Dictionary<string,ItemSpec> treasureDic;
	Dictionary<string,ItemSpec> bulletItemDic;
	Dictionary<string,ItemSpec> cannonDic;
	Dictionary<string,ItemSpec> bulletDic;
	string groupId =  "Treasure";
	string itemId;


	// Use this for initialization
	void Start () {
		
		itemList =  transform.Find ("ItemList").Find ("Grid").gameObject;
		ItemInfo = transform.Find ("ItemInfoPanel").gameObject;
		transform.Find ("TitleText").GetComponent<Text>().text = LanController.getString ("shop").ToUpper();
		listItemRender =  transform.Find ("ItemList").Find  ("ShopItemRender").gameObject;

		treasureDic = SpecController.getGroup (TreasureTab);
		bulletDic = SpecController.getGroup (BulletTab);
		bulletItemDic = SpecController.getGroup (BulletItemTab);
		cannonDic = SpecController.getGroup (CannonTab);

		showItemList ();
	}


	string[] idsArr;

	Vector3 lastListPos ;

	public void initPanel(string initTab){
		initCoinAndGem ();
		if(initTab != null){
			groupId = initTab;
		}
	}
	void initCoinAndGem(){
		transform.Find ("CoinText").GetComponent<Text> ().text = PlayerData.getcoin().ToString();
		transform.Find ("GemText").GetComponent<Text> ().text = PlayerData.getgem().ToString();
		if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
			LoginController.GetInstance ().initTreasure ();
		}
	}
	void showItemList(){
		ItemInfo.SetActive (false);
		Dictionary<string,ItemSpec> dic = new Dictionary<string, ItemSpec>();
		switch(groupId){
		case TreasureTab:
			dic = treasureDic;
			break;
		case BulletItemTab:
			dic = bulletItemDic;
			break;
		case BulletTab:
			dic = bulletDic;
			break;
		case CannonTab:
			dic = cannonDic;
			break;

		}
		idsArr = new string[dic.Count];
		dic.Keys.CopyTo (idsArr,0);
		for (int i = 0; i < itemList.transform.childCount; i++) {
			GameObject go = itemList.transform.GetChild (i).gameObject;
			Destroy (go);
		}
		creatItemList (itemList,dic);
	}

	void refreshItemList(){
		lastListPos = itemList.GetComponent<RectTransform> ().localPosition;
		showItemList ();
		itemList.GetComponent<RectTransform> ().localPosition = lastListPos;
	}

	void creatItemList(GameObject listGrid,Dictionary<string,ItemSpec> dic){
		GridLayoutGroup gridLayout= listGrid.GetComponent<GridLayoutGroup> ();
		foreach (string key in dic.Keys) {
			ItemSpec spec = dic [key];
			GameObject bt = Instantiate(listItemRender);
			bt.GetComponent<RectTransform>().SetParent(listGrid.GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			bt.name = spec.item_id;
			bt.GetComponent<ShopItemRender> ().init (spec);
			bt.SetActive (true);
			bt.GetComponent<Button>().onClick.AddListener(          //
				delegate()
				{
					this.onItemClick(bt);
				}
			);
			bt.transform.Find ("BuyButton").GetComponent<Button> ().onClick.AddListener (
				delegate() {
					this.onBuyButtonClicked (spec);
				}
			);
			bt.transform.Find ("UseButton").GetComponent<Button> ().onClick.AddListener (
				delegate() {
					this.onUseButtonClicked (spec);
				}
			);
		}
		int p = 2;
		int t = (dic.Count % p) == 0 ? (dic.Count / p) : ((dic.Count / p) + 1);
		float h = t * gridLayout.cellSize.y + (t - 1) * 10 + 20;
		listGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(630, h);
		listGrid.transform.Translate (new Vector3(0,-h/2,0));
	}

	ItemSpec curInfoSpec;
	void showItemInfo(string id){

		itemId = id;
		ItemSpec  spec = SpecController.getItemById (id);
		curInfoSpec = spec;
		ItemInfo.transform.Find("ItemImage").GetComponent<Image>().sprite =  GameController.GetInstance().getSpByName("Pic/ui/" + spec.name + "Icon"); 
		ItemInfo.transform.Find ("NameText").gameObject.GetComponent<Text> ().text = LanController.getString (spec.name).ToUpper();
		ItemInfo.SetActive (true);
		GameObject buyBut = ItemInfo.transform.Find ("BuyButton").gameObject;
		GameObject useBut = ItemInfo.transform.Find ("UseButton").gameObject;

		useBut.transform.Find("Text").GetComponent<Text>().text = LanController.getString ("use").ToUpper();


		GameObject treasurePartGrid =  ItemInfo.transform.Find ("TreasurePart").Find("Grid").gameObject;
		GameObject iconPart = ItemInfo.transform.Find ("TreasurePart").Find ("Icon").gameObject;
		iconPart.SetActive (false);
		ItemInfo.transform.Find ("message").GetComponent<Text> ().text = spec.getMessage();

		buyBut.SetActive (false);
		useBut.SetActive (false);

		int cost = 10000;
		if (spec.gem > 0) {
			cost = spec.gem;
			buyBut.transform.Find("Image").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/gemIcon");
		} else {
			cost = spec.coin;
			buyBut.transform.Find("Image").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/CoinIcon");
		}

		List<string[]> list = new List<string[]> ();
		if (spec.getItemType () == SpecController.TreasureType) {
			if (spec.gem > 0) {
				list.Add(new string[]{"gemIcon","×" + cost});
			} else {
				list.Add(new string[]{"CoinIcon","×" + cost});
			}
			useBut.SetActive (true);

			TreasureData treasure = GameController.GetInstance ().treasureDatas [spec.item_id]; 
			if (treasure == null || !GameController.GetInstance().IsPaymentInitialized()) {
				useBut.transform.Find ("Text").GetComponent<Text> ().text = LanController.getString("buy").ToUpper();
			} else {
				useBut.transform.Find ("Text").GetComponent<Text> ().text = treasure.priceString ;
			}

		} else {
			bool owned = false;
			if(spec.getItemType () == SpecController.BulletType){
				list.Add(new string[]{"AttackTipIcon",  (spec as BulletItemSpec).attack.ToString ()});
				list.Add(new string[]{"frozen",  (spec as BulletItemSpec).speed.ToString ()});
				List<string> bullets = PlayerData.getBullets ();
				if (bullets.Contains (spec.item_id)) {
					owned = true;
					if (PlayerData.getCurrentBullet () == spec.item_id) {
						useBut.transform.Find("Text").GetComponent<Text>().text = LanController.getString ("using").ToUpper();
					}
				}
			}else if(spec.getItemType () == SpecController.CannonType){
				list.Add(new string[]{"ExpIcon",(spec as CannonItemSpec).maxLevel.ToString ()});
				list.Add(new string[]{"AttackTipIcon","+" + (spec as CannonItemSpec).attack.ToString ()+"%"});
				list.Add(new string[]{"frozen","+" + (spec as CannonItemSpec).attackSpeed.ToString ()+"%"});

				List<string> cannons = PlayerData.getCannons ();
				if (cannons.Contains (spec.item_id)) {
					owned = true;
					if (PlayerData.getCurrentCannon () == spec.item_id) {
						useBut.transform.Find("Text").GetComponent<Text>().text = LanController.getString ("using").ToUpper();
					}
				}
			}else if(spec.getItemType () == SpecController.BulletItemType){
				list.Add(new string[]{"BulletShopIcon","×" + spec.count.ToString ()});
			}

			if (owned) {
				useBut.SetActive (true);
			} else {
				buyBut.transform.Find ("Text").GetComponent<Text> ().text = cost.ToString ();
				buyBut.SetActive (true);
			}
				
		}

		for (int i = 0; i < treasurePartGrid.transform.childCount; i++) {
			GameObject go = treasurePartGrid.transform.GetChild (i).gameObject;
			Destroy (go);
		}

	
		foreach (string[] l in list) {
			GameObject bt = (GameObject)Instantiate(iconPart,Vector3.zero,Quaternion.identity,treasurePartGrid.GetComponent<RectTransform>());
			bt.transform.Find("Image").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" + l[0]);
			bt.transform.Find("Text").GetComponent<Text>().text = l[1];
			bt.SetActive (true);
		
		}
		
	}



	// Update is called once per frame
	void Update () {
	
	}

	public void onInfoClickBuy(){
		ItemInfo.SetActive (false);
		onBuyButtonClicked (curInfoSpec);
	}

	public void onInfoUseClick(){
		ItemInfo.SetActive (false);
		onUseButtonClicked (curInfoSpec);
	}

	public void onBuyButtonClicked(ItemSpec spec){
		if (spec.getItemType () == SpecController.TreasureType) {
			
			GameController.GetInstance().BuyConsumable (spec.item_id);
		} else {
			if (spec.gem > 0) {
				if (PlayerData.getgem () < spec.gem) {
					//not enough
					DialogController.GetInstance().showMessagePanel(LanController.getString("GemNotEnough"));
					return;
				} else {
					PlayerData.setgem (PlayerData.getgem()-spec.gem);
				}
			} else {
				if (PlayerData.getcoin () < spec.coin) {
					//not enough
					DialogController.GetInstance().showMessagePanel(LanController.getString("CoinNotEnough"));
					return;
				} else {
					PlayerData.setcoin (PlayerData.getcoin()-spec.coin);
				}
			}

			initCoinAndGem ();
			if (spec.getItemType () == SpecController.BulletType) {
				PlayerData.addBullet (spec.item_id);
			} else if (spec.getItemType () == SpecController.CannonType) {
				PlayerData.addCannon (spec.item_id);
			} else if (spec.getItemType () == SpecController.BulletItemType) {
				PlayerData.addBulletCount (spec.count);
				if (GameController.GetInstance ().CurrentScene == GameController.BATTLESCENE) {
					BattleController.GetInstance ().refreshBulletCount ();
				}else if(GameController.GetInstance ().CurrentScene == GameController.LOGINSCENE){
					LoginController.GetInstance ().refreshBulletCount ();
				}
			}
		}
		initCoinAndGem ();
		refreshItemList ();
		AudioController.PlaySound (AudioController.SOUND_FULLSKILL);
	}

	public void onUseButtonClicked(ItemSpec spec){
		switch(spec.getItemType()){
		case SpecController.BulletType:
			PlayerData.setCurBullet (spec.item_id);
			if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
				LoginController.GetInstance ().initBullet ();
			}else if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
				BattleController.GetInstance ().initBullet ();
			}
			break;

		case SpecController.CannonType:
			PlayerData.setCurCannon (spec.item_id);
			if (GameController.GetInstance ().CurrentScene == GameController.LOGINSCENE) {
				LoginController.GetInstance ().initCannon ();
			} else if(GameController.GetInstance().CurrentScene == GameController.BATTLESCENE){
				BattleController.GetInstance ().initCannon ();
			}

			break;
		case  SpecController.TreasureType:
			GameController.GetInstance().BuyConsumable (spec.item_id);
			break;
		}
		refreshItemList ();
		onClickOut ();
		AudioController.PlaySound (AudioController.SOUND_button);

	}

	public void onItemClick(GameObject but){
		showItemInfo (but.name);
	}


	public void onClickTab(Button but){
		if(groupId != but.name){
			groupId = but.name;
			showItemList ();
		}
	}

	public void onClickOut(){
		gameObject.SetActive (false);
	}

	public void onClickRemoveItem(){
		ItemInfo.SetActive (false);
	}



	public void onClickScollLeft(){
		string nextId = string.Empty;
		for(int i =0;i<idsArr.Length;i++){
			if (idsArr[i] == itemId) {
				if (i == 0) {
					nextId = idsArr [idsArr.Length-1];
				} else {
					nextId = idsArr [i-1];
				}

				break;
			}
		}
		if(nextId != string.Empty){
			showItemInfo (nextId);
		}
	}
	public void onClickScollRight(){
		string nextId = string.Empty;
		for(int i =0;i<idsArr.Length;i++){
			if (i == idsArr.Length - 1) {
				nextId = idsArr [0];
				break;
			} else {
				if (idsArr[i] == itemId) {
					nextId = idsArr [i+1];
					break;
				}
			}
		}
		if(nextId != string.Empty){
			showItemInfo (nextId);
		}
	}
}
