using UnityEngine;
using System.Collections;

public class ItemSpec{
	public string item_id;
	public string name;
	public int coin ;
	public int gem ;
	public int count ;
	public string message;
	public int common ;

	public string getMessage(){
		if (message != null && message !="") {
			return LanController.getString (message);
		} else {
			return "";
		}
	}
	public int getItemType(){
		return (int.Parse(item_id) / 10000);
	}

}
