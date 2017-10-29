using UnityEngine;
using System.Collections;

public class PetSkin : MonoBehaviour
{
	bool canuse = false;
	// Use this for initialization
	void Start ()
	{
	}

	public void fullSkill(){
		canuse = true;
	}

	public void useSkill(){
		canuse = false;
		transform.rotation =Quaternion.identity;
	}
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (canuse) {
			transform.Rotate (new Vector3 (0, 0, 20));
		} 
	}
}

