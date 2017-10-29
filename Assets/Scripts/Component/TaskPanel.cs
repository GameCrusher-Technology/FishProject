using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TaskPanel : MonoBehaviour
{

	TaskData taskdata;
	GameObject requireRender;
	GameObject rewardRender;
	GameObject	requireGrid;
	GameObject	rewardGrid;
	Text confirmButText;

	bool hasFinished = true;

	// Use this for initialization
	void Start ()
	{
		
		requireRender = transform.Find ("RequirePart").Find ("Render").gameObject;
		rewardRender = transform.Find ("RewardPart").Find ("Render").gameObject;

		requireGrid = transform.Find ("RequirePart").Find  ("Viewport").gameObject;
		rewardGrid = transform.Find ("RewardPart").Find  ("Viewport").gameObject;

		confirmButText = transform.Find ("ConfirmButton").Find ("Text").GetComponent<Text>();

		configTask ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}


	public void initTask(TaskData data){
		taskdata = data;
	}

	public void refreshTask(TaskData data){
		taskdata = data;
		configTask ();
	}
	Dictionary<string,TaskItem> requireList ;
	Dictionary<string,int> rewardList;
	void configTask(){


		if(taskdata.taskId == "random"){
			transform.Find ("TaskName").GetComponent<Text> ().text = LanController.getString ("randomTask").ToUpper();
		}else{
			
			TaskItemSpec taskspec = taskdata.getTaskSpec ();			
			transform.Find ("TaskName").GetComponent<Text> ().text = LanController.getString (taskspec.name).ToUpper();
		}
	

		for (int i = 0; i < requireGrid.transform.childCount; i++) {
			GameObject go = requireGrid.transform.GetChild (i).gameObject;
			Destroy (go);
		}
		for (int i = 0; i < rewardGrid.transform.childCount; i++) {
			GameObject to = rewardGrid.transform.GetChild (i).gameObject;
			Destroy (to);
		}

		requireList = taskdata.getRequireList ();
		rewardList = taskdata.getRewardList ();

		hasFinished = true;
		foreach (string id in requireList.Keys) {

			GameObject bt = Instantiate(requireRender);
			bt.GetComponent<RectTransform>().SetParent(requireGrid.GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			bt.SetActive (true);

			TaskItem taskItem = requireList [id];

			GameObject	completeIcon = bt.transform.Find ("CompletIcon").gameObject;

			int completeC = Mathf.Min(taskItem.finished, taskItem.total);

			bt.transform.Find ("CompleteText").GetComponent<Text> ().text =  completeC.ToString();
			bt.transform.Find ("TotalText").GetComponent<Text> ().text = "/" +  taskItem.total.ToString();
			if (taskItem.beFinished()) {
				completeIcon.SetActive (true);

			} else {
				completeIcon.SetActive (false);
				hasFinished = false;
			}
			ItemSpec spec = SpecController.getItemById (id);
			bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" + spec.name + "Icon");

		}

		int t = (requireList.Count % 2) == 0 ? (requireList.Count / 2) : ((requireList.Count/ 2) + 1);
		float h = t * 120 + (t - 1) * 10 + 20;
		requireGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(requireGrid.GetComponent<RectTransform>().sizeDelta.x, h);
		requireGrid.transform.Translate (new Vector3(0,-h/2,0));


		foreach (string key in rewardList.Keys) {
			GameObject bt = Instantiate(rewardRender);
			bt.GetComponent<RectTransform>().SetParent(rewardGrid.GetComponent<RectTransform>());
			bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			bt.SetActive (true);

			bt.transform.Find ("Text").GetComponent<Text> ().text = "×" +  rewardList[key].ToString();
			if(key== "gem"){
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  "gemIcon");
		
			}else if(key== "coin"){
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  "CoinIcon");
			}else if(key== "bullet"){
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  "BulletShopIcon");
			}else if(key== "exp"){
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  "ExpIcon");
			}else if(key== "ship"){
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  "PirateBoat");
			}else
			{
				ItemSpec spec = SpecController.getItemById (key);
				bt.transform.Find("Icon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" + spec.name + "Icon");
			}
		}

		if (hasFinished) {
			confirmButText.text = LanController.getString ("complete").ToUpper();
		} else {
			confirmButText.text = LanController.getString ("confirm").ToUpper();
		}
	}

	public void onClickConfirm(){
		if (hasFinished) {
			foreach (string key in rewardList.Keys) {
				if (key == "gem") {
					PlayerData.setgem (PlayerData.getgem () + rewardList [key]);
				} else if (key == "coin") {
					PlayerData.setcoin (PlayerData.getcoin () + rewardList [key]);
				} else if (key == "bullet") {
					PlayerData.addBulletCount (rewardList [key]);
				} else if (key == "exp") {
					PlayerData.setFishExp (PlayerData.getFishExp () + rewardList [key]);
				} else if (key == "ship") {
					
				} else {
					PlayerData.addOwnedItem (key, rewardList [key]);
				}
			}
			GameController.GetInstance ().finishTask (taskdata.taskId);
			if (PlayerData.getCurTask () != null) {
				DialogController.GetInstance ().showTaskPanel (PlayerData.getCurTask ());
			} else {
				onClickOut ();
			}
		} else {
			onClickOut ();
		}
	}

	public void onClickOut(){
		gameObject.SetActive (false);

		if(GameController.GetInstance().isNewer &&  GameController.GetInstance().curGuildStep == GuilderPanel.Step02){
			DialogController.GetInstance ().showGuiderPanel (GuilderPanel.Step03, LoginController.GetInstance ().UI.transform, LoginController.GetInstance ().UI.transform.Find ("FishPetsButton").transform.position);

		}
	}

}

