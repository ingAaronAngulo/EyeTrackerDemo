using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;

public class Laser : MonoBehaviour {

	public ParticleSystem.EmissionModule emissionModule;
	public ParticleSystem.ShapeModule particlePosition;
	public BoxCollider collider;
	public LineRenderer laser;
	public GazePoint gazePoint;
	public Camera camera;
	public Vector3 initialPosition;
	public float laserRegeneration;
	public float laserDegeneration;
	public float laserLimit;
	public float laserSpeed;
	public float health;
	private void Awake() {
		camera = Camera.main;
		laser = GetComponent<LineRenderer>();
		initialPosition = laser.GetPosition(1);
		collider = GetComponent<BoxCollider>();
		emissionModule = GetComponent<ParticleSystem>().emission;
		particlePosition = GetComponent<ParticleSystem>().shape;
	}

	private void FixedUpdate() {
		
		gazePoint = TobiiAPI.GetGazePoint();

		if (gazePoint.IsRecent() && Validate(gazePoint.Viewport) && laser.widthMultiplier > 0f) 
		{
			collider.enabled = true;
			emissionModule.enabled = true;
			var pos = camera.ScreenToWorldPoint(new Vector3(gazePoint.Screen.x,gazePoint.Screen.y, 10));
			var fixedPos = new Vector3(pos.x, pos.y, 0);
			
			particlePosition.position = new Vector3(fixedPos.x - transform.position.x, fixedPos.y, fixedPos.z);;
			laser.SetPosition(1, fixedPos);
			collider.center = new Vector3(fixedPos.x - transform.position.x, fixedPos.y, fixedPos.z);
			laser.widthMultiplier -= laserDegeneration;
			collider.size = new Vector3(laser.widthMultiplier, laser.widthMultiplier, laser.widthMultiplier);
		}
		else
		{
			collider.enabled = false;
			emissionModule.enabled = false;
			if(laser.widthMultiplier < laserLimit)
				laser.widthMultiplier += laserRegeneration;

			laser.SetPosition(1, initialPosition);
		}
	}

	public bool Validate(Vector2 position) 
	{
		if (position.x < 0 || position.x > 1 || position.y < 0 || position.y > 1)
			return false;
			
		return true;
	}

	public void ApplyDamage(float damage)
	{
		health -= damage;
		if(health <= 0)
			Destroy(gameObject);
	}
}
