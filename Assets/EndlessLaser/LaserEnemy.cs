using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaserEnemy : MonoBehaviour {

	public Vector3 objective;
	public Renderer renMaterial;
	public Color initialColor;
	public float damageMultiplier;
	public float health;
	public float speed;
	public float damage;
	public bool isBeingDamaged;

	private void Awake() 
	{
		renMaterial = GetComponent<Renderer>();
		objective = GameObject.Find("LaserEye" + Random.Range(1, 3)).transform.GetChild(0).position;
		initialColor = renMaterial.material.GetColor("_Color");
	}

	private void Update() {
		if(isBeingDamaged)
			ApplyDamage();
		
		if (!isBeingDamaged && health > 0f)
		{
			if(health < 0)
				health = 0;
			else
				health -= 0.01f;
			
			renMaterial.material.SetColor(
				"_Color", 
				Color.Lerp(
					initialColor,
					Color.red,
					health));
		}
	}

	private void FixedUpdate() {
		if(objective != null)
			transform.position = Vector3.MoveTowards(transform.position, objective, Time.timeScale * speed);
		else
			objective = GameObject.Find("LaserEye" + Random.Range(1, 3)).transform.GetChild(0).position;
	}

	public void ApplyDamage()
	{
		health += damageMultiplier;

		renMaterial.material.SetColor(
			"_Color", 
			Color.Lerp(
				initialColor,
				Color.red,
				health));
		if(health >= 1)
		{
			GameObject.Find("TxtScore").GetComponent<TextMeshProUGUI>().text = "" + 
				(int.Parse(GameObject.Find("TxtScore").GetComponent<TextMeshProUGUI>().text) + 1);
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Laser")
			isBeingDamaged = true;
		if (other.tag == "LaserEntity")
		{
			other.transform.parent.GetComponent<Laser>().ApplyDamage(damage);
			Destroy(gameObject);
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == "Laser")
			isBeingDamaged = false;
	}
}
