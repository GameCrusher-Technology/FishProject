using UnityEngine;
using System.Collections;

public class Net : MonoBehaviour {
	public float attack;
	public float attackRadit;
	Animator netAnimator;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void initNet (string netName,float _attack,float _radit){
		netAnimator = transform.GetComponent<Animator>();
		netAnimator.Play (netName);
		attack = _attack;
		attackRadit = _radit;
	}

	void endFrame(){
		Destroy (gameObject);
	}
}
