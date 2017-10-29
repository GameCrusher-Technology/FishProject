using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFormation
{
	public const string RandomMode = "random";
	public const string ArrowMode = "arrow";
	public const string BossMode = "boss";
	public const string EnemyMode = "enemy";
	public const string MissionMode = "mission";

	public static List<FishData> getFishList(string mode){
		if(mode  == ArrowMode){
			Vector2 bounds = GameController.WorldBound;
			float arL = Mathf.Max (3f, (Screen.height * 0.35f) /Random.Range(40,60));
			FishItemSpec spec;
			List<string> ar =new List<string>();
			int[] rat = {80,30,5};

			while(arL > 0){
				spec = getRandFishId (rat);
				ar.Add (spec.item_id);
				arL--;
			}
			int spacetime = Random.Range (1,2);
			int wavetime = Random.Range (3,4);
			int speed = Random.Range (20,40);
			int waves = Random.Range (10,20);
			return creatA (ar,new Vector3(-1,0,0),spacetime,wavetime,speed,waves);

		}else if(mode == RandomMode){
			return randList ();
		}else if(mode == BossMode){
			
		}else if(mode == MissionMode){
			return getMissionFishList (GameController.GetInstance ().missionFishData);
		}
		return new List<FishData> ();
	}

	//mission fish
	public static List<FishData> getMissionFishList(MissionFishData data){
		List<FishData> fishList = new  List<FishData>();
		Vector2 bounds = GameController.WorldBound;
		FishItemSpec spec = SpecController.getItemById (data.item_id) as FishItemSpec;
		GameObject fishObj = GameController.GetInstance().getPrefab(spec.name);
		string lC = data.levelCount;
		string[] arr = lC.Split (new char[1]{ '|' });
		float t = bounds.x * 0.1f;

		for(int j =0;j< arr.Length ;j++){
			string[] arr1 =  arr[j].Split (new char[1]{ ':'});
			int c = Mathf.FloorToInt (int.Parse (arr1 [1]) / 2) + 1;
			for(int i =0;i< c;i++){
				if (i == 0) {
					fishList.Add(new FishData (spec, new Vector3(0,-1,0), new Vector3(0,bounds.y*1.2f,0), j*2 +(i+1)*0.5f, 50, 0));
				} else {
					fishList.Add( new FishData (spec, new Vector3(0,-1,0), new Vector3(i*t,bounds.y*1.2f,0),  j*2 +(i+1)*0.5f, 50, 0));
					fishList.Add(new FishData (spec, new Vector3(0,-1,0), new Vector3(-i*t,bounds.y*1.2f,0), j*2 +(i+1)*0.5f, 50, 0));
				}
			}
		}
		return fishList;
	}

	//箭头形状

	private static List<FishData> creatA(List<string> fishArr,Vector3 direction,int spacetime,int wavetime,int speed,int waves =1){
		List<FishData> fishList = new  List<FishData>();
		FishItemSpec spec;
		Vector2 bounds = GameController.WorldBound;
		float t = bounds.y * 0.7f/ fishArr.Count;
		for (int w = 0; w < waves; w++){
			for (int i = 0; i < fishArr.Count; i++) {
				spec = SpecController.getItemById (fishArr[i]) as FishItemSpec;
				if (i == 0) {
					fishList.Add( new FishData (spec, direction, new Vector3(bounds.x,0,0), w*wavetime +(i+1)*spacetime,speed,0));
				} else {
					fishList.Add(  new FishData (spec, direction, new Vector3(bounds.x,i*t ,0), w*wavetime +(i+1)*spacetime,speed,0));
					fishList.Add(  new FishData (spec, direction, new Vector3(bounds.x,-i*t,0), w*wavetime +(i+1)*spacetime,speed,0));
				}
			}
		}
		return fishList;
	}

	//随机刷

	private static List<FishData> randList(){
		List<FishData> fishList = new  List<FishData>();

		int[] ratCommon = {80,30,5 };
		Vector2 bounds = GameController.WorldBound;
		FishItemSpec spec = getRandFishId(ratCommon);

		int count = 1;
		if(spec.common == 1){
			count = Random.Range (3, 8);
		}else if(spec.common == 2){
			count = Random.Range (1, 3);
		}
		float r = Random.value;
		Vector3 pos;
		Vector2 pivatVec;
		if (r > 0.6) {
			//left
			pos = new Vector3 (-bounds.x * 1.2f, Random.Range (-bounds.y * 0.8f, bounds.y * 0.8f), 0);
			pivatVec = new Vector2 (-1f,0); 
		} else if (r > 0.2) {
			//right
			pos = new Vector3 ( bounds.x * 1.2f , Random.Range (-bounds.y * 0.8f, bounds.y * 0.8f), 0);
			pivatVec = new Vector2 (1f,0); 
		} else if (r > 0.1) {
			//top
			pos =	new Vector3 (Random.Range (- bounds.x*1.5f, bounds.x*1.5f),bounds.y*1.2f, 0);
			pivatVec = new Vector2 (0,1f); 
		} else {
			//bottom
			pos =	new Vector3 (Random.Range (- bounds.x*1.5f, bounds.x*1.5f),-bounds.y*1.2f, 0);
			pivatVec = new Vector2 (0,-1f); 
		}
			
		Vector3 tarPos = new Vector3 (Random.Range (-bounds.x*0.5f, bounds.x*0.5f),Random.Range (- bounds.y*0.5f, bounds.y*0.5f), 0);
		Vector3 direction = (tarPos - pos).normalized;
		float fishPivate = Random.value *0.4f+0.4f;

		float px;
		float py;

		for (int i = 0; i < count; i++) {
			
			if(pivatVec.x == 0){
				px = Random.value - 0.5f;
				py = pivatVec.y;
			}else {
				px = pivatVec.x;
				py = Random.value - 0.5f;
			}

			Vector3 curPos = new Vector3 (pos.x + i*fishPivate*px,pos.y + i*fishPivate*py,0);
			fishList.Add (new FishData (spec, direction, curPos, 1f,0,2));
		}
		return fishList;
	}

	private static FishItemSpec getRandFishId(int[] rat){
		int type = getRatINT (rat);
		List<FishItemSpec> dic =  getFishDic()[type +1];
		return dic [Random.Range (0, dic.Count)];
	}
	private static int getRatINT(int[] arr){
		int sum = 0;
		foreach(int v in arr){
			sum += v;
		}
		int rat = Random.Range (0,sum);
		for(int i =0;i<arr.Length;i++){
			if (arr [i] >= rat) {
				return i;
			} else {
				rat -= arr[i];
			}
		}
		return arr.Length - 1;

	}

	private static Dictionary<int,List<FishItemSpec>> fishDic;
	private static Dictionary<int,List<FishItemSpec>> getFishDic(){
		if(fishDic == null){
			fishDic = new Dictionary<int, List<FishItemSpec>> ();
			Dictionary<string,ItemSpec>dic =  SpecController.getGroup ("Fish");
			foreach (ItemSpec spec in dic.Values) {
				int com = (spec as FishItemSpec).common;
				if(fishDic.ContainsKey(com)){
					fishDic [com].Add (spec as FishItemSpec);
				}else{
					fishDic.Add (com,new List<FishItemSpec> (){spec as FishItemSpec});
				}
			}
		}
		return fishDic;
	}

	public static Dictionary<string,TaskItem> getRanTaskFishList(){
		int m = Random.Range (1, 2);
		Dictionary<string,TaskItem> fishDic = new Dictionary<string, TaskItem> ();

		while(m>0){
			int[] ratCommon = {80,30,5 };
			Vector2 bounds = GameController.WorldBound;
			FishItemSpec spec = getRandFishId(ratCommon);

			int count = 10;
			if(spec.common == 1){
				count = Random.Range (30, 50);
			}else if(spec.common == 2){
				count = Random.Range (10, 20);
			}else if(spec.common == 3){
				count = Random.Range (5, 10);
			}

			TaskItem item = new TaskItem ();
			item.item_id = spec.item_id;
			item.finished = 0;
			item.total = count;

			if (fishDic.ContainsKey (spec.item_id)) {
				continue;
			} else {
				fishDic.Add (spec.item_id,item);
			}

			m--;
		}

		return fishDic;
	}

	public static string getRanPetEggId(){
		int[] ratCommon = {80,30,5 };
		FishItemSpec spec = getRandFishId(ratCommon);
		return spec.item_id;
	}

	public static string getRanFishId(){
		int[] ratCommon = {20,20,20,20};
		FishItemSpec spec = getRandFishId(ratCommon);
		return spec.item_id;
	}
	public static string getRandFishIdByLevel(int command){
		List<FishItemSpec> dic =  getFishDic()[command];
		return dic [Random.Range (0, dic.Count)].item_id;
	}


}

