using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tobii.Gaming;
using UnityEngine;

public class Plane : MonoBehaviour {
	public Camera camera;
	public GazePoint gazePoint;
	public Transform cursor;
	public Vector3 offsetRotation;
	public TextMeshProUGUI txtScore;
	public SphereSpawner spawner;
	public float speed;
	public int score;
	public bool isMovible;

	private void Awake() {
		camera = Camera.main;
		cursor = GameObject.Find("Cursor").transform;
		txtScore = GameObject.Find("TxtScore").GetComponent<TextMeshProUGUI>();
		txtScore.text = "" + score;
		spawner = GetComponent<SphereSpawner>();
	}

	private void Update() {
		if(Input.GetKeyDown(KeyCode.Space))
			isMovible = !isMovible;
	}

	private void FixedUpdate() {
		gazePoint = TobiiAPI.GetGazePoint();

		if (gazePoint.IsRecent() && Validate(gazePoint.Viewport)) 
		{
			var pos = camera.ScreenToWorldPoint(new Vector3(gazePoint.Screen.x,gazePoint.Screen.y, 10));
        	
			if(isMovible)
				transform.position = Vector3.MoveTowards(transform.position, new Vector3(pos.x, pos.y, 0), Time.deltaTime * speed);
			
			transform.LookAt(new Vector3(pos.x, pos.y, transform.position.z));
			offsetRotation.z = transform.position.x < pos.x ? 270 : 90;
			transform.Rotate(offsetRotation);

			if(cursor)
			{
				cursor.position = Vector3.Lerp(
					cursor.position,
					pos,
					Time.deltaTime * speed);
			}
		}
	}

	public bool Validate(Vector2 position) 
	{
		if (position.x < 0 || position.x > 1 || position.y < 0 || position.y > 1)
			return false;
			
		return true;
	}

	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Obstacle")
		{
			gameObject.SetActive(false);
		}
	}

	private void OnTriggerExit(Collider other) {
		if(other.tag == "ScoreCollider")
		{
			other.GetComponent<Collider>().enabled = false;
			score++;
			txtScore.text = "" + score;
			spawner.SpawnNextCollider();
			speed += 0.1f;
		}
	}
}
