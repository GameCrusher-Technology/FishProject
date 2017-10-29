using UnityEngine;

public class PetData {
	public int data_id;
	public string fishName;
	public int fishExp = 0 ;
	public int attackSpeed;
	public string item_id;
	public int curHeart;
	public int level;
	public double posx = 0;
	public double posy = -3f;
	public int currentState = 0;


	FishItemSpec spec;
	public void initId(int _dataid,string _itemid){
		data_id = _dataid;
		item_id = _itemid;
		fishName = getSpec().name;
	}

	public int getCurSpeed(){
		return getSpec().speed +  level*getSpec().common;
	}

	public int getCurASpeed(){
		
		return 3000/getAttackSpeed()  ;
	}
	public int getAttackSpeed(){
		return getSpec ().attackSpeed + level;
	}

	public int getCurAttack(){
		return getSpec().attack + level*getSpec().common;
	}

	public float getCurAttackRadit(){
		return  level;
	}

	public float getBulletSpeed(){
		return getCurSpeed()*5;
	}


	public float getCannonScale(){
		return getSpec().cannonScale;
	}

	public string getBulletName(){
		return getSpec().bullet;
	}


	public int getLevelCost(){
		return 	(int)Mathf.Pow((level+1),2) * 1000;
	}

	private FishItemSpec getSpec(){
		if (spec == null) {
			spec = SpecController.getItemById (item_id) as FishItemSpec;
		}
		return spec;
	}

	float getCurUpExp(){
		
		return 100*Mathf.Pow((level+1),2f);
	}
	public int getUpgradeExp(){
		if (level < 0) {
			return 100;
		} else {
			return 100*(int)(Mathf.Pow((level+1),2f) - Mathf.Pow((level),2f));
		}

	}

	public bool canUp(){

		return getCurUpExp()<= fishExp;
	}

	public PetData getNextLevelData(){
		PetData data = new PetData ();
		data.item_id = item_id;
		data.data_id = data_id;
		data.fishExp = fishExp ;
		data. level = level + 1;
		data.fishName = fishName;
		return data;
	}

}
