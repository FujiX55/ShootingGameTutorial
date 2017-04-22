using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour
{
	const int BUFF_SIZE = 8;

	//加速度センサーの傾き
//	private Vector2 acc_vec;
	private Vector2 pre_vec;
	private Vector2[] his_vec;

	// Use this for initialization
	void Start()
	{
		his_vec = new Vector2[BUFF_SIZE];

		//加速度を取得
		Vector2 acc_vec = Input.acceleration;
		pre_vec = new Vector2();

//		if (acc_vec != null) 
		{
			//加速度を取得
			acc_vec.y = -Input.acceleration.z;

			if (GetParaInv()) {
				// 反転
				acc_vec.y *= -1;
			}
			acc_vec.x = 0;

			// 履歴を初期化
			for (int i = 0; i < BUFF_SIZE; i++) {
				his_vec[i] = acc_vec;
			}

			pre_vec = acc_vec;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		// 視差設定がONでなければ抜ける
		if (!GetParallax()) {
			return;
		}
		//加速度を取得
		Vector2 acc_vec = Input.acceleration;

//		if (acc_vec != null) 
		{
			//加速度を取得
			acc_vec.y = -Input.acceleration.z;
			if (GetParaInv()) {
				acc_vec.y *= -1;
			}
			acc_vec.x = 0;

			// 履歴を更新
			for (int i = 1; i < BUFF_SIZE; i++) {
				his_vec[(BUFF_SIZE - i)] = his_vec[(BUFF_SIZE - i) - 1];
			}
			his_vec[0] = acc_vec;

			// 平均化
			for (int i = 0; i < BUFF_SIZE; i++) {
				acc_vec += his_vec[i];
			}
			acc_vec /= BUFF_SIZE;

			// 背景の座標を更新
			transform.position = (acc_vec - pre_vec);

			Vector3 pos = Camera.main.transform.position;
			pos.y = -transform.position.y;
			Camera.main.transform.position = pos;

			// じわじわと戻す
			Vector2 dir = acc_vec - pre_vec;
			pre_vec += (dir * 0.005f);
		}
	}

	void OnGUI()
	{
		// テキスト描画
		Vector3 vec = Input.acceleration;

//		if (vec != null) 
		{
			string text;

			text = string.Format("accel.x={0}", vec.x);
			Util.GUILabel(0, Screen.height / 20 * 1, 120, 30, text);

			text = string.Format("accel.y={0}", vec.y);
			Util.GUILabel(0, Screen.height / 20 * 2, 120, 30, text);

			text = string.Format("accel.z={0}", vec.z);
			Util.GUILabel(0, Screen.height / 20 * 3, 120, 30, text);

			text = string.Format("volume={0}", Sound.GetVolumeSe(0));
			Util.GUILabel(0, Screen.height / 20 * 7, 120, 30, text);
		}
	}

	//
	public static void SetParallax(bool state)
	{
		PlayerPrefs.SetInt("Parallax", state ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static bool GetParallax()
	{
		// ※セーブデータがない場合のデフォルト値=false
		return (PlayerPrefs.GetInt("Parallax", 0) == 1);
	}

	//
	public static void SetParaInv(bool state)
	{
		PlayerPrefs.SetInt("ParaInv", state ? 1 : 0);
		PlayerPrefs.Save();
	}

	public static bool GetParaInv()
	{
		// ※セーブデータがない場合のデフォルト値=false
		return (PlayerPrefs.GetInt("ParaInv", 0) == 1);
	}
}
