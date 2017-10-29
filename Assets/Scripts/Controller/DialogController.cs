using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogController : MonoBehaviour {

	GameObject shopPanel;
	GameObject fishInfoPanel;
	GameObject fishEggsPanel;
	GameObject settingPanel;
	GameObject battleResultPanel;
	GameObject messagePanel;
	GameObject rewardsPanel;
	GameObject titlePanel;
	GameObject	taskPanel;
	GameObject	worldFishPanel;
	private static DialogController _controller;
	public static DialogController GetInstance(){
		if (!_controller) {
			_controller = (DialogController)GameObject.FindObjectOfType(typeof(DialogController));
		}
		if (!_controller) {
			Debug.LogError (" no DialogController");
		}
		return _controller;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	public void showWorldFishPanel(WorldFishData data){
		if(worldFishPanel == null){ 
			worldFishPanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("FishBossPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity, transform);
		}
		worldFishPanel.SetActive (true);
		worldFishPanel.GetComponent<WorldFishPanel> ().init (data);
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}

	public void showFishInfoPanel(int dataId = -1){
		if(fishInfoPanel == null){ 
			fishInfoPanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("FishInfoPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity, transform);
		}
		fishInfoPanel.SetActive (true);
		fishInfoPanel.GetComponent<FishInfoPanel> ().initFishData (dataId);
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}

	public void showFishEggsInfoPanel(){
		if(fishEggsPanel == null){ 
			fishEggsPanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("FishEggPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity, transform);
		}
		fishEggsPanel.SetActive (true);
		fishEggsPanel.GetComponent<FishEggPanel> ().initFishData ();
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}


	public void showRewardsPanel(List<OwnedItem> _list,string message,System.Action callBackAction = null){
		if(rewardsPanel == null){ 
			rewardsPanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("RewardsPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity, transform);
		}
		rewardsPanel.SetActive(true);
		rewardsPanel.GetComponent<RewardsPanel> ().init (_list,message,callBackAction);
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}
		
	public void showSettingPanel(){
		if(settingPanel == null){ 
			settingPanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("SettingPanel"), new Vector3(0,0,0), Quaternion.identity, transform);
		}
		settingPanel.SetActive(true);
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}

	GameObject petToggle;
	public void showPetToggle(int data_id,Vector3 pos){
		if (petToggle == null) { 
			petToggle = (GameObject)Instantiate (GameController.GetInstance ().getPrefab ("PetToggleButs"), pos, Quaternion.identity, transform);
		} else {
			petToggle.transform.position = pos;
		}

		petToggle.SetActive(true);
		petToggle.GetComponent<PetToggleButs> ().initPet (data_id);	
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}
	GameObject missionFishPanel;
	public void showMissionFishPanel(MissionFishData data){
		if(missionFishPanel == null){
			missionFishPanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("MissionFishPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity, transform);
		}
		missionFishPanel.SetActive (true);
		missionFishPanel.GetComponent<MissionFishPanel>().init (data);
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}

	public void showShopPanel(string tab = null){
		if(shopPanel == null){
			shopPanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("ShopPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity, transform);
		}
		shopPanel.SetActive (true);
		shopPanel.GetComponent<ShopPanel>().initPanel (tab);
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}

	public void showMessagePanel(string message,bool isconfirm = false,System.Action action = null,System.Action closeAction = null){
		if(messagePanel == null){ 
			messagePanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("MessagePanel"),new Vector3(Screen.width/2,Screen.height/2,0),Quaternion.identity,transform);
		}
		//messagePanel.GetComponent<RectTransform> ().position = new Vector3(Screen.width,Screen.height*4/5,0);
		messagePanel.transform.Find("Skin").Find ("Text").GetComponent<Text> ().text = message;
		if (action != null) {
			messagePanel.transform.Find ("SkinButton").gameObject.SetActive (true);
			messagePanel.transform.Find ("SkinButton").GetComponent<Button> ().onClick.AddListener (delegate {
				if(closeAction != null){
					closeAction();
				}
				Destroy (messagePanel);
			});
		} else {
			messagePanel.transform.Find ("SkinButton").gameObject.SetActive (false);
		}
		if (isconfirm) {
			messagePanel.transform.Find("Skin").Find ("Button").gameObject.SetActive (true);
			messagePanel.transform.Find("Skin").Find ("Button").Find ("Text").GetComponent<Text> ().text = LanController.getString("confirm").ToUpper();
			messagePanel.transform.Find("Skin").Find ("Button").GetComponent<Button> ().onClick.AddListener (
				delegate() {
					if(action != null){
						action();
					}
					Destroy (messagePanel);
				}
			);
		} else {
			Destroy (messagePanel,2f);
		}
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}
	public void removeMessagePanel(){
		if(messagePanel != null){
			Destroy (messagePanel);
		}
	}

	public void showBattleResult(){
		if(battleResultPanel == null){ 
			battleResultPanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("BattleResultPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity, transform);
		}
		battleResultPanel.SetActive (true);
		battleResultPanel.GetComponent<BattleResultPanel> ().Refresh ();
		AudioController.PlaySound (AudioController.SOUND_Panel);
	}
	GameObject levelUpPanel;
	public void showLevelUpPanel(PetData data){
		if (levelUpPanel == null) { 
			levelUpPanel = (GameObject)Instantiate (GameController.GetInstance ().getPrefab ("PetLevelUpPanel"), new Vector3 (Screen.width / 2, Screen.height / 2, 0), Quaternion.identity, transform);
		}
		levelUpPanel.SetActive (true);
		levelUpPanel.GetComponent<PetLevelUpPanel> ().initPet (data);

		AudioController.PlaySound (AudioController.SOUND_Panel);
	}


	public void showIslandPanel(string landName){
		GameObject levelUpPanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("IslandPanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity, transform);
		levelUpPanel.GetComponent<IslandPanel> ().init (landName);

		AudioController.PlaySound (AudioController.SOUND_Panel);
	}


	public void showTitlePanel(string title){
		if (titlePanel == null) { 
			titlePanel = (GameObject)Instantiate (GameController.GetInstance().getPrefab("TitlePanel"), new Vector3(Screen.width/2,Screen.height/2,0), Quaternion.identity, transform);
		}
		titlePanel.SetActive (true);
		titlePanel.GetComponent<TitlePanel> ().init (title);

		AudioController.PlaySound (AudioController.SOUND_Panel);
	}


	public void showTaskPanel(TaskData data){
		if (taskPanel == null) {
			taskPanel = (GameObject)Instantiate (GameController.GetInstance ().getPrefab ("TaskPanel"), new Vector3 (Screen.width / 2, Screen.height / 2, 0), Quaternion.identity, transform);
			taskPanel.GetComponent<TaskPanel> ().initTask (data);
		} else {
			taskPanel.GetComponent<TaskPanel> ().refreshTask (data);
		}
		taskPanel.SetActive (true);

		AudioController.PlaySound (AudioController.SOUND_Panel);
	}


	GameObject gPanel;
	public void showGuiderPanel(string step, Transform parTrans , Vector3 vecPos){
		GameController.GetInstance ().curGuildStep = step;

		if (gPanel == null) {
			gPanel =  (GameObject)Instantiate (GameController.GetInstance ().getPrefab ("GuilderPanel"), new Vector3 (Screen.width / 2, Screen.height / 2, 0), Quaternion.identity, parTrans);
		} else {
			gPanel.transform.SetParent (parTrans);
		}
		gPanel.SetActive (true);

		gPanel.transform.GetComponent<GuilderPanel> ().initStep (step,vecPos);

	}
}
