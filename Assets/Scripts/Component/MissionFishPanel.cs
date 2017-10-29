using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
public class MissionFishPanel : MonoBehaviour
{
	MissionFishData fishData;
	Text EndTimeText;
	// Use this for initialization
	void Start ()
	{
		transform.Find ("TaskName").GetComponent<Text> ().text = LanController.getString ("MissionFish");
		transform.Find ("ConfirmButton").Find("Text").GetComponent<Text> ().text = LanController.getString ("attack").ToUpper();
	}

	public void init(MissionFishData data){
		fishData = data;
		FishItemSpec spec = SpecController.getItemById (data.item_id) as FishItemSpec;
		if(spec != null){
			transform.Find ("Icon").GetComponent<Image> ().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  spec.name + "Icon");
		}

		EndTimeText = transform.Find ("TimeText").GetComponent<Text> ();
		GameObject render =	transform.Find ("FishPart").Find ("Render").gameObject;
		GameObject listView = transform.Find ("FishPart").Find ("Viewport").gameObject;

		for (int i = 0; i < listView.transform.childCount; i++) {
			GameObject go = listView.transform.GetChild (i).gameObject;
			Destroy (go);
		}

		string lC = data.levelCount;
		string[] arr = lC.Split (new char[1]{ '|' });
		foreach(string str in arr){
			string[] arr1 =  str.Split (new char[1]{ ':'});
			GameObject newRender = Instantiate(render);
			newRender.GetComponent<RectTransform>().SetParent(listView.GetComponent<RectTransform>());
			newRender.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			newRender.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			newRender.SetActive (true);

			newRender.transform.Find ("LevelText").GetComponent<Text> ().text = arr1 [0].ToString ();
			newRender.transform.Find ("Text").GetComponent<Text> ().text = "×"+arr1 [1].ToString ();
			newRender.transform.Find ("Icon").GetComponent<Image> ().sprite = GameController.GetInstance().getSpByName("Pic/ui/" +  spec.name + "Icon");
		}

		int t = arr.Length ;
		float h =Mathf.Max( t * 60 + (t - 1) * 5 + 20,240);
		listView.GetComponent<RectTransform>().sizeDelta = new Vector2(280, h);
		listView.transform.Translate (new Vector3(0,-h/2,0));

		refreshEndTime ();
	}
	bool isOutTime = false;
	// Update is called once per frame
	void Update ()
	{
		if(!isOutTime){
			refreshEndTime();	
		}
	}

	void refreshEndTime(){
		if(fishData != null){
			int t = Mathf.Max(0, fishData.getEndTime () - GameController.GetInstance ().getCurrentSystemNum ());
			TimeSpan span = new TimeSpan (0,0,t);
			EndTimeText.text = span.ToString();	
			if(t == 0 ){
				isOutTime = true;
			}
		}
	}
	public void onClickOut(){
		gameObject.SetActive (false);

	}

	public void onClickConfirm(){
		GameController.GetInstance ().missionFishData = fishData;
		GameController.GetInstance ().BeginBattle ();

	}

}

