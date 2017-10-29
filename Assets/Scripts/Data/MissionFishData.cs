using UnityEngine;
using System.Collections;

public class MissionFishData
{
	public int fishLevel = 1;
	public string item_id;
	public int showTime ;
	public string walkType;
	public int wholeTime = 300;
	public string levelCount = "1:10|3:3";

	public int getEndTime(){
		return showTime + fishLevel* 4 * 3600;
	}

	FishItemSpec _spec;
	public FishItemSpec getSpec(){
		if(_spec == null){
			_spec = SpecController.getItemById (item_id) as FishItemSpec;
		}
		return _spec;
	}
	public bool isEnd(){
		return (getEndTime())  <  GameController.GetInstance().getCurrentSystemNum();
	}
}

