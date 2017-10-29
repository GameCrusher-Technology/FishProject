using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WorldShip : MonoBehaviour
{

	Rigidbody2D mainRig;
	ShipData shipData;
	int speed;
	public void init(ShipData data){
		shipData = data;
		transform.Find ("Name").GetComponent<Text> ().text = data.shipName;
		mainRig = GetComponent<Rigidbody2D> ();
		speed =5000;
	}

	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (shipData != null) {

			if(curTargetPos!= Vector2.zero){
				float curDistance = Vector2.Distance (curTargetPos,transform.position);
				if (curDistance == lastDistance) {
					getNewForce ();
				} else if(curDistance < lastDistance){
					lastDistance = curDistance;
				}
			}
		}
	}

	void getNewForce(){
		if(curTargetPos != Vector2.zero){
			Vector2 curDirection = (curTargetPos - (Vector2)transform.position).normalized;
			if (curDirection.x == 0 && curDirection.y == 0) {
				curDirection = new Vector3 (1,1,0).normalized;
			}
			addForce(new Vector2 (curDirection.x * speed, curDirection.y * speed));
			transform.localScale = new Vector3 (curDirection.x<0 ? 1:-1,1,1);
			transform.localRotation = Quaternion.identity ;
		}
	}
	public void getCollider(Vector3 pos,string targetId){
		if (targetId == shipData.targetID) {
			GetComponent<Rigidbody2D> ().Sleep ();
			arriveToTarget ();
		} else if(curTargetPos != Vector2.zero) {
			Vector3 curDirection = (transform.position - pos).normalized;
			Vector2 toDirection = (curTargetPos - (Vector2)transform.position).normalized;
			Vector3 a = Vector3.Cross (curDirection, toDirection);
			curDirection = Quaternion.AngleAxis(a.z>0?90f: -90f,Vector3.forward)*curDirection;
			if (curDirection.x == 0 && curDirection.y == 0) {
				curDirection = new Vector3 (1,1,0).normalized;
			}

			lastDistance = Vector2.Distance (curTargetPos,transform.position);
			addForce(new Vector2 (curDirection.x * speed, curDirection.y * speed));
			transform.localScale = new Vector3 (curDirection.x<0 ? 1:-1,1,1);
		}
	

	}

	Vector2 curTargetPos = Vector2.zero;
	float lastDistance;
	public void moveToTarget(Vector2 _targetPos,string id){
		curTargetPos = _targetPos;
		lastDistance = Vector2.Distance (curTargetPos,transform.position);
		shipData.targetID = id;
		getNewForce ();

		shipData.scaleX = (int) transform.localScale.x;
		PlayerData.updateMyShipData (shipData);
	}

	void arriveToTarget(){
		curTargetPos = Vector2.zero;

	
	}

	void addForce(Vector2 vec){
		mainRig.Sleep ();
		mainRig.AddForce (vec);
	}

	public void onClick(){
		WorldController.GetInstance().onPlayerShipSelected(shipData.gameUid,transform.position);
	}
}

