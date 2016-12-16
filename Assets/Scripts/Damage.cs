using UnityEngine;
using System.Collections;

public class Damage : MonoBehaviour {

	public void OnAnimFinish()
	{
		this.gameObject.SetActive (false);
	}
}
