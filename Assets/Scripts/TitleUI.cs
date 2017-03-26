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
		// タイトルへ戻る
		SceneManager.LoadScene("Setting");
	}

	public void StartGame()
	{
		// タイトルへ戻る
		SceneManager.LoadScene("Main");
	}
}
