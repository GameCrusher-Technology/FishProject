using UnityEngine;
using System.Collections;
using System;
using LitJson;
using System.Collections.Generic;
using UnityEngine.Purchasing;

public class PlatFormController:MonoBehaviour
{
	private static PlatFormController _controller;
	public static PlatFormController GetInstance(){
		if (!_controller) {
			_controller = (PlatFormController)GameObject.FindObjectOfType(typeof(PlatFormController));
		}
		if (!_controller) {
			Debug.LogError (" no PlatFormController");
		}
		return _controller;
	}

	public void buyCommand(string id,string transid,string receipt){
		string uid = PlayerData.getUid ();
		string gameuid = PlayerData.getGameuid ();
		WWWForm form = new WWWForm ();
		form.AddField ("command",Command.PLATFORM_IAP);
		AmfObject amf = new  AmfObject ();
		amf.uid = uid;
		amf.gameuid = gameuid.ToString();
		amf.receiptStr  = receipt;
		amf.status  = transid;
		amf.productId = id;
		form.AddField ("data",JsonUtility.ToJson(amf));	

		StartCoroutine (command(Command.ip,form));
	}


	IEnumerator command(string phpUrl, WWWForm form){
		WWW www = new WWW(phpUrl, form);
		while(!www.isDone){
			//Debug.Log ("wait...");
		}
		yield return www;

		if (www.error != null) {
			Debug.LogError (www.error);
			DialogController.GetInstance ().showMessagePanel (LanController.getString("paymentError02"));
		} else {
			Debug.Log (www.text);
			AmfObject jsonData = JsonMapper.ToObject<AmfObject> (www.text);
			Debug.Log ("test" + jsonData.productId);
			if(jsonData.status == "ok"){
				switch (jsonData.productId) {
				case GameController.littleCoins:
					PlayerData.setcoin (PlayerData.getcoin() + 10000);
					break;
				case GameController.largeCoins:
					PlayerData.setcoin (PlayerData.getcoin() + 66666);
					break;
				case GameController.littleGems:
					PlayerData.setgem (PlayerData.getgem() + 100);
					break;
				case GameController.largeGems:
					PlayerData.setgem (PlayerData.getgem() + 1500);
					break;

				}
			}
			if(GameController.GetInstance().CurrentScene == GameController.LOGINSCENE){
				LoginController.GetInstance ().initTreasure ();
			}
			DialogController.GetInstance ().showMessagePanel (LanController.getString("paymentSuc"));
		}
	}

}

