using UnityEngine;
using System.Collections;

public class SeqFolder : MonoBehaviour
{

	public GameObject obj;
	public float changeFrameSecond;
	public string folderName;
	public string headText;
	public int imageLength;
	private int firstFrameNum;
	private float dTime;

	// Use this for initialization
	void Start()
	{
		firstFrameNum = 1;
		dTime = 0.0f;
	}

	// Update is called once per frame
	void Update()
	{
		dTime += Time.deltaTime;
		if (changeFrameSecond < dTime) {
			dTime = 0.0f;
			firstFrameNum++;
			if (firstFrameNum > imageLength) {
				firstFrameNum = imageLength;
			}
		}
		string sNum = firstFrameNum.ToString().PadLeft(3, '0');
		string path = folderName + "/" + headText + sNum;

		Sprite spr = Resources.Load<Sprite>(path);
		obj.GetComponent<SpriteRenderer>().sprite = spr;
	}

}