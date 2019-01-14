using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpaceShip : Entity {

	[Header("SpaceShip")]
	public GameObject[] health;
	public Vector2 direction;
	public Rigidbody2D rBody;
	public float horizontalSpeed;
	public float verticalSpeed;
	public float axis;
	public bool isGyroActivated;
	public bool hasPiercingShot;
	public HeadPose headPose;
	
	private void Awake() {
		isGyroActivated = PlayerPrefs.GetInt("Gyro") == 1 ? true : false;

		direction = transform.position;
		rBody = GetComponent<Rigidbody2D>();
		for (int i = 0; i < healthPoints; i++)
			health[i] = GameObject.Find("IHealth" + i);
	}

	private void Update() {
		if(Input.GetMouseButtonDown(0))
			Shot();
	}

	private void FixedUpdate() {
		headPose = TobiiAPI.GetHeadPose();
		if(headPose.IsValid)
		{
			rBody.velocity = new Vector2(headPose.Rotation.y * horizontalSpeed, verticalSpeed);
		}

		headPose = TobiiAPI.GetHeadPose();
		if(headPose.IsValid)
		{
			rBody.velocity = new Vector2(headPose.Rotation.y * horizontalSpeed, verticalSpeed);
		}

		direction.y += verticalSpeed;
	}

	public void Shot()
	{
		var bullet = PoolManager.instance.GetInactivePrefab((int) Constants.IDS.BULLET);
		bullet.transform.position = transform.position;
		var bulletScript = bullet.GetComponent<Bullet>();
		bulletScript.isPiercingShot = hasPiercingShot;
		bulletScript.bulletSpeed = verticalSpeed + 200f;
		bulletScript.damagePoints = damagePoints;
		bullet.SetActive(true);
	}

	public override void ReceiveDamage(int damagePoints)
	{
		health[healthPoints - 1].SetActive(false);
		healthPoints--;
		damagePoints--;
		
		if(damagePoints == 0)
			damagePoints = 1;
		
		hasPiercingShot = false;

		if(healthPoints <= 0)
			DeActivate();
	}

	public override void DeActivate()
	{
		EventManager.Instance.GameOver();
		Destroy(gameObject);
	}
}
