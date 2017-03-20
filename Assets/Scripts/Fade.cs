﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
	float alpha = 0.0f;
	//	GameMgr gameMgr;

	// Use this for initialization
	void Start()
	{
//		gameMgr = GameObject.Find("GameMgr").GetComponent<GameMgr>();
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Time.timeScale != 1.0f) {
			if (alpha < 0.5f) {
				alpha += 0.01f;
			}
			GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, alpha);
			//GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
		}
		else {
			GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			alpha = 0.0f;
//			if (gameMgr) {
//				switch (gameMgr.State) {
//				case GameMgr.eState.GameOver:
//					GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.5f);
//					break;
//				default:
//					GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
//					break;
//				}
//			}

		}
	}
}