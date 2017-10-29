using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Advertisements;


public class BattleResultPanel : MonoBehaviour {

	GameObject fishList;
	GameObject listItemRender;
	Transform coinPart;
	GameObject listGrid;
	Dictionary<string,int> fishes;
	GameObject notText;
	GameObject finishBut;
	Text petExpText;

	int coins;
	int petExp ;
	void Awake () {
		fishList =  transform.Find ("FishList").gameObject;
		listItemRender = fishList.transform.Find ("FishRender").gameObject;

		coinPart =  transform.Find ("CoinButton");
		listGrid =fishList.transform.Find ("Grid").gameObject;
		notText = transform.Find ("NotText").gameObject;
		notText.GetComponent<Text> ().text = LanController.getString ("notFish");

		finishBut = transform.Find ("FinishButton").gameObject;
		finishBut.transform.Find ("Text").GetComponent<Text> ().text = LanController.getString ("exit").ToUpper();

		transform.Find ("TitleText").GetComponent<Text> ().text = LanController.getString ("rewards").ToUpper();

		petExpText = transform.Find ("PetCanvas").Find ("Text").GetComponent<Text> ();

		coins = 0;
	}
	
	// Update is called once per frame


	public void Refresh () {
		coins = 0;
		fishList =  transform.Find ("FishList").gameObject;
		listItemRender = fishList.transform.Find ("FishRender").gameObject;
		coinPart =  transform.Find ("CoinButton");
		listGrid =fishList.transform.Find ("Grid").gameObject;

		for (int i = 0; i < listGrid.transform.childCount; i++) {  
			Destroy (listGrid.transform.GetChild (i).gameObject);  
		}  

		notText = transform.Find ("NotText").gameObject;


		petExp = BattleController.GetInstance ().getPetCatchExp ();
		petExpText.text = petExp.ToString ();


		notText.GetComponent<Text> ().text = LanController.getString ("notFish");

		Dictionary<string,int> fishes = BattleController.GetInstance ().catchFishes;

		if (fishes.Count <= 0) {
			notText.SetActive (true);
			coinPart.gameObject.SetActive (false);
			finishBut.SetActive (true);

		} else {
			notText.SetActive (false);
			coinPart.gameObject.SetActive (true);
			finishBut.SetActive (false);

			foreach (string key in fishes.Keys) {
				int count = fishes [key];
				GameObject bt = Instantiate(listItemRender);
				bt.GetComponent<RectTransform>().SetParent(listGrid.GetComponent<RectTransform>());
				ItemSpec spec = SpecController.getItemById (key);
				bt.SetActive (true);
				bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
				bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
				bt.name = spec.item_id;
				bt.transform.Find ("CountText").gameObject.GetComponent<Text> ().text = "" + count;
				bt.transform.Find("Icon").gameObject .GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" + spec.name + "Icon"); 
				coins += spec.coin * count;
			}

			int p = 4;
			int t = (fishes.Count % p) == 0 ? (fishes.Count / p) : ((fishes.Count / p) + 1);
			int h = t * 100 + (t - 1) * 10 + 20;
			listGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(500, h);
			listGrid.transform.Translate (new Vector3(0,-h/2,0));

			coinPart.Find("Text").GetComponent<Text>().text = "" + coins;
		}
	}

	public void ClickConfirm(){
		PlayerData.setcoin (PlayerData.getcoin()+ coins);
		PlayerData.setFishExp (PlayerData.getFishExp()+ petExp);


		hidePanel ();
		BattleController.GetInstance ().endFishing ();
		GameController.GetInstance ().HomeScene ();
		GameController.GetInstance ().saveData ();


		//ShowRewardedAd ();
		//if (Advertisement.IsReady())
		//{
		//	Advertisement.Show();
		//}

	}

/*	void ShowRewardedAd()
	{
			if (Advertisement.IsReady("rewardedVideo"))
			{
				var options = new ShowOptions { resultCallback = HandleShowResult };
				Advertisement.Show("rewardedVideo", options);
			}

	}
	void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			//
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.

			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}*/
	public void ClickOut(){
		hidePanel ();
	}

	void hidePanel(){
		gameObject.SetActive (false);
	}
}
