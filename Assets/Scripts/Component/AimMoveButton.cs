using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class AimMoveButton : MonoBehaviour,IDragHandler,IPointerDownHandler,IPointerUpHandler{
	
	public RectTransform UIcanvas;
	public GameObject AimButton;

	Vector2 offset = new Vector2();
	// Use this for initialization
	void Start () {
		UIcanvas.sizeDelta = new Vector2(Screen.width, Screen.height); 
	}


	// Update is called once per frame
	void FixedUpdate () {
		Vector2 movePos = new Vector2 (-(transform as RectTransform).anchoredPosition.x / 20, -(transform as RectTransform).anchoredPosition.y /20);
		WorldController.GetInstance ().moveWorld (movePos);
	}

	public void OnPointerDown(PointerEventData eventData){
		Vector2 mouseDown = eventData.position;
		Vector2 mouseUguiPos = new Vector2 ();

		bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(UIcanvas, mouseDown, eventData.enterEventCamera, out mouseUguiPos);
		if(isRect){
			offset =  - mouseUguiPos;
		}

	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 mouseDrag = eventData.position;   //当鼠标拖动时的屏幕坐标
		Vector2 uguiPos = new Vector2();   //用来接收转换后的拖动坐标
		//和上面类似

		bool isRect = RectTransformUtility.ScreenPointToLocalPointInRectangle(UIcanvas, mouseDrag, eventData.enterEventCamera, out uguiPos);
		if (isRect) {
			//设置图片的ugui坐标与鼠标的ugui坐标保持不变
			(transform as RectTransform).anchoredPosition = checkRectBounds (offset + uguiPos);

		} 
	}



	Vector3 checkRectBounds(Vector2 pos){
		return new Vector2 (Mathf.Min(Screen.width/2,Mathf.Max(-Screen.width/2,pos.x)),Mathf.Min(Screen.height/2,Mathf.Max(-Screen.height/2,pos.y)));
	}
		

	public void OnPointerUp(PointerEventData eventData){
		(transform as RectTransform).anchoredPosition = Vector2.zero;
		gameObject.SetActive (false);
		AimButton.SetActive (true);
	}

	public void OnEndDrag(PointerEventData eventData){
		Debug.Log ("OnEndDrag");
		offset = Vector2.zero;
	}

}
