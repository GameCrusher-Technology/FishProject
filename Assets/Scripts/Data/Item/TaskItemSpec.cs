using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TaskItemSpec : ItemSpec {
	public string require;
	public string reward;

	public List<OwnedItem> getRequireList(){
		string[] arr =  require.Split (new char[1]{'|'});
		List<OwnedItem> list = new List<OwnedItem>();
		foreach(string str in arr){
			string[] arr1 = str.Split (new char[1]{':'});
			OwnedItem item = new OwnedItem ();
			item.item_id = arr1 [0];
			item.count =int.Parse( arr1 [1]);
			list.Add (item);
		}
		return list;
	}
	public List<OwnedItem> getRewardList(){
		string[] arr =  reward.Split (new char[1]{'|'});
		List<OwnedItem> list = new List<OwnedItem>();
		foreach(string str in arr){
			string[] arr1 = str.Split (new char[1]{':'});
			OwnedItem item = new OwnedItem ();
			item.item_id = arr1 [0];
			item.count = int.Parse( arr1 [1]);
			list.Add (item);
		}
		return list;
	}
}
