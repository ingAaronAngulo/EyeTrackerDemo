using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSpawner : MonoBehaviour {

	public PlaneCollider[] colliders;

	public int next;
	private void Start() {
		SpawnNextCollider();	
	}
	public void SpawnNextCollider()
	{
		//Gap min 12.5 max 20
		//Thickness min 0.25f max 100
		//offset min -5 max 5
		var currentCollider = colliders[next];

		currentCollider.gap = Random.Range(12.5f, 15f);
		currentCollider.thickness = Random.Range(0.25f, 10);
		currentCollider.offset = Random.Range(-5f, 5f);
		currentCollider.ApplyParameters();
		currentCollider.transform.position = new Vector3(
			currentCollider.transform.position.x,
			transform.position.y - 7.5f,
			currentCollider.transform.position.z);
		currentCollider.GetComponent<Collider>().enabled = true;

		if(++next == 5)
			next = 0;
	}
}
