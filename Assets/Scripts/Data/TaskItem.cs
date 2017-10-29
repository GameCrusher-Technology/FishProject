using UnityEngine;
using System.Collections;

public class TaskItem  {
	public string item_id;
	public int finished;
	public int total;

	public bool beFinished(){
		return finished >= total;
	}
}
