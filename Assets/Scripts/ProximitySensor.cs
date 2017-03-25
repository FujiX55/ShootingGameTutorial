using UnityEngine;
using System.Collections;

public class ProximitySensor : MonoBehaviour
{
	int alert;

	public int Alert { 
		get { return alert; } 
	}

	// Use this for initialization
	void Start()
	{
		alert = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate()
	{
		if (0 < alert) {
			alert--;
		}
	}

	/// 衝突判定
	void OnTriggerEnter2D(Collider2D other)
	{
		string name = LayerMask.LayerToName(other.gameObject.layer);
		switch (name) {
		case "Enemy":
		case "Bullet":
			// 振動してみたり
			Vibration.Vibrate(50);
			break;
		}
	}

	/// 衝突判定
	void OnTriggerStay2D(Collider2D other)
	{
		string name = LayerMask.LayerToName(other.gameObject.layer);
		switch (name) {
		case "Enemy":
		case "Bullet":
			alert = 20;
			break;
		}
	}
}
