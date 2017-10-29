using UnityEngine;

public class WorldFishData {
	public Vector3 direction;
	public Vector3 position;
	public float creatTime;
	public string item_id;
	public int heart;
	public float curBlood;
	public int level;


	public int getCurSpeed(){
		return getSpec().speed;
	}
	public int getBlood(){
		return getSpec().Blood;
	}

	public string getFishName(){
		return getSpec().name;
	}
	public int getRadits(){
		return getSpec().radits;
	}

	public int getAttackSpeed(){
		return getSpec ().attackSpeed + level;
	}

	FishItemSpec spec;
	private FishItemSpec getSpec(){
		if (spec == null) {
			spec = SpecController.getItemById (item_id) as FishItemSpec;
		}
		return spec;
	}

	public string getBulletName(){
		return getSpec().bullet;
	}

	public float getBulletSpeed(){
		return getCurSpeed()*5;
	}
}
