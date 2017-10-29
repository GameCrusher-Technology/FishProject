using UnityEngine;
using System;

public class PetEggData {
	public int data_id;
	public string item_id;
	public double creatTime;

	FishItemSpec spec;

	private FishItemSpec getSpec(){
		if (spec == null) {
			spec = SpecController.getItemById (item_id) as FishItemSpec;
		}
		return spec;
	}

	public bool canSetOut(){
		return true;
	}

	public TimeSpan getLeftTime(){
		
		int t=  Math.Max(0,  getSpec().growtime*60*60 - (int)(System.DateTime.Now - getCreatDate ()).TotalSeconds);
		TimeSpan span = new TimeSpan (0,0,t);
		return span;
	}

	public void speedup(){
		creatTime = 0;
		creatDate  = new DateTime(1970,1,1);
	}
	DateTime creatDate  = new DateTime(1970,1,1);
	DateTime getCreatDate(){
		if(creatDate .Year == 1970){
			DateTime dTime = TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (1970, 1, 1));
			long ltime = long.Parse (creatTime + "0000000");
			TimeSpan toNow = new TimeSpan (ltime);
			creatDate = dTime.Add (toNow);
		}
		return creatDate;
	}
}

