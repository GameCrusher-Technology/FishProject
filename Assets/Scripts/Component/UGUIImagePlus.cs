using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UGUIImagePlus : Image {
	PolygonCollider2D ucollider;
	// Use this for initialization
	protected override void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	protected override void Awake(){
		ucollider = GetComponent<PolygonCollider2D> ();
	} 

	public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera){
		bool inside = ucollider.OverlapPoint (screenPoint);
		return inside;
	}
}
