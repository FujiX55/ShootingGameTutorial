using UnityEngine;
using System.Collections;

public class EndImage : MonoBehaviour
{
	public GameObject sprite;

	// Use this for initialization
	void Start()
	{
		StartCoroutine(FadeSpriteProc());
	}
	
	// Update is called once per frame
	void Update()
	{
	
	}

	IEnumerator FadeSpriteProc()
	{
#if false
		Color spriteColor = sprite.GetComponent<SpriteRenderer>().color;

		spriteColor.a = 0.0f;
		// フェードイン
		while (spriteColor.a < 1) {
			spriteColor.a += Time.deltaTime;
			sprite.GetComponent<SpriteRenderer>().color = 
				new Color(spriteColor.r, spriteColor.g, spriteColor.b, spriteColor.a);
			yield return null;
		}
#endif
		yield return null;
	}
}
