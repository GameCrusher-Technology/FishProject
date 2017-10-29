using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PetLevelUpPanel : MonoBehaviour
{
	PetData curData;
	PetData newData;
	FishInfoTip tipOld;
	FishInfoTip tipnew;
	// Use this for initialization
	void Start ()
	{
		
	}

	public void initPet(PetData data){
		curData = data;
		newData = data.getNextLevelData ();

		transform.Find ("TitleText").GetComponent<Text> ().text = LanController.getString ("levelUpPET").ToUpper();
		transform.Find ("FishButton").GetComponent<FishInfoTip> ().init (curData);
		transform.Find ("NewFishButton").GetComponent<FishInfoTip> ().init (newData);
		transform.Find ("CoinButton").Find ("Text").GetComponent<Text> ().text = LanController.getString ("confirm").ToUpper ();
		transform.Find ("TipText").GetComponent<Text> ().text = LanController.getString ("PetLevelUpTip");

		transform.Find ("CoinText").GetComponent<Text> ().text = curData.getLevelCost().ToString();
		transform.Find ("ExpText").GetComponent<Text> ().text = curData.getUpgradeExp().ToString();
	}
	// Update is called once per frame
	void Update ()
	{
	
	}
	public void ClickConfirm(){
		if (PlayerData.getcoin () < curData.getLevelCost ()) {
			DialogController.GetInstance ().showMessagePanel (LanController.getString ("CoinNotEnough"));
		} else if (PlayerData.getFishExp() < curData.getUpgradeExp ()) {
			DialogController.GetInstance ().showMessagePanel (LanController.getString ("ExpNotEnough"));
		} else {
			PlayerData.setcoin (PlayerData.getcoin () - curData.getLevelCost ());
			PlayerData.setFishExp (PlayerData.getFishExp () - curData.getUpgradeExp ());
			curData.level++;
			PlayerData.updatePetData (curData);
			AudioController.PlaySound (AudioController.SOUND_FULLSKILL);
		}
		ClickOut ();
	}
	public void ClickOut(){
		gameObject.SetActive (false);
	}


}

