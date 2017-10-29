using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WorldController : MonoBehaviour {
	public GameObject AimButton;
	public GameObject AimMoveIcon;
	public GameObject ArrowButton;


	public GameObject SeaCanvas;
	public GameObject WorldFishCanvas;
	public GameObject HomeFishCanvas;
	public GameObject LandCanvas;
	public GameObject ShipCanvas;
	public GameObject CloudCanvas;
	public GameObject UICanvas;

	Slider scaleSlider;
	RectTransform seaCanvas;

	Vector2 homePos = new Vector2(0,0);
	float orSize = 400f;

	private static WorldController _controller;
	public static WorldController GetInstance(){
		if (!_controller) {
			_controller = (WorldController)GameObject.FindObjectOfType(typeof(WorldController));
		}
		if (!_controller) {
			Debug.LogError (" no WorldController");
		}
		return _controller;
	}

	void Start(){
		gameObject.GetComponent<AudioSource> ().volume = PlayerData.getMusic ();

		seaCanvas = SeaCanvas.GetComponent<RectTransform>();
		scaleSlider = UICanvas.transform.Find ("Slider").GetComponent<Slider>();
		init ();


		float t = Mathf.Max ((float)Screen.width / 650, (float)Screen.height / 960);
		orSize = 400*t;

		Camera.main.orthographicSize = orSize;

		GameController.WorldBound = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0)) - Camera.main.ScreenToWorldPoint (Vector3.zero);
		if(myWorldShip != null){
			homePos = myWorldShip.GetComponent<RectTransform> ().anchoredPosition;
			moveWorld(-homePos + (Vector2)Camera.main.transform.position);
			onPlayerShipSelected (PlayerData.getGameuid (), myWorldShip.transform.position);
		}

	}

	void init(){
		initBattleShip ();
		initWorldFish ();
		scaleSlider.value = 0;
	}

	void initBattleShip(){
		ShipData data = PlayerData.getMyShipData ();
		if(data != null){
			creatPlayerShip (data);

		}

	}
	void initWorldFish(){
		
	}



	void FixedUpdate () {
		
	}

	void creatWorldFish(WorldFishData data){
		GameObject pFish = (GameObject)Instantiate(GameController.GetInstance ().getPrefab (data.getFishName()),data.position,Quaternion.identity,WorldFishCanvas.transform);
		pFish.AddComponent<WorldFish> ().init (data);
	}
	WorldShip myWorldShip;
	void creatPlayerShip(ShipData data){
		GameObject pShip = (GameObject)Instantiate(GameController.GetInstance ().getPrefab ("PlayerShip"),new Vector2((float)data.posx,(float)data.posy),Quaternion.identity,ShipCanvas.transform);
		myWorldShip = pShip.GetComponent<WorldShip> ();
		myWorldShip.init (data);
	}

	void creatWorldBoat(){
		
	}

	void checkArrow(){
		Vector2 vec = new Vector2 ( Camera.main.transform.position.x - homePos.x , Camera.main.transform.position.y - homePos.y);
		if (Mathf.Abs (vec.x) >= Camera.main.orthographicSize || Mathf.Abs (vec.y) >= Camera.main.orthographicSize) {
			ArrowButton.SetActive (true);
			(ArrowButton.transform as RectTransform).anchoredPosition = -vec.normalized * (300);

			float tarRot = Mathf.Atan2 (vec.y, vec.x) * Mathf.Rad2Deg  + 20f;
			ArrowButton.transform.rotation =  Quaternion.Slerp (ArrowButton.transform.localRotation, Quaternion.Euler (0, 0, tarRot), 100f * Time.deltaTime);
		} else {
			ArrowButton.SetActive (false);
		}
	}


	public void onScaleValueChanged(Slider obj){
		Camera.main.orthographicSize = orSize + (obj.value *10 );
		moveWorld (new Vector2 (0, 0));
		if(obj.value > 0){
			WorldFishCanvas.SetActive (true);
		}
		GameController.WorldBound = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, 0)) - Camera.main.ScreenToWorldPoint (Vector3.zero);
	}


	Vector2 checkRectBounds(Vector2 pos){
		return new Vector2 (Mathf.Min(seaCanvas.rect.width/2 , Mathf.Max(-seaCanvas.rect.width/2 , pos.x)),
							Mathf.Min(seaCanvas.rect.height/2, Mathf.Max(-seaCanvas.rect.height/2 ,pos.y)));
	}
	public void onClickArrow(){
		moveWorld(-homePos + (Vector2)Camera.main.transform.position);
	}
	public void onClickAim(){
		AimButton.SetActive (false);
		AimMoveIcon.SetActive (true);
	}

	public void onIslandSelected(GameObject island){
		//scaleSlider.value = 0;
		moveWorld(-island.GetComponent<RectTransform> ().anchoredPosition + (Vector2)Camera.main.transform.position);

		if (island.name == "NoviceIsland") {
			if (PlayerData.getMyShipData () == null) {
					DialogController.GetInstance ().showMessagePanel (LanController.getString("beginShipTask"),true,delegate {
					TaskData data =  PlayerData.getCurTask();
					if(data == null || data.taskId == "random"){
						DialogController.GetInstance().showTaskPanel(GameController.GetInstance().addTask("70001"));
					}else{
						DialogController.GetInstance().showTaskPanel(data);
					}
				});
			} else {
				DialogController.GetInstance ().showIslandPanel (island.transform.name);
			}
		} else {
			DialogController.GetInstance ().showMessagePanel (LanController.getString("comingsoon"));
		}
			
		AudioController.PlaySound (AudioController.SOUND_button);

	}
		
	public void onPlayerShipSelected(string id,Vector2 pos){

		moveWorld(-pos+ (Vector2)Camera.main.transform.position);
		showHomeObjects (id,pos);
		//LandCanvas.SetActive (false);
		WorldFishCanvas.SetActive (false);
		AudioController.PlaySound (AudioController.SOUND_button);
	}
	string currentHomeId ;
	void showHomeObjects(string id,Vector3 pos){
		if (currentHomeId != id) {
			cleanHomeLayer ();
			currentHomeId = id;

			List<GameObject> Glist = new List<GameObject> ();
			HomeObject curObject = PlayerData.getHomeObject (id);
			if (curObject == null) {
				curObject = creatNewHomeData (id);
			}
			//Trapped 
			if (curObject.trappedId == null) {
				GameController.GetInstance().creatTrappedFish (curObject, 0);
			}
			GameObject tFish = (GameObject)Instantiate (GameController.GetInstance ().getPrefab ("TrapedFish"), new Vector3 (pos.x + (float)curObject.trappedPosx, pos.y + (float)curObject.trappedPosy, pos.z), Quaternion.identity, HomeFishCanvas.transform);
			tFish.name = "TrappedFish";
			tFish.GetComponent<TrappedFish> ().init (curObject);
			Glist.Add (tFish);
			if (tFish.GetComponent<TrappedFish> ().canBeClick ()) {
				tFish.SetActive (true);
			} else {
				tFish.SetActive (false);
			}

			//MISSION
			if(id == PlayerData.getGameuid()){
				curObject.missionFishes = checkMissionFishes (curObject.missionFishes);
			}
		
			foreach(MissionFishData data in curObject.missionFishes){
				if(data.showTime <= GameController.GetInstance().getCurrentSystemNum()){
					GameObject mFish = (GameObject)Instantiate (GameController.GetInstance ().getPrefab ("MissionFish"), pos, Quaternion.identity, HomeFishCanvas.transform);
					mFish.name = "MissionFish";
					mFish.GetComponent<MissionFish> ().init (data,pos);
				}
			}

			if(id == PlayerData.getGameuid()){
				PlayerData.updateHomeObject (curObject);
			}
		} 
	}

	void cleanHomeLayer(){

		for (int i = 0; i < HomeFishCanvas.transform.childCount; i++) {
			Destroy ( HomeFishCanvas.transform.GetChild (i).gameObject);
		}
	}

	List<MissionFishData> checkMissionFishes( List<MissionFishData> list){
		bool[] bArr = new bool[3]{false,false,false };
		List<MissionFishData> newlist = new List<MissionFishData> ();
		foreach (MissionFishData data in list) {
			bArr [data.fishLevel -1 ] = true;
			if (data.isEnd ()) {
				newlist.Add(GameController.GetInstance().creatMissionFish (data.fishLevel, GameController.GetInstance ().getCurrentSystemNum ()));
			} else {
				newlist.Add(data);
			}
		}
		for (int i = 0; i < bArr.Length; i++) {
			if(!bArr[i]){
				newlist.Add(GameController.GetInstance().creatMissionFish (i+1,GameController.GetInstance().getCurrentSystemNum()));
			}
		}

		return newlist;
	}

	HomeObject creatNewHomeData(string id){
		HomeObject obj = new HomeObject ();
		obj.gameUid = id;
		GameController.GetInstance().creatTrappedFish (obj,0);

		PlayerData.updateHomeObject (obj);
		return obj;
	}

	//move
	Vector2 offset = Vector2.zero;
	public void pointDown(BaseEventData eventData){
		Vector2 mouseUguiPos = new Vector2 ();
		bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(seaCanvas, (eventData as PointerEventData).position, Camera.main, out mouseUguiPos);
		if(isRect){
			offset = - mouseUguiPos ;
		}
	}

	public void moveMyShip (Vector2 pos,string id){
		if(myWorldShip != null){
			myWorldShip.moveToTarget (pos,id);
		}
	}


	public void draging(BaseEventData eventData){
		Vector2 uguiPos = new Vector2();   
		//和上面类似
		bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(seaCanvas, (eventData as PointerEventData).position, Camera.main, out uguiPos);
		if (isRect) {
			moveWorld (offset + uguiPos);
		}
	}


	public void moveWorld(Vector2 movepos){
		Vector2 vec= checkRectBounds ((Vector2)Camera.main.transform.position - movepos);
		Camera.main.transform.position = new Vector3 (vec.x,vec.y,-10);
		checkArrow ();
	}

	public void clickHome(){
		GameController.GetInstance ().HomeScene ();
	}
	public void clickBattle(){
		GameController.GetInstance ().BeginBattle ();
	}

	bool isMaxScale(){
		return Camera.main.orthographicSize == 300; 
	}
}
