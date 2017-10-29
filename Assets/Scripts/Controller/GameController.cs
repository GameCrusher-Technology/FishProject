using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using LitJson;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.EventSystems;

public class GameController :IStoreListener{
	private static GameController _controller;
	public static GameController GetInstance(){
		if (_controller == null) {
			_controller = new GameController ();
		}
		return _controller;
	}

	public bool isNewer = false;
	public string curGuildStep;
	public bool login = false;
	public string CurrentScene = "LoginScene";
	public static string LOGINSCENE = "LoginScene";
	public static string BATTLESCENE = "BattleScene";
	public static string WORLDSCENE = "WorldScene";
	//PET 
	public static int Pet_Default = 0;
	public static int Pet_Fight = 1;

	//layer z
	public static float fishCannon_z = -0.1f;

	//time
	private TimeSpan timeDelay = new TimeSpan(0) ;
	public void setSystemTime(double newTime){

		DateTime dTime = TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (1970, 1, 1));
		long ltime = long.Parse (newTime + "0000000");
		TimeSpan toNow = new TimeSpan (ltime);
		DateTime curTime = dTime.Add (toNow);

		timeDelay = curTime - System.DateTime.Now;
	}

	public DateTime getSystemTime(){
		return  System.DateTime.Now - timeDelay;
	}
	public int getCurrentSystemNum(){
		DateTime dTime = TimeZone.CurrentTimeZone.ToLocalTime (new DateTime (1970, 1, 1));
		TimeSpan s = System.DateTime.Now - dTime;
		return  (int)(System.DateTime.Now - dTime).TotalSeconds;
	}

	//moving Pet
	public int movingPetDataId = -1;
	public const int minPetCount = 5;
	public const int maxPetCount = 20;

	public const int minPetEggCount = 3;
	public const int maxPetEggCount = 20;

	public const int minPetCannonCount = 1;
	public const int maxPetCannonCount = 5;

	public static Vector2 WorldBound = new Vector2() ;

	//
	public const string initPetId = "10004";
	public const string initCannonId = "50001";
	public const string initBulletId = "30000";


	public const string littleCoins =    "fish.littlecoins";   
	public const string largeCoins =    "fish.largecoins";   
	public const string littleGems =    "fish.littlegems";   
	public const string largeGems =    "fish.largegems";   
	//next battle

	public float ContributionValue = 0.1f;

	public WorldFishData bossData;
	public MissionFishData missionFishData;
	public void BeginBattle(){
		CurrentScene = BATTLESCENE;
		SceneManager.LoadScene("BattleScene");
	}

	public void OpenWorldScene(){
		CurrentScene = WORLDSCENE;
		SceneManager.LoadScene("WorldScene");
	}

	public void HomeScene(){
		CurrentScene = LOGINSCENE;
		SceneManager.LoadScene("LoginScene");
	}
	//
	public AssetBundle FishAssetsBundle;
	Dictionary<string,GameObject> prefabs = new Dictionary<string, GameObject> ();

	public GameObject  getPrefab(string name){
		GameObject fab;
		if (prefabs.ContainsKey (name)) {
			fab = prefabs [name];
		}
		//else if (bundle != null) {
		//	AssetBundleRequest request = bundle.LoadAssetAsync (name);
		//	fab = (GameObject)request.asset;
		//	prefabs.Add (name, fab);
		//} 
		else {
			fab = (GameObject)Resources.Load ("Prefabs/" + name);
			prefabs.Add (name, fab);
		}
		return fab;
	}

	Dictionary<string,Sprite> spriteDic = new Dictionary<string, Sprite> ();
	public Sprite getSpByName(string url){
		if (spriteDic.ContainsKey (url)) {
			return spriteDic [url];
		} else {
			Sprite iconsp = new Sprite ();
			iconsp = Resources.Load (url,iconsp.GetType()) as Sprite;
			spriteDic.Add (url, iconsp);
			return iconsp;
		}

	}

	private static IStoreController m_StoreController;
	private UnityEngine.Purchasing.Security.CrossPlatformValidator validator; 
	private static Dictionary<string,string> treasures = new Dictionary<string, string>();
	public Dictionary<string,TreasureData> treasureDatas = new Dictionary<string, TreasureData>();
	public void InitUnityPurchase() { 
		var  module = StandardPurchasingModule.Instance();  
		var builder = ConfigurationBuilder.Instance (module);  

		//添加计费点  
		// UnityEngine.Purchasing.ProductType  
		builder.AddProduct(littleCoins, ProductType.Consumable);
		builder.AddProduct(largeCoins, ProductType.Consumable);
		builder.AddProduct(littleGems, ProductType.Consumable);
		builder.AddProduct(largeGems, ProductType.Consumable);

		treasures.Add ("40000",GameController.littleCoins);
		treasures.Add ("40001",GameController.largeCoins);
		treasures.Add ("40002",GameController.littleGems);
		treasures.Add ("40003",GameController.largeGems);


		#if RECEIPT_VALIDATION
		string appIdentifier;
		#if UNITY_5_6_OR_NEWER
		appIdentifier = Application.identifier;
		#else
		appIdentifier = Application.bundleIdentifier;
		#endif
		validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), appIdentifier);
		#endif


		// Now we're ready to initialize Unity IAP.
		UnityPurchasing.Initialize(this, builder);

	}  

	public void OnInitialized (IStoreController controller, IExtensionProvider extensions) {  
		m_StoreController = controller;  

		// On Apple platforms we need to handle deferred purchases caused by Apple's Ask to Buy feature.  
		// On non-Apple platforms this will have no effect; OnDeferred will never be called.  
		var m_AppleExtensions = extensions.GetExtension<IAppleExtensions> ();  
		m_AppleExtensions.RegisterPurchaseDeferredListener(OnDeferred); 
		foreach(string id in treasures.Keys){
			var product = m_StoreController.products.WithID(treasures[id]);  
			//价格 (带货币单位的字符串)  
			string priceString = product.metadata.localizedPriceString;  
			//价格 （换算汇率后的价格）  
			decimal price = product.metadata.localizedPrice;  

			if(treasureDatas.ContainsKey(id)){
				treasureDatas.Remove (id);
			}
			treasureDatas.Add (id,new TreasureData(id,price,priceString));
		}


	}  

	public bool IsPaymentInitialized()
	{
		return m_StoreController != null;
	}

	public void BuyConsumable(string id)
	{
		BuyProductID(treasures[id]);
	}


	void BuyProductID(string productId)
	{
		if (IsPaymentInitialized())
		{
			Product product = m_StoreController.products.WithID(productId);

			// If the look up found a product for this device's store and that product is ready to be sold ... 
			if (product != null && product.availableToPurchase)
			{
				Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
				m_StoreController.InitiatePurchase(product);
			}
			// Otherwise ...
			else
			{
				// ... report the product look-up failure situation  
				Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				DialogController.GetInstance ().showMessagePanel (LanController.getString("paymentError03"));
			}
		}
		// Otherwise ...
		else
		{
			// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
			// retrying initiailization.
			Debug.Log("BuyProductID FAIL. Not initialized.");
			DialogController.GetInstance ().showMessagePanel (LanController.getString("paymentError01"));
			InitUnityPurchase ();
		}
	}


	public void OnInitializeFailed(InitializationFailureReason error)
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.

		Debug.Log("Billing failed to initialize!");  
		switch (error) {  
		case InitializationFailureReason.AppNotKnown:  
			Debug.LogError("Is your App correctly uploaded on the relevant publisher console?");  
			break;  
		case InitializationFailureReason.PurchasingUnavailable:  
			// Ask the user if billing is disabled in device settings.  
			Debug.Log("Billing disabled!");  
			break;  
		case InitializationFailureReason.NoProductsAvailable:  
			// Developer configuration error; check product metadata.  
			Debug.Log("No products available for purchase!");  
			break;  
		}  

	}
	public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs e) {  
		try {  
			var result = validator.Validate (e.purchasedProduct.receipt);  
			Debug.Log ("Receipt is valid. Contents:");  
			foreach (IPurchaseReceipt productReceipt in result) {  
				Debug.Log(productReceipt.productID);  
				Debug.Log(productReceipt.purchaseDate);  
				Debug.Log(productReceipt.transactionID);  

				AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;  
				if (null != apple) {  
					Debug.Log(apple.originalTransactionIdentifier);  
					Debug.Log(apple.subscriptionExpirationDate);  
					Debug.Log(apple.cancellationDate);  
					Debug.Log(apple.quantity);  


					//如果有服务器，服务器用这个receipt去苹果验证。  
				//	var receiptJson = JSONObject.Parse(e.purchasedProduct.receipt);  
				//	var receipt = receiptJson.GetString("Payload");  

				}  
				GooglePlayReceipt google = productReceipt as GooglePlayReceipt;  
				if (null != google) {
					Debug.Log(google.purchaseState);  
					Debug.Log(google.purchaseToken);  
				}
				PlatFormController.GetInstance().buyCommand (productReceipt.productID,productReceipt.transactionID,e.purchasedProduct.receipt);
			}  
			return PurchaseProcessingResult.Complete;  
		} catch (Exception ) {  
			Debug.Log("Invalid receipt, not unlocking content");  
			return PurchaseProcessingResult.Complete;  
		}  
	}  
	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
		// this reason with the user to guide their troubleshooting actions.

		Debug.Log (string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
		DialogController.GetInstance ().showMessagePanel (LanController.getString("paymentError02"));

	}
	private void OnDeferred(Product item)  
	{  
		Debug.Log("Purchase deferred: " + item.definition.id);  
	}

	public const string TITLE01 = "Title01";

	//task

	public void personCheck(){
		if(PlayerData.getCurTask() == null){
			if(PlayerData.hasTitle (GameController.TITLE01)){
				addTask ("random");
			}else{
				addTask ("random");
			}
		}

	}

	public void finishTask(string id){
		if(CurrentScene == LOGINSCENE){
			LoginController.GetInstance ().initTreasure ();
		}
		if (id == "random") {
			PlayerData.setCurTask (null);
		} else {
			string nextId = (int.Parse (id) + 1).ToString ();
			TaskItemSpec nextSpec = SpecController.getItemById (nextId) as TaskItemSpec;
			if (nextSpec != null) {
				addTask (nextId);
			} else {
				PlayerData.setCurTask (null);
				creatMyShip ();
				if(id == "70003"){
					creatMyShip ();
					if(CurrentScene == LOGINSCENE){
						LoginController.GetInstance ().initShipButton ();
					}
				}
			}
		}
	}

	void creatMyShip(){
		ShipData data = new ShipData ();
		data.gameUid = PlayerData.getGameuid ();
		data.scaleX = 1;
		data.shipName = PlayerData.getPlayerName ();
		data.targetID = "NoviceIsland";
		data.posx = 90f;
		data.posy = -60f;
		PlayerData.updateMyShipData (data);
	}

	//添加宠物
	public void addPet(string id){
		Dictionary<string,ItemSpec>dic =  SpecController.getGroup ("Fish");
		string[] dicNames = new string[dic.Count];
		dic.Keys.CopyTo (dicNames, 0);

		PetData petData = new PetData ();
		petData.initId (PlayerData.creatPetId(), id);
		petData.curHeart = 2000;
		PlayerData.addPet (petData);
		LoginController.GetInstance ().creatNewPet (petData);
	}

	public TaskData addTask(string id){
		TaskData data = new TaskData ();
		data.init (id);
		PlayerData.setCurTask (data);
		return data;
	}


	public void saveData(){
		//任务
		PlayerData.setCurTask (PlayerData.getCurTask());
		
	}

	//world FISH
	public void creatTrappedFish(HomeObject obj,int creatTime){
		string id = BattleFormation.getRanFishId ();
		obj.trappedId = "10029";
		Vector2 p = UnityEngine.Random.insideUnitCircle *3;
		Vector2 pos = p.normalized*(2.5f+p.magnitude);  
		obj.trappedPosx = pos.x*50;
		obj.trappedPosy = pos.y*50;
		obj.trappedTime = creatTime;
		PlayerData.updateHomeObject (obj);
	}

	public MissionFishData creatMissionFish(int level,int creatTime){
		string id = BattleFormation.getRandFishIdByLevel (level);
		MissionFishData d = new MissionFishData ();
		d.item_id = id;
		d.fishLevel = level;
		d.showTime = creatTime;
		d.wholeTime = level * 30;
		if (level == 3) {
			d.levelCount = "5:5|10:3|15:3";
		} else if (level == 2) {
			d.levelCount = "1:10|5:5|8:2|10:1";
		} else {
			d.levelCount = "1:10|2:8|3:6|4:3|5:1";
		}
		return d;
	} 
}
