using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldSea : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(Screen.width >= 800){
			float t = Mathf.Max ((float)Screen.width / 650, (float)Screen.height / 960)/2 +0.5f;
			gameObject.transform.localScale = new Vector3(t,t,1);
		}
		Vector3 vec = Camera.main.ViewportToWorldPoint (new Vector3(1,1,1));

		gameObject.transform.localScale = new Vector3 ((float)vec.y/9.6f*2,(float)vec.x/6.5f*2,1);
	}
	
	// Update is called once per frame
	void FixedUpdate () {

	}


/*	IEnumerator OnMouseDown(){
		if (!EventSystem.current.IsPointerOverGameObject ()) {
			GameObject o = EventSystem.current.gameObject;
			if (EventSystem.current.gameObject.GetComponent ("Island")) {
				Debug.Log ("Island");
			} else {
				Vector3 ScreenSpace = Camera.main.WorldToScreenPoint (transform.position);
				Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, ScreenSpace.z));

				while (Input.GetMouseButton (0)) {
					Vector3 curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, ScreenSpace.z);
					Vector3 CurPosition = Camera.main.ScreenToWorldPoint (curScreenSpace) + offset;

					transform.position = checkRectBounds (CurPosition);
					yield return new WaitForFixedUpdate ();
				}
			}
		}
	}

	Vector3 checkRectBounds(Vector3 pos){
		return new Vector3 (Mathf.Min(bound.x,Mathf.Max(-bound.x,pos.x)),Mathf.Min(bound.y,Mathf.Max(-bound.y,pos.y)),pos.z);
	}
	void setBound(){
		Bounds seaBounds= transform.GetComponent<SpriteRenderer> ().bounds;
		Vector2 viewP =  Camera.main.ScreenToWorldPoint (new Vector3(Screen.width,Screen.height));
		bound = new Vector2 (seaBounds.extents.x - viewP.x, seaBounds.extents.y - viewP.y);
	}

	public void onScaleValueChanged(Slider obj){
		float newSize = 6 + (obj.value * 10);
		Camera.main.orthographicSize = newSize;
		setBound ();
		transform.position = checkRectBounds(transform.position);
	}
*/
}
