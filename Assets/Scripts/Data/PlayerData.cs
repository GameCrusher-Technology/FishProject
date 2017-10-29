using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LitJson;

public class PlayerData  {
	public static string Music = "music";
	public static string Sound = "sound";

	public static string getGameuid(){
		return PlayerPrefs.GetString ("gameuid");
	}
	public static void setGameuid(string gameuid){
		PlayerPrefs.SetString ("gameuid",gameuid);
	}

	public static string getTitle(){
		return PlayerPrefs.GetString ("gameTitle");
	}

	public static void setTitle(string title){
		PlayerPrefs.SetString ("gameTitle",title);
	}

	public static int getFishExp(){
		return PlayerPrefs.GetInt ("FishExp");
	}

	public static void setFishExp(int FishExp){
		PlayerPrefs.SetInt ("FishExp",FishExp);
	}

	public static bool hasTitle(string title){
		List<string> titles = getTitles ();
		return titles.Contains (title);
	}


	public static void addTitle(string title){
		List<string> titles = getTitles ();
		if(!titles.Contains(title)){
			titles.Add (title);
		}

		PlayerPrefs.SetString ("gameTitles", JsonMapper.ToJson(titles));
		PlayerPrefs.SetString ("gameTitle",title);
	}

	public static List<string> getTitles(){
		List<string> titles = new List<string>();
		string str = PlayerPrefs.GetString ("gameTitles");
		if (str != "") {
			titles = JsonMapper.ToObject<List<string>> (str);
		}

		return titles;
	}

	public static string getUid(){
		//return "Fishuid_"+ System.DateTime.Now.ToBinary();
		string uid =  PlayerPrefs.GetString ("uid");
		if (uid == null || uid == "") {
			uid =  SystemInfo.deviceUniqueIdentifier;
			if(uid == null  || uid == ""){
				uid = "FishGameuid_"+ System.DateTime.Now;
			}
			PlayerPrefs.SetString ("uid",uid);

		} 
		return uid;
	}

	public static string getPlayerName(){
		return PlayerPrefs.GetString ("PlayerName");
	}
	public static void setPlayerName(string name){
		PlayerPrefs.SetString ("PlayerName",name);
	}

	public static int getcoin(){
		return PlayerPrefs.GetInt ("coin");
	}
	public static void setcoin(int c){
		PlayerPrefs.SetInt ("coin",c);
	}
	public static int getgem(){
		return PlayerPrefs.GetInt ("gem");
	}
	public static void setgem(int g){
		PlayerPrefs.SetInt ("gem",g);
	}

	public static int getBulletCount(){
		return PlayerPrefs.GetInt ("BulletCount");
	}
	public static void setBulletCount(int g){
		PlayerPrefs.SetInt ("BulletCount",g);
	}
	public static void addBulletCount(int add){
		PlayerPrefs.SetInt ("BulletCount",getBulletCount() + add);
	}

	public static void updatePetData(PetData data){
		List<PetData> pets = getPets ();
		for(int i = 0 ;i< pets.Count;i++){
			if(pets[i].data_id == data.data_id){
				pets [i] = data;
				break;
			}
		}
		PlayerPrefs.SetString ("Pets", JsonMapper.ToJson(pets));
	}

	public static List<PetData> getPets(){
		List<PetData> pets = new List<PetData>();
		string str = PlayerPrefs.GetString ("Pets");
		if (str != "") {
			pets = JsonMapper.ToObject<List<PetData>> (str);
		}
		return pets;
	}

	public static int creatPetId(){
		List<PetData> pets = getPets ();
		int maxId = 0;
		foreach(PetData data in pets){
			maxId = Mathf.Max (maxId,data.data_id);
		}
		return (maxId + 1);
	}

	public static void addPet(PetData data){
		List<PetData> pets = getPets ();
		pets.Add (data);
		PlayerPrefs.SetString ("Pets", JsonMapper.ToJson(pets));
	}

	public static void refreshPets(List<PetData> list){
		
		PlayerPrefs.SetString ("Pets", JsonMapper.ToJson(list));
	}

	public static PetData getPet(int data_id){
		List<PetData> pets = getPets ();
		foreach (PetData data in pets) {
			if(data.data_id == data_id){
				return data;
			}
		}
		return null;
	}


	public static int getPetMaxCount(){
		int c =  PlayerPrefs.GetInt ("PetMaxCount");
		if(c< GameController.minPetCount){
			setPetMaxCount (GameController.minPetCount);
			return GameController.minPetCount;
		}
		return c;
	}
	public static void setPetMaxCount(int count){
		PlayerPrefs.SetInt ("PetMaxCount",count);
	}
	public static int getPetCannonMaxCount(){
		int c =  PlayerPrefs.GetInt ("PetCannonMaxCount");
		if(c< GameController.minPetCannonCount){
			setPetCannonMaxCount (GameController.minPetCannonCount);
			return GameController.minPetCannonCount;
		}
		return c;

	}
	public static void setPetCannonMaxCount(int count){
		PlayerPrefs.SetInt ("PetCannonMaxCount",count);
	}

	//petegg

	public static int getPetEggMaxCount(){
		int c =  PlayerPrefs.GetInt ("PetEggMaxCount");
		if(c< GameController.minPetEggCount){
			setPetEggMaxCount (GameController.minPetEggCount);
			return GameController.minPetEggCount;
		}
		return c;
	}
	public static void setPetEggMaxCount(int count){
		PlayerPrefs.SetInt ("PetEggMaxCount",count);
	}

	public static List<PetEggData> getPetEggs(){
		List<PetEggData > pets = new List<PetEggData>();
		string str = PlayerPrefs.GetString ("PetEggs");
		if (str != "") {
			pets = JsonMapper.ToObject<List<PetEggData>> (str);
		}
		return pets;
	}
	public static void addPetEgg(PetEggData data){
		List<PetEggData> pets = getPetEggs ();
		pets.Add (data);
		PlayerPrefs.SetString ("PetEggs", JsonMapper.ToJson(pets));
	}

	public static void deletePetEgg(PetEggData data){
		List<PetEggData> pets = getPetEggs ();
		foreach(PetEggData pet in pets){
			if(pet.data_id == data.data_id){
				pets.Remove (pet);
				break;
			}
		}
		PlayerPrefs.SetString ("PetEggs", JsonMapper.ToJson(pets));
	}

	public static void updatePetEgg(PetEggData data){
		List<PetEggData> pets = getPetEggs ();

		for(int i = 0 ;i< pets.Count;i++){
			if(pets[i].data_id == data.data_id){
				pets [i] = data;
				break;
			}
		}


		PlayerPrefs.SetString ("PetEggs", JsonMapper.ToJson(pets));
	}




	//owneditem
	public static List<OwnedItem> getOwnedItems(){
		List<OwnedItem> items = new List<OwnedItem>();
		string str = PlayerPrefs.GetString ("OwnedItem");
		if (str != "") {
			items = JsonMapper.ToObject<List<OwnedItem>> (str);
		}
		return items;
	}

	public static OwnedItem getOwnedItem(string id){
		List<OwnedItem> items = getOwnedItems();
		OwnedItem oItem =new OwnedItem();
		foreach(OwnedItem item in items){
			if (item.item_id == id) {
				oItem = item;
				break;
			}
		}
		if(oItem.item_id == null){
			oItem.item_id = id;
			oItem.count = 0;
		}
		return oItem;
	}

	public static void addOwnedItem(string id,int count){
		List<OwnedItem> items = getOwnedItems();
		OwnedItem newitem = null;
		foreach(OwnedItem item in items){
			if (item.item_id == id) {
				newitem = item;
				break;
			}
		}
		if (newitem != null) {
			newitem.count += count;
		} else {
			newitem = new OwnedItem ();
			newitem.item_id = id;
			newitem.count = count;
			items.Add (newitem);
		}


		PlayerPrefs.SetString ("OwnedItem", JsonMapper.ToJson(items));
	}


	public static float getMusic(){
		return PlayerPrefs.GetFloat (Music);
	}
	public static float getSound(){
		return PlayerPrefs.GetFloat (Sound);
	}	

	public static List<string> getBullets(){
		List<string> bullets = new List<string> ();
		string str = PlayerPrefs.GetString ("Bullets");
		if (str != "") {
			bullets = JsonMapper.ToObject<List<string>> (str);
		} else {
			bullets.Add (GameController.initBulletId);
			PlayerPrefs.SetString ("Bullets", JsonMapper.ToJson(bullets));
		}
		return bullets;
	}
	public static string getCurrentBullet(){
		string curCannon = PlayerPrefs.GetString ("curBullet");
		if (curCannon == "") {
			PlayerPrefs.SetString ("curBullet",GameController.initBulletId);
			curCannon = GameController.initBulletId;
		} 
		return curCannon;
	}
	public static void addBullet(string bulletid){
		List<string> bullets = getBullets();
		bullets.Add (bulletid);
		PlayerPrefs.SetString ("Bullets", JsonMapper.ToJson(bullets));
	}
	public static void setCurBullet(string bulletid){
		List<string> bullets = getBullets();
		if (bullets.Contains(bulletid)) {
			PlayerPrefs.SetString ("curBullet", bulletid);
		} else {
			Debug.Log ("has no cur bullet");
		}
	}


	public static string getCurrentCannon(){
		string curCannon = PlayerPrefs.GetString ("curCannon");
		if (curCannon == "") {
			PlayerPrefs.SetString ("curCannon",GameController.initCannonId);
			curCannon =  GameController.initCannonId;
		} 
		return curCannon;
	}
	public static List<string> getCannons(){
		List<string> cannons = new List<string> ();
		string str = PlayerPrefs.GetString ("Cannons");
		if (str != "") {
			cannons = JsonMapper.ToObject<List<string>> (str);
		} else {
			cannons.Add (GameController.initCannonId);
			PlayerPrefs.SetString ("Cannons", JsonMapper.ToJson(cannons));
		}
		return cannons;
	}

	public static void addCannon(string cannonid){
		List<string> cannons = getCannons();
		cannons.Add (cannonid);
		PlayerPrefs.SetString ("Cannons", JsonMapper.ToJson(cannons));
	}

	public static void setCurCannon(string cannonid){
		List<string> cannons = getCannons();
		if (cannons.Contains(cannonid)) {
			PlayerPrefs.SetString ("curCannon", cannonid);
		} else {
			Debug.Log ("has no cur cannon");
		}
	}


	//task
	private static TaskData _curTaskData;
	public static TaskData  getCurTask(){
		if(_curTaskData == null){
			string curtaskstr = PlayerPrefs.GetString ("curTask");
			if (curtaskstr != "") {
				_curTaskData =	JsonMapper.ToObject<TaskData> (curtaskstr);
			} else {
				_curTaskData = null;
			}
		}
		return _curTaskData;
	}

	public static void  setCurTask(TaskData data){
		_curTaskData = data;
		if (data != null) {
			PlayerPrefs.SetString ("curTask", JsonMapper.ToJson(data));
		} else {
			PlayerPrefs.SetString ("curTask", "");
		}
	}


	//homeobjectdata

	public static HomeObject getHomeObject(string gameuid){
		List<HomeObject> datas = getHomeObjects ();
		foreach (HomeObject hObj in datas) {
			if(hObj.gameUid == gameuid){
				return hObj;
			}
		}
		return null;
	}

	public static List<HomeObject> getHomeObjects(){
		List<HomeObject> datas = new List<HomeObject>();
		string HomeDataStr = PlayerPrefs.GetString ("HomeDatas");
		if (HomeDataStr != "") {
			datas =	JsonMapper.ToObject<List<HomeObject>> (HomeDataStr);
		}

		return datas;
	}

	public static void updateHomeObject(HomeObject h){
		List<HomeObject> datas = getHomeObjects ();
		foreach (HomeObject hObj in datas) {
			if(hObj.gameUid == h.gameUid){
				datas.Remove (hObj);
				break;
			}
		}
		datas.Add (h);
		PlayerPrefs.SetString ("HomeDatas", JsonMapper.ToJson(datas));
	}

	//ship
	private static ShipData MyShipData;
	public static ShipData getMyShipData(){
		if(MyShipData == null ){
			string curShip = PlayerPrefs.GetString ("MyShip");
			if (curShip != "") {
				MyShipData =	JsonMapper.ToObject<ShipData> (curShip);
			}
		}
		return MyShipData;
	}

	public static void updateMyShipData(ShipData data){
		MyShipData = data;
		PlayerPrefs.SetString ("MyShip", JsonMapper.ToJson(MyShipData));
	}

	//world boss

	public static List<PlayerBossData> getWorldBossPlayerData(){
		List<PlayerBossData> list = new List<PlayerBossData> ();
		string playerBossData = PlayerPrefs.GetString ("MyWorldBossData");
		if(playerBossData != ""){
			list =	JsonMapper.ToObject<List<PlayerBossData>> (playerBossData);
		}
		return list;
	}


}
