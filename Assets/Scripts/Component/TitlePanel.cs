using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LitJson;

public class TitlePanel : MonoBehaviour
{
	InputField inputText;
	// Use this for initialization
	void Start ()
	{
		transform.Find ("TitleText").GetComponent<Text> ().text = LanController.getString ("Congratulations").ToUpper();
		transform.Find ("ConfirmButton").Find ("Text").GetComponent<Text> ().text = LanController.getString ("confirm").ToUpper();

		inputText = transform.Find ("InputField").GetComponent<InputField> ();
		transform.Find ("InputField").Find("Placeholder").GetComponent<Text> ().text = LanController.getString ("inputTextPlace");
	}
	public void init(string title ="Title01")
	{
		transform.Find("TitleEffect").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" + title+"Icon"); 
		transform.Find ("Name").GetComponent<Text>().text = LanController.getString (title).ToUpper();
	
		transform.Find ("MessageText").GetComponent<Text> ().text = LanController.getString ("inputTextMessage");
	}
	// Update is called once per frame
	void Update ()
	{
		
	}


	public void ClickOut(){
		hidePanel ();
	}
	public void ClickConfirm(){
		Debug.Log (inputText.text); 
		if (inputText.text == "" || inputText.text == null) {
			DialogController.GetInstance ().showMessagePanel (LanController.getString("inputTextMessage"));	
		} else {
			setNameCommand (inputText.text);
			PlayerData.setPlayerName (inputText.text);
			PlayerData.addTitle (GameController.TITLE01);
		}
	}

	void setNameCommand(string name){
		string uid = PlayerData.getUid ();
		string gameuid = PlayerData.getGameuid ();
		WWWForm form = new WWWForm ();
		form.AddField ("command",Command.SETPROFILE);
		AmfObject amf = new AmfObject();
		amf.uid = uid;
		amf.gameuid = gameuid.ToString();
		amf.name = name;
		amf.title = GameController.TITLE01;

		form.AddField ("data",JsonUtility.ToJson(amf));		
		StartCoroutine (command(Command.ip,form));

	}

	IEnumerator command(string phpUrl, WWWForm form){
		WWW www = new WWW(phpUrl, form);
		while(!www.isDone){
			Debug.Log ("wait...");
		}
		yield return www;

		if (www.error != null) {
			Debug.LogError (www.error);
		} else {
			Debug.Log (www.text);
		}
		hidePanel ();
	}

	void hidePanel(){
		gameObject.SetActive (false);
	}


}

