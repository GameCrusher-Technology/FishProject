using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioController : MonoBehaviour {
	public const string SOUND_GUN = "gunShot";
	public const string SOUND_BATTLER = "refreshBattle";
	public const string SOUND_FULLSKILL = "petFullSkill";
	public const string SOUND_USESKILL = "explosion";
	public const string SOUND_Panel = "pannel";
	public const string SOUND_button = "buttonClick";
	public const string SOUND_coin = "coin";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void PlaySound(string name, float _delay = 0,float sVolume = 1f){
		Transform emitter = Camera.main.transform;

		GameObject oj = new GameObject ("Audio_"+name);
		oj.transform.position = emitter.position;
		oj.transform.SetParent (emitter);
		AudioSource source = oj.AddComponent<AudioSource> ();
		source.clip = getClip (name);
		source.volume = PlayerData.getSound()*sVolume;
		source.pitch = 1f;
		source.Play ();

		if (_delay > 0) {
			source.loop = true;		
			Destroy (oj,_delay);
		} else {
			Destroy (oj,source.clip.length);
		}


	}

	private static Dictionary<string,AudioClip> clips = new  Dictionary<string, AudioClip> ();
	public static AudioClip getClip(string name){
		if (clips.ContainsKey (name)) {
			return clips [name];
		} else {
			AudioClip aClip = new AudioClip ();
			aClip = Resources.Load ("Audio/"+name,aClip.GetType()) as AudioClip;
			clips.Add (name, aClip);
			return aClip;
		}

	}

}
