using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour
{
	//加速度センサーの傾き
	private Vector2 acc_vec;
	private Vector2 org_vec;

	// Use this for initialization
	void Start()
	{
		//加速度を取得
		acc_vec = Input.acceleration;
		org_vec = new Vector2();

		if (acc_vec != null) {
//			acc_vec.x = Input.acceleration.x;
			acc_vec.y = -Input.acceleration.z;
			acc_vec.x = 0;

			org_vec = acc_vec;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		//加速度を取得
		acc_vec = Input.acceleration;

		if (acc_vec != null) {
//			acc_vec.x = Input.acceleration.x;
			acc_vec.y = -Input.acceleration.z;
			acc_vec.x = 0;
			transform.position = acc_vec - org_vec;

			Vector2 dir = acc_vec - org_vec;
//			dir *= Time.deltaTime;
			org_vec += (dir * 0.005f);
		}
	}

	void OnGUI()
	{
		// テキスト描画
		Vector3 vec = Input.acceleration;

		if (vec != null) {
			string text;
			text = string.Format("x={0}", vec.x);
			Util.GUILabel(0, Screen.height / 20 * 1, 120, 30, text);
			text = string.Format("y={0}", vec.y);
			Util.GUILabel(0, Screen.height / 20 * 2, 120, 30, text);
			text = string.Format("z={0}", vec.z);
			Util.GUILabel(0, Screen.height / 20 * 3, 120, 30, text);
		}
	}
}
