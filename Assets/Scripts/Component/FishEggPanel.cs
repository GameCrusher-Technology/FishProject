using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FishEggPanel : MonoBehaviour {

	// Use this for initialization
	Image fishIcon;
	GameObject LeftBut;
	GameObject RightBut;
	GameObject ConfirmBut;
	Text timePartText;
	ItemSpec spec;
	public GameObject itemInfoPart;

	const int TYPE_BUY = 1 ;
	const int TYPE_USE = 0 ;
	const int TYPE_SPEED = 2 ;
	const int TYPE_OUT = 3 ;
	const int TYPE_Extend = 4 ;
	int itemType = 0;

	int curIndex = 0;
	int curEggsCount ;
	int maxEggsCount ;

	List<PetEggData> list ;
	PetEggData curPetData;

	void Start () {
		Text title = transform.Find ("SkinImage").Find ("Title").GetComponent<Text> ();
		title.text = LanController.getString ("babyroom").ToUpper();
	}

	public void initFishData(int _index = 0){
		list = PlayerData.getPetEggs ();

		curEggsCount = list.Count;
		maxEggsCount = PlayerData.getPetEggMaxCount ();
		if(curEggsCount < GameController.maxPetEggCount){
			PetEggData boxData = new PetEggData ();
			boxData.item_id = "20001";
			list.Add (boxData);
		}

		transform.Find ("SkinImage").Find ("ExtendButton").Find ("Text").GetComponent<Text> ().text = LanController.getString ("capacity").ToUpper () + " " + curEggsCount + "/" + maxEggsCount;

		PetEggData data = list [_index];
		curIndex = _index;

		itemInfoPart.SetActive (false);

		setPetData (data);

		if (list.Count <= 1) {
			LeftBut.SetActive (false);
			RightBut.SetActive (false);
		} else {
			LeftBut.SetActive (true);
			RightBut.SetActive (true);
		}
	}

	void setPetData(PetEggData data){
		
		fishIcon = transform.Find ("SkinImage").Find ("FishPart").Find ("Icon").GetComponent<Image>();
		LeftBut = transform.Find ("SkinImage").Find ("LButton").gameObject;
		RightBut = transform.Find ("SkinImage").Find ("RButton").gameObject;
		ConfirmBut = transform.Find ("SkinImage").Find ("ConfirmButton").gameObject;
		timePartText = transform.Find ("SkinImage").Find ("TimePart").Find ("Text").GetComponent<Text>();

		GameObject steppart =  transform.Find ("SkinImage").Find ("StepPart").gameObject;
		if (data.item_id == "20001") {
			steppart.SetActive (false);
		} else {
			steppart.SetActive (true);
			steppart.transform.Find ("text").GetComponent<Text> ().text = (curIndex + 1).ToString ();
		}
		spec = SpecController.getItemById (data.item_id) ;
		fishIcon.sprite = GameController.GetInstance().getSpByName("Pic/ui/" + spec.name + "Icon");


		curPetData = data;
		if (spec is FishItemSpec) {
			TimeSpan leftTime = data.getLeftTime ();
			if(leftTime.TotalSeconds <= 0){
				itemType = TYPE_OUT;
				ConfirmBut.transform.Find ("Text").GetComponent<Text> ().text = LanController.getString ("setout").ToUpper ();
				timePartText.text = LanController.getString ("PetOutMes").ToUpper ();
			}
			else {
				itemType = TYPE_SPEED;
				timePartText.text = leftTime.ToString();
				ConfirmBut.transform.Find ("Text").GetComponent<Text> ().text = LanController.getString ("speedup").ToUpper ();
			} 
		} else {

			OwnedItem item =  PlayerData.getOwnedItem (spec.item_id);
			if(item.count >0){
				itemType = TYPE_USE;

				timePartText.text = LanController.getString ("randomPetEgg");

				ConfirmBut.transform.Find ("Text").GetComponent<Text> ().text = LanController.getString("open").ToUpper();
			}else{
				itemType = TYPE_BUY;

				timePartText.text = LanController.getString ("buyEggCase");
				ConfirmBut.transform.Find ("Text").GetComponent<Text> ().text = LanController.getString("buy").ToUpper();
			}
		}

	}

	void FixedUpdate () {
		if(itemType == TYPE_SPEED){
			TimeSpan t = curPetData.getLeftTime ();
			if (t.TotalSeconds <= 0) {
				setPetData (curPetData);
			} else {
				timePartText.text = curPetData.getLeftTime ().ToString();
			}

		}
	}

	public void onConfirmClicked()
	{
		AudioController.PlaySound (AudioController.SOUND_button);
		if (itemType == TYPE_BUY) {
			itemInfoPart.SetActive (true);
			itemInfoPart.transform.Find ("ItemImage").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/" + spec.name + "Icon"); 
			int cost = 10000;
			GameObject buyButImg = itemInfoPart.transform.Find ("BuyButton").gameObject;
			if (spec.gem > 0) {
				cost = spec.gem;
				buyButImg.transform.Find ("Image").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/gemIcon");
			} else {
				cost = spec.coin;
				buyButImg.transform.Find ("Image").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/CoinIcon");
			}

			itemInfoPart.transform.Find ("message").GetComponent<Text> ().text = spec.getMessage ();
			buyButImg.transform.Find ("Text").GetComponent<Text> ().text = cost.ToString ();
			itemInfoPart.transform.Find ("NameText").gameObject.GetComponent<Text> ().text = LanController.getString (spec.name).ToUpper ();

		} else if (itemType == TYPE_USE) {
			if (curEggsCount < maxEggsCount) {
				PlayerData.addOwnedItem (spec.item_id, -1);
				PlayerData.addPetEgg (creatNewEgg ());
				initFishData (curIndex);
			} else {
				showExtendPet ();
				DialogController.GetInstance ().showMessagePanel (LanController.getString("PetEggFull"));
			}

		}else if (itemType == TYPE_OUT) {
			PlayerData.deletePetEgg (curPetData);

			PetData data = new PetData ();
			data.initId (PlayerData.creatPetId(),curPetData.item_id);
			PlayerData.addPet (data);

			LoginController.GetInstance ().creatNewPet (data);
			onClickOut ();


		}else if (itemType == TYPE_SPEED) {
			
			itemInfoPart.SetActive (true);
			itemInfoPart.transform.Find ("ItemImage").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/" + spec.name + "Icon");
			int cost = (curPetData.getLeftTime ().Hours +1);

			GameObject buyButImg = itemInfoPart.transform.Find ("BuyButton").gameObject;
			buyButImg.transform.Find ("Image").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/gemIcon");

			itemInfoPart.transform.Find ("message").GetComponent<Text> ().text = LanController.getString("speedEggMes");

			buyButImg.transform.Find ("Text").GetComponent<Text> ().text = cost.ToString ();
			itemInfoPart.transform.Find ("NameText").gameObject.GetComponent<Text> ().text = LanController.getString ("speedup").ToUpper ();
		}

	}

	int getExtendCost(){
		return  (int)Mathf.Pow( (float)(maxEggsCount - GameController.minPetEggCount + 1f),2f)*1000;
	}
	void showExtendPet(){
		itemType = TYPE_Extend;
		itemInfoPart.SetActive (true);
		itemInfoPart.transform.Find ("ItemImage").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/"  + "HomeIcon08");

		GameObject buyButImg = itemInfoPart.transform.Find ("BuyButton").gameObject;
		buyButImg.transform.Find ("Image").GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/CoinIcon");

		itemInfoPart.transform.Find ("message").GetComponent<Text> ().text = LanController.getString("extendPetEgg");

		buyButImg.transform.Find ("Text").GetComponent<Text> ().text = getExtendCost().ToString ();
		itemInfoPart.transform.Find ("NameText").gameObject.GetComponent<Text> ().text = LanController.getString ("capacity").ToUpper ();
	}

	PetEggData creatNewEgg(){
		
		PetEggData newData = new PetEggData ();
		newData.creatTime = GameController.GetInstance ().getCurrentSystemNum();
		newData.item_id = BattleFormation.getRanPetEggId ();

		return newData;
	}
	public void onInfoBuyClicked(){
		AudioController.PlaySound (AudioController.SOUND_button);
		if (itemType == TYPE_SPEED) {
			int cost = (curPetData.getLeftTime ().Hours + 1);
			if (PlayerData.getgem () < cost) {
				DialogController.GetInstance ().showMessagePanel (LanController.getString ("GemNotEnough"));
			} else {
				PlayerData.setgem (PlayerData.getgem () - cost);
				curPetData.speedup ();
				PlayerData.updatePetEgg (curPetData);
			}
			
		} else if (itemType == TYPE_BUY) {
			if (spec.gem > 0) {
				if (PlayerData.getgem () < spec.gem) {
					DialogController.GetInstance ().showMessagePanel (LanController.getString ("GemNotEnough"));
				} else {
					PlayerData.setgem (PlayerData.getgem () - spec.gem);
					PlayerData.addOwnedItem (spec.item_id, 1);
				}
			} else {
				if (PlayerData.getcoin () < spec.coin) {
					DialogController.GetInstance ().showMessagePanel (LanController.getString ("CoinNotEnough"));
				} else {
					PlayerData.setcoin (PlayerData.getcoin () - spec.coin);
					PlayerData.addOwnedItem (spec.item_id, 1);
				}
			}

		} else if (itemType == TYPE_Extend) {
			if (PlayerData.getcoin () < getExtendCost()) {
				DialogController.GetInstance ().showMessagePanel (LanController.getString ("CoinNotEnough"));
			} else {
				PlayerData.setcoin (PlayerData.getcoin () - getExtendCost());

				PlayerData.setPetEggMaxCount (maxEggsCount + 1);
			}
		}

		itemInfoPart.SetActive (false);
		initFishData (curIndex);
	}
	public void onConfirmLeft()
	{
		AudioController.PlaySound (AudioController.SOUND_button);
		curIndex--;
		if(curIndex <0){
			curIndex = list.Count - 1;
		}
		setPetData (list[curIndex]);
	}
	public void onConfirmRight()
	{
		AudioController.PlaySound (AudioController.SOUND_button);
		curIndex++;
		if(curIndex >= list.Count){
			curIndex = 0;
		}
		setPetData (list[curIndex]);
	}
	public void onClickExtend()
	{
		AudioController.PlaySound (AudioController.SOUND_button);
		if (maxEggsCount < GameController.maxPetEggCount) {
			showExtendPet ();
		}
	}
	public void onItemInfoClose()
	{
		itemInfoPart.SetActive (false);
	}

	public void onClickOut()
	{
		AudioController.PlaySound (AudioController.SOUND_button);
		gameObject.SetActive (false);
		if(GameController.GetInstance().isNewer &&  GameController.GetInstance().curGuildStep == GuilderPanel.Step03){
			DialogController.GetInstance ().showGuiderPanel (GuilderPanel.Step04, LoginController.GetInstance ().UI.transform, LoginController.GetInstance ().UI.transform.Find ("WorldButton").transform.position);

		}
		if (GameController.GetInstance ().CurrentScene == GameController.LOGINSCENE) {
			LoginController.GetInstance ().initTreasure ();
		}
	}

}

