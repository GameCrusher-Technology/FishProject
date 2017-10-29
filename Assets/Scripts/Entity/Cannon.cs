using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour
{
	Animator animator;
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}


	void endFrame(){
		getAnimator().enabled = false;
	}

	private Animator getAnimator(){
		if (!animator) {
			animator = GetComponent<Animator> ();
		}
		return animator;
	}

}

