using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class BattleController : MonoBehaviour {

	private string[] SeaNames = {"island01","island02"};

	public GameObject island;
	public GameObject UI;
	public GameObject DialogCanvas;
	public GameObject cannonCanvas;
	float timer = 0;

	GameObject curCannon;
	GameObject CatchedIcon;

	GameObject missionPart;
	GameObject cannonText;
	GameObject taskButton;

	Text bulletText;

	int beginTreasure;
	int fishMaxCount = 100;
	TimeSpan missionTotleTime ;
	DateTime startTime;

	Boolean battleOver;
	GameObject wave;

	AudioSource Music;

	string BattleMode;
	List<GameObject> fishCannonList = new List<GameObject>();
	List<GameObject> fishEntityList = new  List<GameObject> ();
	List<FishData> fishList = new  List<FishData> ();

	BulletItemSpec curBulletSpec;
	CannonItemSpec curCannonSpec;
	int cannonLevel = 1;
	int CannonLevelMax = 5;

	WorldFishData bossData;
	MissionFishData missionData;

	public GameObject EffectLayer;
	public GameObject BattleLayer;


	private static BattleController _controller;
	public static BattleController GetInstance(){
		if (!_controller) {
			_controller = (BattleController)GameObject.FindObjectOfType(typeof(BattleController));
		}
		if (!_controller) {
			Debug.LogError (" no BattleController");
		}
		return _controller;
	}
		
	// Use this for initialization
	void Awake () {
		
		init ();
		initSea ();
		initBullet ();
		initCannon ();

		CatchedIcon = GameController.GetInstance ().getPrefab ("CatchedFish");
		initPet ();
		beginNextWave ();


		if (BattleMode == BattleFormation.MissionMode) {
			missionPart.transform.Find ("Title").GetComponent<Text> ().text = LanController.getString ("MissionFish");
			missionTotleTime = new TimeSpan (0, 0, missionData.wholeTime);
			startTime = System.DateTime.Now;
			missionPart.transform.Find ("TCount").GetComponent<Text> ().text = "/" + fishList.Count.ToString ();
			refreshMissionPart ();
		} else {
			checkTask ();
		}

		beginTreasure = PlayerData.getcoin () + PlayerData.getBulletCount ();

		if(GameController.GetInstance().isNewer && GameController.GetInstance().curGuildStep == GuilderPanel.Step04){
			DialogController.GetInstance().showGuiderPanel(GuilderPanel.Step05,UI.transform, UI.transform.Find ("HomeButton").transform.position);
		}
	}

	// Update is called once per frame
	void Update () {
		if(!battleOver){
			timer += Time.deltaTime;
			if (fishList.Count > 0) {
				foreach (FishData fishdata in fishList) {
					if (fishdata.creatTime <= timer) {
						creatFish (fishdata);
						fishList.Remove (fishdata);
						break;
					}
				}
			} else {
				checkRadit ();
				if (BattleMode == BattleFormation.RandomMode) {
					if (timer >= 120) {
						if (fishEntityList.Count < 1) {
							beginNextWave ();
						} else {
							if (!isOuting) {
								isOuting = true;
								outingFish ();
							}
						}
					} else {
						if (fishEntityList.Count < fishMaxCount) {
							addRandomBattleFishes ();
						}
					}
				} else if(BattleMode == BattleFormation.ArrowMode){
					if (fishEntityList.Count < 1) {
						beginNextWave ();
					}
				}
			}

			if(BattleMode == BattleFormation.MissionMode){
				refreshMissionPart ();
			}

			if(Input.GetMouseButtonDown(0)){
				if (GameController.GetInstance ().movingPetDataId != -1) {
					movePetEntity ();
				}else if (!EventSystem.current.IsPointerOverGameObject ()  && EventSystem.current.currentSelectedGameObject == null) {
					Ray curRay = Camera.main.ScreenPointToRay (Input.mousePosition);
					RaycastHit2D hit = Physics2D.Raycast (new Vector2 (curRay.origin.x, curRay.origin.y), Vector2.down);
					if (hit.collider) {
						doMouseDown ();
					}
				} 
			}
		}

	}

	void doMouseDown(){
			fire ();
	}

	public void onClickNothingLayer(){
		fire ();
	}

	void movePetEntity(){

		foreach(GameObject fishCannon in fishCannonList){
			if(fishCannon.GetComponent<FishCannon>().getDataId() ==  GameController.GetInstance ().movingPetDataId){
				fishCannon.GetComponent<FishCannon> ().move ();
				break;
			}
		}

		GameController.GetInstance ().movingPetDataId = -1;
	}

	void checkRadit(){
		int curTreasure = PlayerData.getcoin () + PlayerData.getBulletCount ();
		float curTrea =Mathf.Min(10000f, Mathf.Max(1000 , curTreasure - beginTreasure))/10000;
		if(curTreasure > 400000){
			GameController.GetInstance().ContributionValue = 0.4f *(1-curTrea) - 0.3f;
		}else if(curTreasure > 100000){
			GameController.GetInstance().ContributionValue = 0.4f *(1-curTrea) - 0.25f;
		}else{
			GameController.GetInstance().ContributionValue = 0.4f *(1-curTrea) - 0.2f;
		}
	}
	void init(){
		missionPart= UI.transform.Find ("MissionPart").gameObject;
		wave = UI.transform.Find ("Wave").gameObject;
		cannonText = UI.transform.Find ("Cannon").Find("Text").gameObject;
		cannonText.GetComponent<Text> ().text = cannonLevel.ToString ();

		taskButton  = UI.transform.Find ("TaskButton").gameObject;
		if(PlayerData.getCurTask() ==null){
			taskButton.SetActive (false);
		}

		bulletText = UI.transform.Find ("Bullet").Find("Text").GetComponent<Text>();
		refreshBulletCount ();

		Music = gameObject.GetComponent<AudioSource> ();
		Music.volume = PlayerData.getMusic ();

		if (Screen.width >= 800) {
			float t = Mathf.Max ((float)Screen.width / 650, (float)Screen.height / 960);
			Camera.main.orthographicSize = 2.25f + t / 2 * 6f;
		} else {
			cannonCanvas.transform.localScale = new Vector3 (0.01f * Screen.width / 800,0.01f * Screen.width / 800,1f);
			BattleLayer.transform.localScale = new Vector3 (Screen.width / 800f,Screen.width / 800f,1f);
		}

		GameController.WorldBound = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0));


		fishMaxCount = (Screen.width / 800 - 1) * 20 + 50;

		bossData = GameController.GetInstance ().bossData;
		if(bossData != null){
			BattleMode = BattleFormation.BossMode;
			creatBossCannon (bossData);
			taskButton.SetActive (false);
		}else if(GameController.GetInstance ().missionFishData != null){
			missionData = GameController.GetInstance ().missionFishData;
			BattleMode = BattleFormation.MissionMode;
			taskButton.SetActive (false);
			missionPart.SetActive (true);
		}
	}

	void initSea(){
		SpriteRenderer spr = island.GetComponent<SpriteRenderer> ();
		string seaName = SeaNames[UnityEngine.Random.Range (0,SeaNames.Length)];

		Texture2D seaTexture = (Texture2D)GameController.GetInstance().getSpByName ("Pic/island/" + seaName).texture ;
		spr.sprite = Sprite.Create (seaTexture,spr.sprite.textureRect, new Vector2 (0.5f,0.5f));

	}
	public void initCannon(){
		if(curCannon){
			Destroy (curCannon);
		}

		curCannonSpec = SpecController.getItemById (PlayerData.getCurrentCannon()) as CannonItemSpec;
		CannonLevelMax = curCannonSpec.maxLevel;
		curCannon = (GameObject)Instantiate(GameController.GetInstance ().getPrefab (curCannonSpec.name),new Vector3(0,0,0),Quaternion.identity,cannonCanvas.transform);
		curCannon.name = "MyCannon";
		curCannon.GetComponent<MyCannon> ().init (curCannonSpec);
		Vector3 v2 = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width/2,  0,0));
		v2.z = GameController.fishCannon_z;
		curCannon.transform.position  = v2; 
	}

	public void initBullet(){
		curBulletSpec = SpecController.getItemById (PlayerData.getCurrentBullet()) as BulletItemSpec;
	}


	private void initPet(){
		List<PetData> pets = PlayerData.getPets ();
		foreach (PetData pData in pets) {
			if (pData.currentState == GameController.Pet_Fight) {
				GameObject fishCannon = (GameObject)Instantiate(GameController.GetInstance ().getPrefab ("FishCannon"),new Vector3((float)pData.posx,(float)pData.posy,GameController.fishCannon_z),Quaternion.identity,cannonCanvas.transform);
				fishCannon.GetComponent<FishCannon> ().init (pData);
				fishCannonList.Add (fishCannon);
			} 
		}
	}
	//creat battle
	void addRandomBattleFishes(){
		fishList = BattleFormation.getFishList (BattleMode);
	}
		
	bool isOuting = false;
	void beginNextWave(){
		timer = 0;
		string bgmName = "bgm_1";
		isOuting = false;

		if (BattleMode == BattleFormation.RandomMode) {
			BattleMode = BattleFormation.ArrowMode;
			bgmName = "bgm_4";
		}else if(BattleMode == BattleFormation.BossMode){
			
		}else if(BattleMode == BattleFormation.MissionMode){
			
		} else {
			BattleMode = BattleFormation.RandomMode;
		}
		wave.SetActive (true);

		fishList = BattleFormation.getFishList (BattleMode);

		Camera.main.GetComponent<AudioSource> ().clip = AudioController.getClip (bgmName);
		Camera.main.GetComponent<AudioSource> ().Play ();

		AudioController.PlaySound (AudioController.SOUND_BATTLER);
	}

	void outingFish(){
		foreach (GameObject fishEntity in fishEntityList) {
			fishEntity.transform.GetComponent<Fish> ().outing ();
		}
	}

	public void fire (){
		int bulletcount = PlayerData.getBulletCount ();
		if(bulletcount <= 0){
			//goumai
			DialogController.GetInstance().showShopPanel(ShopPanel.BulletItemTab);
			DialogController.GetInstance().showMessagePanel (LanController.getString("BulletNotEnough"));
		}
		else {
			if (cannonLevel > bulletcount) {
				cannonLevel = bulletcount;
				cannonText.GetComponent<Text> ().text = cannonLevel.ToString();
			}
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 curPos = curCannon.transform.Find("Cannon").position;
			mousePos = new Vector3 (mousePos.x, mousePos.y, curPos.z);
			Vector3 v = (mousePos - curPos).normalized;
		

			float tar = Mathf.Atan2 (v.y, v.x)  * Mathf.Rad2Deg - 90;

			creatBullet (curPos,v,null);
			curCannon.GetComponent<MyCannon> ().shot (tar);

			int otherBullets = bulletcount - cannonLevel;
			PlayerData.setBulletCount (otherBullets);
			refreshBulletCount();

			AudioController.PlaySound (AudioController.SOUND_GUN);

			foreach(GameObject fishCannon in fishCannonList){
				fishCannon.GetComponent<FishCannon> ().fire (mousePos);

			}
		}
	}

	public void refreshBulletCount(){
		bulletText.text  = PlayerData.getBulletCount ().ToString();
	}

	void refreshMissionPart(){
		if ((System.DateTime.Now -  startTime).Seconds >= 0) {
			TimeSpan span = missionTotleTime - (System.DateTime.Now - startTime);
			if (span.TotalSeconds >= 0) {

				missionPart.transform.Find ("Time").GetComponent<Text> ().text = new TimeSpan(0,0,(int)span.TotalSeconds).ToString ();
				missionPart.transform.Find ("Count").GetComponent<Text> ().text = fishEntityList.Count.ToString ();

				if (fishEntityList.Count <= 0 && fishList.Count <= 0) {
					//success
					battleOver = true;
					finishMission ();
				} 
			} else {
				//timeout
				battleOver = true;
				DialogController.GetInstance ().showMessagePanel (LanController.getString ("MissionTimeOut"), true,delegate {
					GameController.GetInstance().OpenWorldScene();
				},delegate {
					GameController.GetInstance().OpenWorldScene();
				});
				endFishing ();
			}
		}
	}

	void finishMission(){
		List<OwnedItem> rewards = new List<OwnedItem> ();

		if(missionData.fishLevel ==3){
			OwnedItem item0 = new OwnedItem ();
			item0.item_id = "gem";
			item0.count = 2;
			rewards.Add (item0);
		}

		OwnedItem item = new OwnedItem ();
		item.item_id = "coin";
		item.count = getCatchFishCoin ()*2;
		rewards.Add (item);

		OwnedItem item1 = new OwnedItem ();
		item1.item_id = "exp";
		item1.count = getPetCatchExp ();
		rewards.Add (item1);

		OwnedItem item2 = new OwnedItem ();
		item2.item_id = "bullet";
		item2.count = missionData.fishLevel * 500;
		rewards.Add (item2);

		MissionFishData newMissionData = GameController.GetInstance ().creatMissionFish (missionData.fishLevel ,Mathf.Max( missionData.getEndTime(), GameController.GetInstance().getCurrentSystemNum()));
		HomeObject curObject = PlayerData.getHomeObject (PlayerData.getGameuid());
		foreach (MissionFishData mData in  curObject.missionFishes) {
			if(mData.item_id == missionData.item_id){
				curObject.missionFishes.Remove (mData);
				break;
			}
		}
		curObject.missionFishes.Add (newMissionData);
		PlayerData.updateHomeObject (curObject);

		DialogController.GetInstance ().showRewardsPanel (rewards,LanController.getString("MissionRewards"),delegate {
			GameController.GetInstance ().OpenWorldScene ();
		});



	}
	void creatBossCannon(WorldFishData data){
		GameObject fishCannon = (GameObject)Instantiate(GameController.GetInstance ().getPrefab ("BossCannon"),getBossPosition(),Quaternion.identity,cannonCanvas.transform);
		fishCannon.GetComponent<BossFish> ().init (data);
	}



	Vector3 getBossPosition(){
		return new Vector3 (0,Camera.main.orthographicSize*0.7f,GameController.fishCannon_z);
	}

	public void creatFish(FishData data)
	{
		GameObject fishObj = GameController.GetInstance().getPrefab(data.fishName);
		GameObject curfish = (GameObject)Instantiate(fishObj,data.position,Quaternion.identity,BattleLayer.transform);
		curfish.AddComponent<Fish> ().init (data,BattleMode);
		fishEntityList.Add (curfish);
	}

	public void removeFish(GameObject fish){
		if(fishEntityList.Contains(fish)){
			fishEntityList.Remove (fish);
		}
	}
	public void creatBullet(Vector3 pos, Vector3 rot,PetData pdata){
		GameObject myBullet = (GameObject)Instantiate(GameController.GetInstance().getPrefab("Bullet"),pos,Quaternion.identity,BattleLayer.transform);
	
		if (pdata == null) {
			myBullet.name = "Bullet";
			myBullet.GetComponent<Bullet> ().initBullet (rot,curBulletSpec,getCannonBulletSpeed(),getBulletAttack(),cannonLevel);
		} else {
			myBullet.name = "PetBullet";
			myBullet.GetComponent<Bullet> ().initPetBullet (rot, pdata);
		}
	}

	public void creatBossBullet(Vector3 pos, Vector3 rot,WorldFishData bdata){
		GameObject myBullet = (GameObject)Instantiate(GameController.GetInstance().getPrefab("BossBullet"),pos,Quaternion.identity,BattleLayer.transform);

		if (bdata != null) {
			myBullet.name = "BossBullet";
			myBullet.AddComponent<BossBullet> ().initBossBullet (rot, bdata);
		}
	}

	float getCannonBulletSpeed(){
		return Mathf.Round( curBulletSpec.speed *(curCannonSpec.attackSpeed/100 + 1));
	}

	public void creatNet(string netName,float attack,float attackradit,Vector3 pos){
		GameObject myNet = (GameObject)Instantiate (GameController.GetInstance ().getPrefab (netName), pos, Quaternion.identity,BattleLayer.transform);
		myNet.GetComponent<Net> ().initNet (netName, attack, attackradit);
	}

	public void creatBulletEffect(Vector3 pos){
		GameObject myNet = (GameObject)Instantiate (GameController.GetInstance ().getPrefab ("BulletEffect"), pos, Quaternion.identity,BattleLayer.transform);
		Destroy (myNet, 0.05f);
	}

	float getBulletAttack(){
		return curBulletSpec.attack*(1+curCannonSpec.attack/100);
	}


	public Dictionary<string,int> catchFishes = new Dictionary<string, int> ();
	Dictionary<string,int> petCatchFishes = new Dictionary<string, int> ();

	public void catchFish(FishData fishData,Vector3 posVec,PetData pdata){


	
		if (catchFishes.ContainsKey (fishData.item_id)) {
			catchFishes [fishData.item_id] = catchFishes [fishData.item_id] + 1;
		} else {
			catchFishes.Add (fishData.item_id, 1);
		}

		TaskData taskdata = PlayerData.getCurTask ();
		if(taskdata!= null){
			taskdata.addItem (fishData.item_id, 1);
		}


		if (pdata != null) {
			if (petCatchFishes.ContainsKey (fishData.item_id)) {
				petCatchFishes [fishData.item_id] = petCatchFishes [fishData.item_id] + 1;
			} else {
				petCatchFishes.Add (fishData.item_id, 1);
			}

			AddHeartTip(fishData.heart,posVec,Vector3.zero,pdata.fishName);

		//	foreach(GameObject fishCannon in fishCannonList){
			//	if(fishCannon.name == pdata.data_id.ToString()){
				//	AddHeartTip(fishData.heart,posVec,fishCannon.transform.position);
			//		break;
			//	}
			//}
		} else {
			AddHeartTip(fishData.heart,posVec,Vector3.zero);
		}

		AudioController.PlaySound (AudioController.SOUND_coin,0,0.2f);
	}

	public void creatFishHole(Vector3 pos,string fishId, float totalTime){
		GameObject fishHole = (GameObject)Instantiate(GameController.GetInstance ().getPrefab ("FishHole"),pos,Quaternion.identity,BattleLayer.transform);
		fishHole.GetComponent<FishHole> ().init (fishId,200f);
	}



	public void AddHeartTip(int addNum,Vector3 pos,Vector3 targetPos,string cannonName = null){
		GameObject tip = (GameObject)Instantiate (GameController.GetInstance().getPrefab("HeartTip"),Camera.main.WorldToScreenPoint( pos), Quaternion.identity, EffectLayer.transform);
		tip.transform.Find ("Number").GetComponent<Text> ().text = "+" + addNum;
		if(cannonName!= null){
			tip.GetComponent<Image> ().sprite = GameController.GetInstance ().getSpByName ("Pic/ui/" +  cannonName + "Icon");
		}

		tip.GetComponent<HeartTip> ().init (targetPos);
	}
		
	public GameObject getCurBossAttackTarget(){
		foreach(GameObject fishCannon in fishCannonList){
			if(fishCannon.GetComponent<FishCannon>().isActive()){
				return fishCannon;
			}
		}
		return curCannon;
	}



	public void onClickHome(){
		DialogController.GetInstance().showBattleResult ();
		AudioController.PlaySound (AudioController.SOUND_button);
	}

	public void onClickCannonAdd()
	{
		cannonLevel++;
		if(cannonLevel >CannonLevelMax){
			cannonLevel = 1;
		}
		cannonText.GetComponent<Text> ().text = cannonLevel.ToString();
		AudioController.PlaySound (AudioController.SOUND_button);
	}
	public void onClickCannonSuntract()
	{
		cannonLevel--;
		if(cannonLevel < 1){
			cannonLevel = CannonLevelMax;
		}
		cannonText.GetComponent<Text> ().text = cannonLevel.ToString();
		AudioController.PlaySound (AudioController.SOUND_button);
	}
		
	public int getPetCatchExp(){
		int rewardExp = 0;
		ItemSpec spec;
		foreach (string key in petCatchFishes.Keys) {
			int count = petCatchFishes [key];
			spec = SpecController.getItemById (key);

			rewardExp += spec.common * count * 3;
		}
		foreach (string key in catchFishes.Keys) {
			int count = catchFishes [key];
			spec = SpecController.getItemById (key);
			rewardExp += spec.common * count  ;
		}
		return   rewardExp;
	}

	int getCatchFishCoin(){
		int rewardCoin = 0;
		ItemSpec spec;
		foreach (string key in petCatchFishes.Keys) {
			int count = petCatchFishes [key];
			spec = SpecController.getItemById (key);

			rewardCoin += spec.coin * count ;
		}
		foreach (string key in catchFishes.Keys) {
			int count = catchFishes [key];
			spec = SpecController.getItemById (key);
			rewardCoin += spec.coin * count  ;
		}
		return   rewardCoin;
	}

	public void onClickTask(){
		TaskData data = PlayerData.getCurTask ();
		if (data != null) {
			DialogController.GetInstance ().showTaskPanel (data);
		} else {
			taskButton.SetActive (false);
		}
	}

	void checkTask(){
		if (PlayerData.getCurTask () != null) {
			taskButton.SetActive (true);
		} else {
			taskButton.SetActive (false);
		}
	}

	public void endFishing(){
		GameController.GetInstance ().bossData = null;
		GameController.GetInstance ().missionFishData = null;
	}
}
