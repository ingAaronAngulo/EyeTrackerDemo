﻿using System.Collections;
using System.Collections.Generic;
using Tobii.Gaming;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Drager : MonoBehaviour 
{
	public Slider posX;
	public Slider posY;
	public Slider posZ; 
	public GazePoint gazePoint;
	public HeadPose headPose;
	public Camera camera;
	public Vector3 initialPosition;
	public Vector3 offsetPosition;
	public float headPositionDivider;
	public float tempZ;
	public GameObject lookingObject;
	public GameObject selectedObject;
	public Transform cursor;
	public float speed;
	public float rotationMultiplier;
	public float thickness;
	public bool isHeadDown;

	private void Awake() 
	{
		camera = Camera.main;
		initialPosition = camera.transform.position;
		cursor = GameObject.Find("Cursor").transform;

		posX = GameObject.Find("SliderPosX").GetComponent<Slider>();
		posY = GameObject.Find("SliderPosY").GetComponent<Slider>();
		posZ = GameObject.Find("SliderPosZ").GetComponent<Slider>();
	}
	// Use this for initialization
	void Start () 
	{
		TobiiAPI.SetCurrentUserViewPointCamera(Camera.main);
	}

	private void Update() 
	{
		if(Input.GetKeyDown(KeyCode.D) && selectedObject) {
			DropObject();
		}
		
		if(Input.GetKey(KeyCode.Space) && lookingObject && !selectedObject)
		{
			TakeObject(lookingObject);
		}

		offsetPosition = new Vector3(posX.value, posY.value, posZ.value);
	}

	private void FixedUpdate()
	{
		gazePoint = TobiiAPI.GetGazePoint();
		headPose = TobiiAPI.GetHeadPose();

		if (gazePoint.IsRecent() && Validate(gazePoint.Viewport)) 
		{
			var pos = camera.ScreenToWorldPoint(new Vector3(gazePoint.Screen.x,gazePoint.Screen.y, 10));
        	
			//Raycast3D(gazePoint.Screen);
			SphereCast(gazePoint.Screen);

			if(selectedObject) 
			{
				selectedObject.transform.position = Vector3.Lerp(
					selectedObject.transform.position, 
					pos,
					Time.deltaTime * speed);
			}
			
			if(cursor)
			{
				cursor.position = Vector3.Lerp(
					cursor.position,
					pos,
					Time.deltaTime * speed);
			}
		}

		if(headPose.IsValid)
		{
			Debug.Log(headPose.Rotation.x);

			if (headPose.Rotation.x >= 0.05f && !isHeadDown && lookingObject) 
			{
				isHeadDown = true;
				if(selectedObject)
					DropObject();
				else
					TakeObject(lookingObject);
			}
			if (headPose.Rotation.x <= 0f && isHeadDown)
			{
				isHeadDown = false;
			}

			camera.transform.rotation = Quaternion.Lerp(
					camera.transform.rotation,
					new Quaternion(
						headPose.Rotation.x * rotationMultiplier, 
						headPose.Rotation.y * rotationMultiplier, 
						headPose.Rotation.z * rotationMultiplier, 
						headPose.Rotation.w * rotationMultiplier),
					Time.deltaTime * speed);
			
			camera.transform.position = initialPosition + offsetPosition;
		}
	}

	public void TakeObject(GameObject go)
	{
		selectedObject = go;
		tempZ = selectedObject.transform.position.z;
		selectedObject.GetComponent<Rigidbody>().isKinematic = true;	
	}
	
	public void DropObject() 
	{
		selectedObject.GetComponent<Rigidbody>().isKinematic = false;
		selectedObject.GetComponent<Renderer>().material.SetInt("_Selected", 0);
		
		selectedObject.transform.position = new Vector3(
			selectedObject.transform.position.x, 
			selectedObject.transform.position.y, 
			tempZ);

		selectedObject = null;
	}

	public void Raycast2D(Vector3 pos) 
	{
		RaycastHit2D hit = Physics2D.Raycast(pos, -Vector2.up);
		if (hit.collider != null) 
		{
			if(Input.GetKeyDown(KeyCode.Space))
				OnRaycastHit(hit.collider.gameObject);
		}
	}

	public void Raycast3D(Vector2 pos)
	{
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(pos);

		if (Physics.Raycast(ray, out hit))
			if (hit.collider != null)
				OnRaycastHit(hit.collider.gameObject);
	}

	public void SphereCast(Vector2 pos) 
	{
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(pos);
		
		if (Physics.SphereCast(ray, thickness, out hit))
		{
			if (hit.collider != null) 
			{
				OnRaycastHit(hit.collider.gameObject);
			}
		}
		if (hit.collider == null)
			{
				if(lookingObject && !selectedObject)
				{
					lookingObject.GetComponent<Renderer>().material.SetInt("_Selected", 0);
					lookingObject = null;
				}
			}
	}

	public void OnRaycastHit(GameObject hitedObject)
	{
		if(!selectedObject && hitedObject.transform.tag == "Selectable")
		{
			if(lookingObject && hitedObject != lookingObject)
				lookingObject.GetComponent<Renderer>().material.SetInt("_Selected", 0);
				
			lookingObject = hitedObject;

			lookingObject.GetComponent<Renderer>().material.SetInt("_Selected", 1);
		}
	}

	public bool Validate(Vector2 position) 
	{
		if (position.x < 0 || position.x > 1 || position.y < 0 || position.y > 1)
			return false;
			
		return true;
	}
}