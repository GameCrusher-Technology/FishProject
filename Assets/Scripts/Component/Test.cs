using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
	int number = -1;
	string str = "aa";
	OwnedItem item;
	// Use this for initialization
	void Start () {
		number = -2;
		str = "00";
		item = new OwnedItem ();
		item.item_id = "23";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onClick(){
		Debug.Log (number + " " + str +" " + item.item_id);
	}
}
