using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleUI : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}

	public void TransitSetting()
	{
		// 設定画面へ
		SceneManager.LoadScene("Setting", LoadSceneMode.Additive);
//		SceneManager.LoadScene("Setting");
	}

	public void StartGame()
	{
		// ゲーム開始
		SceneManager.LoadScene("Main");
	}
}
