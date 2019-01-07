using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneCollider : MonoBehaviour {

	public BoxCollider boxCollider;
	public Transform leftCollider;
	public Transform rightCollider;
	public float gap;
	public float thickness;
	public float offset;
	
	private void Awake() {
		leftCollider = transform.GetChild(0);
		rightCollider = transform.GetChild(1);
		boxCollider = GetComponent<BoxCollider>();
	}

	public void ApplyParameters()
	{
		leftCollider.localPosition = new Vector3(-gap/2, 0, 0);
		leftCollider.localScale = new Vector3(10, thickness, 1);
		rightCollider.localPosition = new Vector3(gap/2, 0, 0);
		rightCollider.localScale = new Vector3(10, thickness, 1);
		transform.position = new Vector3(offset, transform.position.y - thickness, transform.position.z);
		boxCollider.center = new Vector3(0, -thickness / 2, 0);
	}
}
