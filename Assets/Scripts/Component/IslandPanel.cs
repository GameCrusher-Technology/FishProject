using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class IslandPanel : MonoBehaviour
{

	public void init(string landName){
		
		transform.Find ("TitleText").GetComponent<Text> ().text = LanController.getString (landName);
		transform.Find("IslandIcon").GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" + "IslandIcon04"); 

		GameObject listItemRender = transform.Find ("FishList").Find ("FishRender").gameObject;
		GameObject listGrid =transform.Find ("FishList").Find ("Grid").gameObject;

		Dictionary<string,ItemSpec> fishes = getFishTypes (landName);
		foreach (string key in fishes.Keys) {
			GameObject bt = Instantiate(listItemRender);
			bt.GetComponent<RectTransform>().SetParent(listGrid.GetComponent<RectTransform>());
			ItemSpec spec = SpecController.getItemById (key);
			bt.SetActive (true);
			bt.GetComponent<RectTransform>().localScale = Vector3.one;  //调整大小
			bt.GetComponent<RectTransform>().localPosition = Vector3.zero;  //调整位置
			bt.name = spec.item_id;
			bt.transform.Find ("CountText").gameObject.GetComponent<Text> ().text = "";
			bt.transform.Find("Icon").gameObject .GetComponent<Image>().sprite = GameController.GetInstance().getSpByName("Pic/ui/" + spec.name + "Icon"); 
		}

		int p = 4;
		int t = (fishes.Count % p) == 0 ? (fishes.Count / p) : ((fishes.Count / p) + 1);
		int h = t * 100 + (t - 1) * 10 + 20;
		listGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(500, h);
		listGrid.transform.Translate (new Vector3(0,-h/2,0));


		transform.Find ("FightButton").Find ("Text").GetComponent<Text> ().text = LanController.getString ("attack").ToUpper();
	}


	Dictionary<string,ItemSpec> getFishTypes(string landName){
		
		Dictionary<string,ItemSpec> fishes =	SpecController.getGroup ("Fish");	

		Dictionary<string,ItemSpec> newFishes = new Dictionary<string, ItemSpec> ();
		foreach (string key in fishes.Keys) {
			FishItemSpec spec = fishes[key] as FishItemSpec;
			if(spec.common == 1 || spec.common == 2 ||spec.common == 3 ){
				newFishes.Add (key, spec);
			}
		}

		return newFishes;
	}

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void ClickOut(){
		hidePanel ();
	}
	public void ClickBattle(){
		
		GameController.GetInstance ().BeginBattle ();
	}



	void hidePanel(){
		gameObject.SetActive (false);
	}

}

