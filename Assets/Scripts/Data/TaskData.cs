using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class TaskData  {
	public string taskId;
	public string conditions;
	public string rewards;


	public void init(string id){
		taskId = id;


		if (taskId == "random") {
			Dictionary<string,TaskItem> requireDic = BattleFormation.getRanTaskFishList ();
			foreach (string key in requireDic.Keys) {
				TaskItem item = requireDic[key];
				requireList.Add (item.item_id, item);

			}
		} else {
			TaskItemSpec spec  = getTaskSpec();

			List<OwnedItem> specrequireList = spec.getRequireList ();
			foreach (OwnedItem item in specrequireList) {
				TaskItem item1 = new TaskItem ();
				item1.item_id = item.item_id;
				item1.finished = 0;
				item1.total = item.count;

				requireList.Add (item.item_id, item1);
			}
		}
		creatRewards ();
		setSaveData ();
	}

	void creatRewards(){
		rewardList = new Dictionary<string, int> ();
		if (taskId == "random") {
			int expCount = 0;
			int coin = 0;
			foreach (TaskItem item in requireList.Values) {
				FishItemSpec fSpec = SpecController.getItemById (item.item_id) as FishItemSpec;
				expCount += fSpec.common * 5;
				coin += fSpec.coin * item.total * 2;
			}

			rewardList.Add ("exp", expCount);
			rewardList.Add ("coin", coin);

		} else {
			List<OwnedItem> specrewardList = getTaskSpec().getRewardList ();
			foreach (OwnedItem item in specrewardList) {
				rewardList.Add (item.item_id, item.count);
			}
		}
		setSaveData ();
	}

	public TaskItemSpec getTaskSpec(){
		return SpecController.getItemById (taskId) as TaskItemSpec;
	}


	Dictionary<string,TaskItem> requireList  = new Dictionary<string, TaskItem> ();
	Dictionary<string,int> rewardList  = new Dictionary<string, int> ();

	public Dictionary<string,TaskItem> getRequireList(){
		if (requireList.Count==0) {
			requireList = new Dictionary<string, TaskItem> ();
			if (conditions != null) {
				requireList = JsonMapper.ToObject<Dictionary<string, TaskItem>> (conditions);

			/*	string[] arr = conditions.Split (new char[1]{ '|' });
				foreach (string str in arr) {
					string[] arr1 = str.Split (new char[1]{ ':' });
					TaskItem item = new TaskItem ();
					item.item_id = arr1 [0];
					item.finished = int.Parse(arr1 [1]);
					item.total = int.Parse(arr1 [2]);
					requireList.Add (arr1 [0], item);
				}*/
			} else {
				TaskItemSpec spec = getTaskSpec ();
				List<OwnedItem> specrequireList = spec.getRequireList ();
				foreach (OwnedItem item in specrequireList) {

					TaskItem item1 = new TaskItem ();
					item1.item_id = item.item_id;
					item1.finished = 0;
					item1.total = item.count;

					requireList.Add (item.item_id,item1);
				}
			}
		} 
		return requireList;
	}


	public Dictionary<string,int> getRewardList(){
		if (rewardList.Count==0) {
			rewardList = new Dictionary<string, int> ();
			if (rewards != null ) {
				rewardList = JsonMapper.ToObject<Dictionary<string, int>> (rewards);
			}

		}
		if (rewardList.Count == 0) {
			creatRewards ();
		}
		return rewardList;
	}
		
	public void addItem( string id, int count =1){
		Dictionary<string,TaskItem> list = getRequireList ();
		if(list.ContainsKey(id)){
			list [id].finished += count;
		}
		setSaveData ();
	}

	private void setSaveData(){
		rewards = JsonMapper.ToJson(rewardList);
		conditions = JsonMapper.ToJson (requireList);
	}
}

