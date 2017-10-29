using UnityEngine;

public class FishData {
	public string fishName;
	public Vector3 direction;
	public Vector3 position;
	public float creatTime;
	public Quaternion fishRot;
	public string item_id;
	public int heart;
	public int repeatTime;
	public float curBlood;

	FishItemSpec spec;
	int speed ;
	public FishData(FishItemSpec _spec,Vector3 _direction, Vector3 _position,float time ,int _setSpeed ,int _repeatTime ){
		spec = _spec;
		item_id = _spec.item_id;
		fishName = spec.name;
		direction = _direction;
		position = _position;
		creatTime = time;
		heart = spec.coin;
		speed = _setSpeed;
		repeatTime = _repeatTime;
		curBlood = spec.Blood;
		float tar =  Mathf.Atan2 (_direction.y, _direction.x) * Mathf.Rad2Deg + 180f ;
		fishRot = Quaternion.Slerp (Quaternion.identity, Quaternion.Euler (0, 0, tar), 3f );
	}

	public int getCurSpeed(){
		if (speed > 0) {
			return speed;
		} else {
			return spec.speed;
		}
	}


	public int getRadits(){
		return spec.radits;
	}

}
