using UnityEngine;
using System.Collections;

public class WorldFishPanel : MonoBehaviour
{
	WorldFishData worldFishData;
	public void init(WorldFishData data){
		worldFishData = data;
	}
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void clickFight(){
		GameController.GetInstance ().bossData = worldFishData;
		GameController.GetInstance ().BeginBattle ();
	}

	public void clickOut(){
		gameObject.SetActive (false);
	}
}

