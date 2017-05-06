using UnityEngine;
using System.Collections;

public class SoundMgr : SingletonMonoBehaviour<SoundMgr>
{
	float bgmTime = 0.0f;
	string bgmNext = "";

	/// 開始
	void Awake()
	{
		if (this != Instance) {
			Destroy(this);
			return;
		}
//		DontDestroyOnLoad(this.gameObject);
	}

	/// <summary>
	/// サウンド管理
	/// </summary>
	void Start()
	{
		// サウンドをロード
		// "bgm01_intro"をロード　キーは"intro"とする
		Sound.LoadBgm("intro", "bgm01_intro");
	
		// "bgm01"をロード　キーは"loop"とする
		Sound.LoadBgm("loop", "bgm01");

		// "ジングル01"をロード　キーは"jingle"とする
		Sound.LoadBgm("jingle", "ジングル01");

		// "ごびおーーーん(爆発音)"をロード　キーは"damage"とする
		Sound.LoadSe("damage", "ごびおーーーん(爆発音)");
        
		// "ドぇン(爆発音)"をロード　キーは"destroy"とする
		Sound.LoadSe("destroy", "ドぇン(爆発音)");    

		// "0000_さとうささら_おしまい"をロード　キーは"oshimai"とする
		Sound.LoadSe("oshimai", "0000_さとうささら_おしまい");    

		// "0001_さとうささら_はい"をロード　キーは"hai"とする
		Sound.LoadSe("hai", "0001_さとうささら_はい");    

		// "0002_さとうささら_キャッ"をロード　キーは"kya"とする
		Sound.LoadSe("kya", "0002_さとうささら_キャッ");    

		// "0003_さとうささら_アンッ"をロード　キーは"ann"とする
		Sound.LoadSe("ann", "0003_さとうささら_アンッ");    

		// "0004_さとうささら_はじまりー"をロード　キーは"hajimari"とする
		Sound.LoadSe("hajimari", "0004_さとうささら_はじまりー");

		// "0005_さとうささら_ヤッタネ"をロード　キーは"yattane"とする
		Sound.LoadSe("yattane", "0005_さとうささら_ヤッタネ");
	}

	public static void PlayBgm(string bgmIntro, string bgmLoop)
	{
		SoundMgr mgr = Instance;

		Sound.PlayBgm(bgmIntro, false);

		mgr.bgmNext = bgmLoop;	
		mgr.bgmTime = Time.time + Sound.GetBgmLength();
	}

	void Update()
	{
		if (bgmNext != "") {
			if ((bgmTime <= Time.time) || (!Sound.IsBgmPlaying())) {
				Sound.PlayBgm(bgmNext);
				bgmNext = "";
				bgmTime = 0.0f;
			}
		}
	}

	public static void PlayBgm(string key, bool bLoop = true)
	{
		SoundMgr mgr = Instance;

		mgr.bgmNext = "";	
		mgr.bgmTime = 0.0f;

		Sound.PlayBgm(key, bLoop);
	}

	public static void StopBgm()
	{
		SoundMgr mgr = Instance;

		mgr.bgmNext = "";	
		mgr.bgmTime = 0.0f;

		Sound.StopBgm();
	}
}
