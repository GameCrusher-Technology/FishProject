using UnityEngine;
using System.Collections;

public class FishHole : MonoBehaviour {

	FishItemSpec spec;
	float maxCount = 100f;
	int count = 20;
	Vector3 direct = new Vector3(0,1,0);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frameD
	void Update () {
		if(spec != null){
			if (maxCount > 0) {
				if (count <= 0) {
					creatFish ();
					count = 20;
					maxCount--;
				} else {
					count--;
				}
			} else {
				Destroy (gameObject);
			}

		}
	}

	public void init(string fishId,float _maxCount){
		spec = SpecController.getItemById (fishId) as FishItemSpec;
		maxCount = _maxCount;
	}

	void creatFish(){
		direct = Quaternion.AngleAxis(20f,Vector3.forward)*direct.normalized;

		FishData data = new FishData (spec, direct, transform.position, 0, 0, 0);
		BattleController.GetInstance ().creatFish (data);
	}
}
