using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FishInfoPanel : MonoBehaviour {

	// Use this for initialization
	GameObject fishBut;
	GameObject LeftBut;
	GameObject RightBut;

	GameObject petRender;
	GameObject fishRender;
	GameObject petBuyRender;
	GameObject fishBuyRender;

	GameObject petGrid;
	GameObject fishGrid;
	Transform infoPart;
	GameObject itemInfoPart;
	GameObject playerInfoPart;
	Text extendFishPartText;

	int curIndex = 0;
	const string ExtendCannon = "ExtendCannon";
	const string ExtendPet = "ExtendPet";
	const string PetBase = "PetBase";
	int leftCannonBaseCount ;

	List<PetData> list = new List<PetData> ();
	void Start () {
		Text title = transform.Find ("Skin").Find ("Title").GetComponent<Text> ();
		title.text = LanController.getString ("operationsroom").ToUpper();
	}


	public void initFishData(int dataId = -1){
		list = PlayerData.getPets ();


		infoPart = transform.Find ("InfoPart");
		Transform skin = transform.Find ("Skin");
		itemInfoPart = transform.Find ("BuyInfoPart").gameObject;
		fishBut = infoPart.Find ("FishButton").gameObject;
		LeftBut = infoPart.Find ("LButton").gameObject;
		RightBut = infoPart.Find ("RButton").gameObject;
		petGrid = skin.Find ("PetList").Find ("Grid").gameObject;
		fishGrid = skin.Find ("FishList").Find ("Grid").gameObject;
		petRender = skin.Find ("PetRender").gameObject;
		fishRender = skin.Find ("FishRender").gameObject;
		petBuyRender = skin.Find ("PetBuyRender").gameObject;
		fishBuyRender = skin.Find ("FishBuyRender").gameObject;
		playerInfoPart = skin.Find ("PlayerPart").gameObject;
		infoPart.Find ("Release").Find("Text").GetComponent<Text> ().text = LanController.getString ("release").ToUpper();
		infoPart.Find ("LevelUp").Find("Text").GetComponent<Text> ().text = LanController.getString ("levelup").ToUpper();
		extendFishPartText = skin.Find ("FishList").Find ("ExtendButton").Find("Text").GetComponent<Text>();
		refreshPetsList ();

		if(dataId != -1){
			foreach(PetData d in list){
				if(d.data_id == dataId){
					setInfoPetData (d);
					break;
				}
			}
		}
	}

	List<GameObject> renderList = new List<GameObject>();

	void refreshPetsList(){
		foreach(GameObject obj in renderList){
			Destroy (obj);
		}
		GameObject bt;
		int petCount = 0;
		int fishCount = 0;
		int maxCannonCount = PlayerData.getPetCannonMaxCount ();
		int maxFishCount = Mathf.Max(GameController.minPetCount, PlayerData.getPetMaxCount ());

		foreach(PetData petData in list){
			if (petData.currentState != GameController.Pet_Fight) {
					fishCount++;
					bt = (GameObject)Instantiate(fishRender,Vector3.zero,Quaternion.identity,fishGrid.transform);
					bt.transform.Find ("Button").Find ("Text").GetComponent<Text> ().text = LanController.getString ("battle").ToUpper();
					bt.transform.Find ("Icon").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/" + petData.fishName + "Icon");
					bt.transform.Find ("expPart").Find ("Text").GetComponent<Text> ().text = petData.level.ToString();
					bt.name = petData.data_id.ToString ();
					bt.SetActive (true);
					renderList.Add (bt);

			} else if(maxCannonCount>0){
				petCount++;
				bt = (GameObject)Instantiate(petRender,Vector3.zero,Quaternion.identity,petGrid.transform);
				bt.transform.Find ("Button").Find ("Text").GetComponent<Text> ().text = LanController.getString ("rest").ToUpper();
				bt.transform.Find ("Icon").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/" + petData.fishName + "Icon");
				bt.transform.Find ("expPart").Find ("Text").GetComponent<Text> ().text = petData.level.ToString();
				bt.name = petData.data_id.ToString ();
				bt.SetActive (true);
				renderList.Add (bt);
				maxCannonCount--;
			}
		}

		extendFishPartText.text = list.Count + "/" + maxFishCount;
		int leftPetBaseCount = maxFishCount - list.Count;
		leftCannonBaseCount = maxCannonCount;
		while(maxCannonCount >0){
			
			petCount++;
			bt = (GameObject)Instantiate(petBuyRender,Vector3.zero,Quaternion.identity,petGrid.transform);
			bt.transform.Find ("Button").gameObject.SetActive(false);
			bt.name = (-1).ToString();
			bt.SetActive (true);
			renderList.Add (bt);

			maxCannonCount--;
		}

		while(leftPetBaseCount >0){

			fishCount++;
			bt = (GameObject)Instantiate(fishBuyRender,Vector3.zero,Quaternion.identity,fishGrid.transform);
			bt.transform.Find ("Button").gameObject.SetActive(false);
			bt.name = PetBase;
			bt.SetActive (true);
			renderList.Add (bt);

			leftPetBaseCount--;
		}


		//extendpart
		if(petCount  < GameController.maxPetCannonCount){
			petCount++;
			bt = (GameObject)Instantiate(petBuyRender,Vector3.zero,Quaternion.identity,petGrid.transform);
			bt.transform.Find ("Button").Find ("Text").GetComponent<Text> ().text = LanController.getString ("buy").ToUpper();
			bt.name = ExtendCannon;
			bt.SetActive (true);
			renderList.Add (bt);
		}
	
		if(fishCount  < GameController.maxPetCount){
			fishCount++;
			bt = (GameObject)Instantiate(fishBuyRender,Vector3.zero,Quaternion.identity,fishGrid.transform);
			bt.transform.Find ("Button").Find ("Text").GetComponent<Text> ().text = LanController.getString ("buy").ToUpper();
			bt.name = ExtendPet;
			bt.SetActive (true);
			renderList.Add (bt);
		}




		float wgrid = Mathf.Max (500, petCount * 130 + (petCount - 1) * 10 + 20);
		petGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(wgrid, 180);
		petGrid.transform.localPosition = new Vector2 (wgrid/2,0);

		int t = (fishCount % 3) == 0 ? (fishCount / 3) : ((fishCount/ 3) + 1);
		float h = t * 150 + (t - 1) * 10 + 20;
		fishGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(fishGrid.GetComponent<RectTransform>().sizeDelta.x, h);
		fishGrid.transform.Translate (new Vector3(0,-h/2,0));

		playerInfoPart.transform.Find("ExpText").GetComponent<Text>().text = PlayerData.getFishExp().ToString();
		playerInfoPart.transform.Find("GemText").GetComponent<Text>().text = PlayerData.getgem().ToString();
		playerInfoPart.transform.Find("CoinText").GetComponent<Text>().text = PlayerData.getcoin().ToString();
	}



	PetData curPetData;
	void setInfoPetData(PetData data){

		infoPart.gameObject.SetActive (true);

		if (data.currentState == GameController.Pet_Default) {
			infoPart.Find ("ConfirmButton").Find ("Text").GetComponent<Text> ().text = LanController.getString ("toFight");
		}else {
			infoPart.Find ("ConfirmButton").Find ("Text").GetComponent<Text> ().text = LanController.getString ("toRest");
		}

		curIndex = list.IndexOf (data);	
		if (list.Count <= 1) {
			LeftBut.SetActive (false);
			RightBut.SetActive (false);
		} else {
			LeftBut.SetActive (true);
			RightBut.SetActive (true);
		}


		curPetData = data;
		fishBut.GetComponent<FishInfoTip> ().init (data);
	}

	string curBuyItemName;
	void showItemBuyPart(string itemName){
		curBuyItemName = itemName;
		if (itemName == ExtendCannon) {
			itemInfoPart.SetActive (true);
			itemInfoPart.transform.Find ("ItemImage").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/" + "LockIcon"); 
			int cost = getExtendCannonCost ();
			GameObject buyButImg = itemInfoPart.transform.Find ("BuyButton").gameObject;
			buyButImg.transform.Find ("Image").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/gemIcon");
			itemInfoPart.transform.Find ("message").GetComponent<Text> ().text = LanController.getString ("addOneCannon");
			buyButImg.transform.Find ("Text").GetComponent<Text> ().text = cost.ToString ();

			itemInfoPart.transform.Find ("NameText").gameObject.GetComponent<Text> ().text = LanController.getString ("petCannon").ToUpper ();
		} else {
			itemInfoPart.SetActive (true);
			itemInfoPart.transform.Find ("ItemImage").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/" + "TipIcon04"); 
			int cost = getExtendFishCost ();
			GameObject buyButImg = itemInfoPart.transform.Find ("BuyButton").gameObject;
			buyButImg.transform.Find ("Image").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/CoinIcon");
			itemInfoPart.transform.Find ("message").GetComponent<Text> ().text = LanController.getString ("addOneFish");
			buyButImg.transform.Find ("Text").GetComponent<Text> ().text = cost.ToString ();
			itemInfoPart.transform.Find ("NameText").gameObject.GetComponent<Text> ().text = LanController.getString ("petFish").ToUpper ();
		}
	}

	int getExtendCannonCost(){
		int[] costArr = {10,100,1000,5000,10000};
		int curCannonCount = PlayerData.getPetCannonMaxCount ();
		if (curCannonCount >= costArr.Length) {
			return 10000000;
		} else {
			return costArr [curCannonCount];
		}

	}

	int getExtendFishCost(){
		int curPetCount = PlayerData.getPetMaxCount () ;
		if (curPetCount  >=  GameController.maxPetCount ) {
			return 100000000;
		} else {
			return  (int)Mathf.Pow( (float)(curPetCount - GameController.minPetCount + 1f),2f)*1000;
		}

	}

	// Update is called once per frame
	void Update () {

	}

	public void onListItemSelected(GameObject itemRender){
		AudioController.PlaySound (AudioController.SOUND_button);
		if (itemRender.name == ExtendCannon) { 
			showItemBuyPart (ExtendCannon);
		} else if(itemRender.name == ExtendPet){
			showItemBuyPart (ExtendPet);
		}else if(itemRender.name == PetBase){
			DialogController.GetInstance ().showFishEggsInfoPanel ();
			onClickOut ();
		}
		else {
			int data_id = int.Parse (itemRender.name);
			foreach(PetData d in list){
				if(d.data_id == data_id){
					setInfoPetData (d);
					break;
				}
			}
		}

	}


	public void onExtendFishClick(){
		AudioController.PlaySound (AudioController.SOUND_button);
		showItemBuyPart (ExtendPet);
	}
	public void onBuyPartOut(){
		AudioController.PlaySound (AudioController.SOUND_button);
		itemInfoPart.SetActive (false);
	}
	public void onBuyConfirm(){
		AudioController.PlaySound (AudioController.SOUND_button);
		if (curBuyItemName == ExtendCannon) {
			int cost = PlayerData.getgem () - getExtendCannonCost ();
			if (cost >= 0) {
				PlayerData.setgem (cost);
				PlayerData.setPetCannonMaxCount (PlayerData.getPetCannonMaxCount () + 1);
				itemInfoPart.SetActive (false);
				refreshPetsList ();
				AudioController.PlaySound (AudioController.SOUND_Panel);
			} else {
				DialogController.GetInstance ().showMessagePanel (LanController.getString ("GemNotEnough"));
			}
		} else if(curBuyItemName == ExtendPet) {
			int cost = PlayerData.getcoin () - getExtendFishCost ();
			if (cost >= 0) {
				PlayerData.setcoin (cost);
				PlayerData.setPetMaxCount (PlayerData.getPetMaxCount () + 1);
				itemInfoPart.SetActive (false);
				refreshPetsList ();
				AudioController.PlaySound (AudioController.SOUND_Panel);
			} else {
				DialogController.GetInstance ().showMessagePanel (LanController.getString ("CoinNotEnough"));
			}
		}
	}
	public void onLevelUpClicked()
	{
		AudioController.PlaySound (AudioController.SOUND_button);
		DialogController.GetInstance ().showLevelUpPanel (curPetData);
		onClickOut ();
	}

	public void onInfoConfirm()
	{
		AudioController.PlaySound (AudioController.SOUND_button);

		foreach (PetData pData in list) {
			if (pData.data_id == curPetData.data_id) {
				if (pData.currentState == GameController.Pet_Default) {

					if (leftCannonBaseCount > 0) {
						pData.currentState = GameController.Pet_Fight;
					} else {
						DialogController.GetInstance ().showMessagePanel (LanController.getString("NotMoreBase"));
					}

				}else {
					pData.currentState = GameController.Pet_Default;
				}
				refreshPetsList ();
				break;
			}
		}

		infoPart.gameObject.SetActive (false);
	}

	public void onReleaseClicked()
	{
		AudioController.PlaySound (AudioController.SOUND_button);

		PetData data = curPetData;
		DialogController.GetInstance ().showMessagePanel (LanController.getString("Suretorelease"),true,delegate {
			if(data!=null){
				List<OwnedItem> l = new List<OwnedItem>();
				OwnedItem cItem = new OwnedItem();
				cItem.item_id = "coin";
				cItem.count = (int)((data.level+1) * 1000 * Random.Range(0.5f,1.5f));
				l.Add(cItem);

				OwnedItem eItem = new OwnedItem();
				eItem.item_id = "exp";
				eItem.count = (int)((data.level+1) * 100 * Random.Range(0.5f,1f));
				l.Add(eItem);

				DialogController.GetInstance().showRewardsPanel(l,LanController.getString("conRewards"));


				foreach(PetData d in list){
					if(data.data_id == d.data_id){
						list.Remove (d);
						break;
					}
				}

				onClickOut ();
			}
		});
	}


	public void onInfoOutClick(){
		AudioController.PlaySound (AudioController.SOUND_button);
		infoPart.gameObject.SetActive (false);
	}



	public void onPetButClick(GameObject petRender){
		AudioController.PlaySound (AudioController.SOUND_button);
		if (petRender.name == ExtendCannon) {
			showItemBuyPart (ExtendCannon);
		} else {
			int selectId =int.Parse( petRender.name);
			foreach(PetData pData in list){
				if(pData.data_id == selectId){
					if(pData.currentState == GameController.Pet_Fight){
						pData.currentState = GameController.Pet_Default;
					}
					refreshPetsList ();
					break;
				}
			}
		}

	}

	public void onFishButClick(GameObject fishRender){
		AudioController.PlaySound (AudioController.SOUND_button);
		if (fishRender.name == ExtendPet) {
			showItemBuyPart (ExtendPet);
		} else {
			if (leftCannonBaseCount > 0) {
				int selectId = int.Parse (fishRender.name);
				foreach (PetData pData in list) {
					if (pData.data_id == selectId) {
						if (pData.currentState == GameController.Pet_Default) {
							pData.currentState = GameController.Pet_Fight;
						}
						refreshPetsList ();
						break;
					}
				}
			} else {
				DialogController.GetInstance ().showMessagePanel (LanController.getString("NotMoreBase"));
			}

		}
	}


	public void onConfirmLeft()
	{
		AudioController.PlaySound (AudioController.SOUND_button);
		curIndex--;
		if(curIndex <0){
			curIndex = list.Count - 1;
		}
		setInfoPetData (list[curIndex]);
	}
	public void onConfirmRight()
	{
		AudioController.PlaySound (AudioController.SOUND_button);
		curIndex++;
		if(curIndex >= list.Count){
			curIndex = 0;
		}
		setInfoPetData (list[curIndex]);
	}
	public void onClickOut()
	{
		AudioController.PlaySound (AudioController.SOUND_button);
		gameObject.SetActive (false);
		PlayerData.refreshPets (list);
		if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
			LoginController.GetInstance ().refreshPets ();
			LoginController.GetInstance ().initTreasure ();
		}
		if(GameController.GetInstance().isNewer && GameController.GetInstance().curGuildStep == GuilderPanel.Step01){
			DialogController.GetInstance ().showGuiderPanel (GuilderPanel.Step02, LoginController.GetInstance ().UI.transform, LoginController.GetInstance ().UI.transform.Find ("TaskButton").transform.position);
		}

	}
		 
}
