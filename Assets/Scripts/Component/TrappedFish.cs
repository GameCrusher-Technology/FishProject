using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrappedFish : MonoBehaviour
{
	string itemId;
	// Use this for initialization
	void Start ()
	{
	
	}
	HomeObject hObject;
	public void init(HomeObject obj){
		hObject = obj;
		FishItemSpec spec = SpecController.getItemById (obj.trappedId) as FishItemSpec;
		transform.Find ("Fish").GetComponent<Animator> ().Play (spec.name);
	}




	// Update is called once per frame
	void Update ()
	{
	
	}

	public bool canBeClick(){
		return GameController.GetInstance ().getCurrentSystemNum () >= hObject.trappedTime;
	}
	public void onClick(){

		List<OwnedItem> list = new List<OwnedItem> ();
		OwnedItem item = new OwnedItem ();
		item.item_id = "bullet";
		item.count = 100;
		list.Add (item);

		OwnedItem item1 = new OwnedItem ();
		item1.item_id = "coin";
		item1.count = 100;
		list.Add (item1);


		DialogController.GetInstance ().showRewardsPanel (list,LanController.getString("HelpReward"));

		Destroy (gameObject);
		GameController.GetInstance ().creatTrappedFish (hObject, GameController.GetInstance ().getCurrentSystemNum () + 3600*4);
	}
}

