using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour {
	public Slider MusicSlider;
	public Slider SoundSlider;



	// Use this for initialization
	void Start () {
		MusicSlider.value = PlayerPrefs.GetFloat (PlayerData.Music);
		SoundSlider.value = PlayerPrefs.GetFloat (PlayerData.Sound);
		transform.Find ("skin").Find ("CloseButton").Find ("Text").GetComponent<Text> ().text = LanController.getString ("confirm").ToUpper();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onMusicValueChanged(){
		PlayerPrefs.SetFloat (PlayerData.Music,MusicSlider.value);
		LoginController.GetInstance ().setMusic (MusicSlider.value);

	}
	public void onSoundValueChanged(){
		PlayerPrefs.SetFloat (PlayerData.Sound,SoundSlider.value);
	}

	public void onClickOut(){
		gameObject.SetActive (false);
	}
}
