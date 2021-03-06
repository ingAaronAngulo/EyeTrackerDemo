﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaneCamera : MonoBehaviour {

	public Transform plane;
	public Vector3 objectivePos;
	public Vector3 cameraVelocity;
	public float offsetY;
	public float speed;
	private void Awake() {
		plane = GameObject.Find("Plane").transform;
		objectivePos = transform.position;
	}

	private void Update() {
		if(Input.GetKeyDown(KeyCode.R))
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	private void FixedUpdate() {
		objectivePos.y = Mathf.Lerp(transform.position.y, plane.position.y - offsetY, speed * Time.deltaTime);
		if(objectivePos.y < transform.position.y)
			transform.position = Vector3.SmoothDamp(
				transform.position,
				objectivePos,
				ref cameraVelocity,
				0.0001f);
	}
}
