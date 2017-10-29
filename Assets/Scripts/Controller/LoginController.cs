using UnityEngine;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LoginController : MonoBehaviour {
	public GameObject UI;
	public GameObject cannonCanvas;

	GameObject curCannon;
	AudioSource Music;

	GameObject cannonText;
	GameObject bulletText;
	GameObject taskButton ;
	Text coinText;
	Text gemText;
	Text landText;
	GameObject boxEntity;
	List<GameObject> fishCannonList = new List<GameObject>();
	List<GameObject> petList = new List<GameObject>();


	BulletItemSpec curBulletSpec;
	CannonItemSpec curCannonSpec;
	int cannonLevel = 1;
	int CannonLevelMax = 5;



	// Use this for initialization

	private static LoginController _controller;
	public static LoginController GetInstance(){
		if (!_controller) {
			_controller = (LoginController)GameObject.FindObjectOfType(typeof(LoginController));
		}
		if (!_controller) {
			Debug.LogError (" no logincontroller");
		}
		return _controller;
	}

	//Android
	//"jar:file://" + Application.dataPath
	void Start(){

		UI.gameObject.SetActive (false);
		if (Screen.width >= 800) {
			float t = Mathf.Max ((float)Screen.width / 650, (float)Screen.height / 960);
			Camera.main.orthographicSize = 2.25f + t / 2 * 6f;
		} else {
			cannonCanvas.transform.localScale = new Vector3 (0.01f * Screen.width / 800,0.01f * Screen.width / 800,1f);
		}

		if (!GameController.GetInstance ().login) {
			SpecController.initGameXML ();
			LanController.ReadAndLoadXml ();
			//yield return StartCoroutine (initAssetsBundle ());
		}

	

		if (!GameController.GetInstance ().login) {
			//PlayerPrefs.DeleteAll ();
			loginCommand();
			GameController.GetInstance ().login = true;

			GameController.GetInstance ().InitUnityPurchase ();
		}

		if (PlayerData.getGameuid () == null || PlayerData.getGameuid () == "") {
			creatNewPlayer ();
			GameController.GetInstance ().isNewer = true;
		} 

		Music = gameObject.GetComponent<AudioSource> ();
		Music.volume = PlayerData.getMusic ();

		cannonText = UI.transform.Find ("Cannon").Find("Text").gameObject;
		bulletText = UI.transform.Find ("Bullet").Find("Text").gameObject;
		cannonText.transform.GetComponent<Text> ().text = cannonLevel.ToString ();
		refreshBulletCount ();

		landText = UI.transform.Find ("IslandTitle").Find("IslandText").GetComponent<Text>();
		coinText = UI.transform.Find ("IslandTitle").Find("CoinButton").Find("Text").GetComponent<Text>();
		gemText = UI.transform.Find ("IslandTitle").Find ("GemButton").Find ("Text").GetComponent<Text> ();
		taskButton  = UI.transform.Find ("TaskButton").gameObject;
		GameController.WorldBound = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0));
		init ();


		if (PlayerData.getPets ().Count <= 0) {
			creatPetBox ();
			if(GameController.GetInstance().curGuildStep == null){
				DialogController.GetInstance ().showGuiderPanel (GuilderPanel.Step01, UI.transform, Camera.main.WorldToScreenPoint( new Vector3(0,0,0)));
			}

			GameController.GetInstance ().ContributionValue = 0.2f;
		}else {
			if (!PlayerData.hasTitle (GameController.TITLE01)) {
				DialogController.GetInstance ().showTitlePanel (GameController.TITLE01);
			}
		}


	}

	void creatNewPlayer(){
		PlayerData.setcoin (10000);
		PlayerData.setgem (100);
		PlayerData.setBulletCount (1000);
		PlayerPrefs.SetFloat (PlayerData.Music,0.2f);
		PlayerPrefs.SetFloat (PlayerData.Sound,0.2f);
	}



	void creatPetBox(){
		boxEntity = (GameObject)Instantiate(GameController.GetInstance ().getPrefab ("Box"),new Vector3(0,0,0),Quaternion.identity,cannonCanvas.transform);
	}
	public void removeBox(){
		if(boxEntity != null){
			Destroy (boxEntity);
		}
	}

	void loginCommand(){
		string uid = PlayerData.getUid ();
		string gameuid = PlayerData.getGameuid ();
		WWWForm form = new WWWForm ();
		form.AddField ("command",Command.LoginName);

		AmfObject amf = new AmfObject();
		amf.uid = uid;
	
		Debug.Log ("uid :" +uid);
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
			AmfObject jsonData = JsonMapper.ToObject<AmfObject> (www.text);
			PlayerData.setGameuid (jsonData.gameuid);
			if (jsonData.compensate != null) {
				List<OwnedItem> list = JsonMapper.ToObject<List<OwnedItem>> (jsonData.compensate );
				DialogController.GetInstance ().showRewardsPanel (list,LanController.getString("compensate"));

			}
			GameController.GetInstance ().setSystemTime (jsonData.__end_time);
			GameController.GetInstance().personCheck ();
			checkTask ();
		}
	}

	IEnumerator initAssetsBundle(){
		string manifestName = "StreamingAssets";
		string assetBundleName = "fish";
		string assetBundlePath;
		if (Application.platform == RuntimePlatform.Android) {
			assetBundlePath = "jar:file://" + Application.dataPath + "!/assets/";
		}else if(Application.platform == RuntimePlatform.IPhonePlayer){
			assetBundlePath =  Application.dataPath + "/Raw/";

		}else{
			assetBundlePath = "file://" + Application.dataPath + "/StreamingAssets/";
		}

		WWW www = WWW.LoadFromCacheOrDownload ( assetBundlePath + manifestName, 0);
		yield return www;
		if (www.error == null) {
			AssetBundle mainfestbundle = www.assetBundle;
			AssetBundleManifest mainfest = (AssetBundleManifest)mainfestbundle.LoadAsset ("AssetBundleManifest");
			mainfestbundle.Unload (false);

			//获取依赖关系
			string[] dependentAssetBundles = mainfest.GetAllDependencies (assetBundleName);
			AssetBundle[] abs = new AssetBundle[dependentAssetBundles.Length];
			for (int i = 0; i < dependentAssetBundles.Length; i++) {
				WWW ww = WWW.LoadFromCacheOrDownload (assetBundlePath + dependentAssetBundles [i], 0);
				yield return ww;
				abs [i] = ww.assetBundle;
			}

			//加载文件
			WWW ww2 = WWW.LoadFromCacheOrDownload (assetBundlePath + assetBundleName, 0);
			yield return ww2;
			GameController.GetInstance().FishAssetsBundle = ww2.assetBundle;

		} else {
			Debug.Log (www.error);
		}
	}




	// Update is called once per frame
	int timeCount = 500;
	void Update () {
		//deviceOrientationCheckUp ();
		if (timeCount <=0 ) {
			timeCount = 300;
			GameController.GetInstance().personCheck ();
			checkTask ();

		} else {
			timeCount--;
		}

		if(Input.GetMouseButtonDown(0)){
			if(GameController.GetInstance().movingPetDataId != -1){
				movePetEntity ();
			}
			else if(!EventSystem.current.IsPointerOverGameObject () && EventSystem.current.currentSelectedGameObject == null ){
				Ray curRay = Camera.main.ScreenPointToRay (Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast (new Vector2 (curRay.origin.x, curRay.origin.y), Vector2.down);
				if(hit.collider){
					doMouseDown (hit.collider.gameObject);
				}
			}

		}
	}

	void doMouseDown(GameObject target){
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

	void init(){
		UI.gameObject.SetActive (true);
		initLand ();
		initTreasure ();
		initBullet ();
		initCannon ();
		initPetFish ();
		initShipButton ();
	}

	public void initShipButton(){
		if (PlayerData.getMyShipData () != null) {
			Destroy( UI.transform.Find ("WorldButton").gameObject);
			UI.transform.Find ("ShipButton").gameObject.SetActive (true);
		} 
	}
	public void initBullet(){
		curBulletSpec = SpecController.getItemById (PlayerData.getCurrentBullet ()) as BulletItemSpec;

	}
	void initLand(){
		landText.text = LanController.getString ("newland");
	}
	public void initTreasure(){
		coinText.text = PlayerData.getcoin ().ToString();
		gemText.text = PlayerData.getgem ().ToString();	
	}

	public void initCannon(){
		if(curCannon){
			Destroy (curCannon);
		}

		string cannonId = PlayerData.getCurrentCannon ();
		curCannonSpec = SpecController.getItemById (cannonId) as CannonItemSpec;

		CannonLevelMax = curCannonSpec.maxLevel;
		curCannon = (GameObject)Instantiate(GameController.GetInstance ().getPrefab (curCannonSpec.name),new Vector3(0,0,0),Quaternion.identity,cannonCanvas.transform);
		curCannon.name = "MyCannon";
		curCannon.GetComponent<MyCannon> ().init (curCannonSpec);

		Vector3 v2 = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width/2,  0,0));
		v2.z = GameController.fishCannon_z;
		curCannon.transform.position  = v2; 

	}

	void initPetFish(){
		List<PetData> pets = PlayerData.getPets ();
		foreach (PetData pData in pets) {
			if (pData.currentState == GameController.Pet_Fight) {
				creatFishCannon (pData);
			} else {
				creatNewPet (pData);
			}
		}
	}
	public void refreshPets(){
		List<PetData> pets = PlayerData.getPets (); 
		foreach (PetData pData in pets) {
			foreach(GameObject cannon in fishCannonList){
				if(cannon.name == pData.data_id.ToString()){
					if (pData.currentState == GameController.Pet_Default) {
						fishCannonList.Remove (cannon);
						Destroy (cannon);

						creatNewPet (pData);
						break;
					} 
				}
			}

			foreach(GameObject pet in petList){
				if(pet.name == pData.data_id.ToString()){
					if (pData.currentState == GameController.Pet_Fight) {
						petList.Remove (pet);
						Destroy (pet);
						creatFishCannon (pData);
						break;
					} 
				}
			}

		}
	}

	void creatFishCannon(PetData data){
		GameObject fishCannon = (GameObject)Instantiate(GameController.GetInstance ().getPrefab ("FishCannon"),new Vector3((float)data.posx,(float)data.posy,GameController.fishCannon_z),Quaternion.identity,cannonCanvas.transform);
		fishCannon.GetComponent<FishCannon> ().init (data);
		fishCannonList.Add (fishCannon);
	}
	public void creatNewPet(PetData data){
		GameObject pFish = (GameObject)Instantiate(GameController.GetInstance ().getPrefab (data.fishName),Vector3.zero,Quaternion.identity);
		pFish.AddComponent<Pet> ().init (data);
		pFish.name = data.data_id.ToString ();
		petList.Add (pFish);
	}

	void autofire (){

		Vector3 tarPos = new Vector3(Random.value *4, Random.value *4);
		Vector3 curPos = curCannon.transform.position;
		Vector3 v = (tarPos - curPos).normalized;
		float tar = Mathf.Atan2 (v.y, v.x)  * Mathf.Rad2Deg - 90;
		creatBullet (curPos, v,null);
		curCannon.GetComponent<MyCannon> ().shot (tar);

		AudioController.PlaySound (AudioController.SOUND_GUN);
	}

	void fire (){
		Vector3 tarPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Vector3 curPos = curCannon.transform.Find("Cannon").position;

		tarPos = new Vector3 (tarPos.x, tarPos.y, curPos.z);
		Vector3 v = (tarPos - curPos).normalized;
		float tar = Mathf.Atan2 (v.y, v.x)  * Mathf.Rad2Deg - 90;

		creatBullet (curPos, v,null);
		curCannon.GetComponent<MyCannon> ().shot (tar);

		AudioController.PlaySound (AudioController.SOUND_GUN);

		foreach(GameObject fishCannon in fishCannonList){
			fishCannon.GetComponent<FishCannon> ().fire (tarPos);
		}

	}

	public void creatBullet(Vector3 pos, Vector3 rot,PetData pdata){
		GameObject myBullet = (GameObject)Instantiate(GameController.GetInstance().getPrefab("Bullet"),pos,Quaternion.identity);
		if (pdata == null) {
			myBullet.GetComponent<Bullet> ().initBullet (rot,curBulletSpec, Mathf.Round( curBulletSpec.speed *(curCannonSpec.attackSpeed/100 + 1)),0,0);
		} else {
			myBullet.GetComponent<Bullet> ().initPetBullet (rot, pdata);
		}

	}


	private DeviceOrientation curDeviceOrientation = 0;

	private int count = 0;
	private void deviceOrientationCheckUp(){
		if (Input.deviceOrientation != curDeviceOrientation) {
			curDeviceOrientation = Input.deviceOrientation;
			count++;
			if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight) {
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, 0), 3f );
			}else if(Input.deviceOrientation == DeviceOrientation.FaceUp|| Input.deviceOrientation == DeviceOrientation.FaceDown){
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, 0), 3f );

			} else {
				float tar = Mathf.Atan2 (1, 0)* Mathf.Rad2Deg;
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, 0, tar), 3f );
			}

		}
	}




	public void setMusic(float a){
		Music.volume = a;
	}

	public void onClickFight()
	{
		GameController.GetInstance ().BeginBattle ();
		AudioController.PlaySound (AudioController.SOUND_button);

	}

	public void onClickWorld()
	{
		GameController.GetInstance ().OpenWorldScene();
		AudioController.PlaySound (AudioController.SOUND_button);
	}

	public void onShopClick(){
		DialogController.GetInstance ().showShopPanel ();
		AudioController.PlaySound (AudioController.SOUND_button);
	}
	public void onFishbuttonClick(){
		DialogController.GetInstance ().showFishEggsInfoPanel ();
		AudioController.PlaySound (AudioController.SOUND_button);
	}
	public void onClickSetting()
	{
		DialogController.GetInstance ().showSettingPanel();
		AudioController.PlaySound (AudioController.SOUND_button);

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
	public void refreshBulletCount(){
		bulletText.transform.GetComponent<Text> ().text = PlayerData.getBulletCount().ToString ();
	}

	public void onClickNothingLayer(){
		fire ();
	}

}
