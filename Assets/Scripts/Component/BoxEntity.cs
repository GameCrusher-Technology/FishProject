using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class BoxEntity : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseDown(){

		if (!GameController.GetInstance().isNewer && !EventSystem.current.IsPointerOverGameObject () && EventSystem.current.currentSelectedGameObject == null) {
			
			GameController.GetInstance ().addPet (GameController.initPetId);
			Destroy (gameObject);

			DialogController.GetInstance ().removeMessagePanel ();

			AudioController.PlaySound (AudioController.SOUND_FULLSKILL);
		}
	}

}
