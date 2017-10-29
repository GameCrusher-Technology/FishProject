using UnityEngine;
using System.Collections;

public class PetToggleButs : MonoBehaviour {
	int data_id;
	public void initPet(int dataId){
		data_id = dataId;

		if (GameController.GetInstance ().CurrentScene == GameController.BATTLESCENE) {
			transform.Find ("Grid").Find ("Info").gameObject.SetActive (false);
			//Destroy (transform.FindChild ("Grid").FindChild ("Info").gameObject);
		}
		StartCoroutine (delayShow());
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onClickInfo(){
		DialogController.GetInstance ().showFishInfoPanel (data_id);
		gameObject.SetActive (false);
	}

	public void onClickMove(){
		GameController.GetInstance ().movingPetDataId = data_id;
		gameObject.SetActive (false);
	}

	IEnumerator delayShow(){
		yield return new WaitForSeconds (3f);
		gameObject.SetActive (false);
	}
}
