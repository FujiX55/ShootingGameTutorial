using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// サウンド管理
public class Sound
{

	/// SEチャンネル数
	const int SE_CHANNEL = 4;

	/// サウンド種別
	enum eType
	{
		// BGM
		Bgm,
		// SE
		Se,
	}

	// シングルトン
	static Sound Instance = null;
	// インスタンス取得
	public static Sound GetInstance()
	{
		return Instance ?? (Instance = new Sound());
	}

	// サウンド再生のためのゲームオブジェクト
	GameObject obj = null;
	
	// サウンドリソース
	// BGM
	AudioSource sourceBgm = null;
	
	// SE (デフォルト)
	AudioSource sourceSeDefault = null;
	
	// SE (チャンネル)
	AudioSource[] sourceSeArray;
	
	// BGMにアクセスするためのテーブル
	Dictionary<string, Data> poolBgm = new Dictionary<string, Data>();
	// SEにアクセスするためのテーブル
	Dictionary<string, Data> poolSe = new Dictionary<string, Data>();

	/// 保持するデータ
	class Data
	{
		/// アクセス用のキー
		public string Key;
		/// リソース名
		public string ResName;
		/// AudioClip
		public AudioClip Clip;

		/// コンストラクタ
		public Data(string key, string res)
		{
			Key = key;
			ResName = "Sounds/" + res;
			// AudioClipの取得
			Clip = Resources.Load(ResName) as AudioClip;
		}
	}

	/// コンストラクタ
	public Sound()
	{
		// チャンネル確保
		sourceSeArray = new AudioSource[SE_CHANNEL];
	}

	/// AudioSourceを取得する
	AudioSource privateGetAudioSource(eType type, int channel = -1)
	{
		if (obj == null) {
			// GameObjectがなければ作る
			obj = new GameObject("Sound");
			
			// 破棄しないようにする
			GameObject.DontDestroyOnLoad(obj);
			
			// AudioSourceを作成
			sourceBgm = obj.AddComponent<AudioSource>();
			sourceSeDefault = obj.AddComponent<AudioSource>();
			
			for (int i = 0; i < SE_CHANNEL; i++) {
				sourceSeArray[i] = obj.AddComponent<AudioSource>();
			}
		}

		if (type == eType.Bgm) {
			// BGM
			return sourceBgm;
		}
		else {
			// SE
			if (0 <= channel && channel < SE_CHANNEL) {
				// チャンネル指定
				return sourceSeArray[channel];
			}
			else {
				// デフォルト
				return sourceSeDefault;
			}
		}
	}

	// サウンドのロード
	// ※Resources/Soundsフォルダに配置すること
	public static void LoadBgm(string key, string resName)
	{
		GetInstance().privateLoadBgm(key, resName);
	}

	public static void LoadSe(string key, string resName)
	{
		GetInstance().privateLoadSe(key, resName);
	}

	void privateLoadBgm(string key, string resName)
	{
		if (poolBgm.ContainsKey(key)) {
			// すでに登録済みなのでいったん消す
			poolBgm.Remove(key);
		}
		poolBgm.Add(key, new Data(key, resName));
	}

	void privateLoadSe(string key, string resName)
	{
		if (poolSe.ContainsKey(key)) {
			// すでに登録済みなのでいったん消す
			poolSe.Remove(key);
		}
		poolSe.Add(key, new Data(key, resName));
	}

	/// BGMの再生
	/// ※事前にLoadBgmでロードしておくこと
	public static bool PlayBgm(string key, bool bLoop = true)
	{
		return GetInstance().privatePlayBgm(key, bLoop);
	}

	bool privatePlayBgm(string key, bool bLoop = true)
	{
		if (poolBgm.ContainsKey(key) == false) {
			// 対応するキーがない
			return false;
		}

		// いったん止める
		privateStopBgm();

		// リソースの取得
		var data = poolBgm[key];

		// 再生
		var source = privateGetAudioSource(eType.Bgm);
		source.loop = bLoop;
		source.clip = data.Clip;
		source.Play();

		return true;
	}

	/// BGMの停止
	public static bool StopBgm()
	{
		return GetInstance().privateStopBgm();
	}

	bool privateStopBgm()
	{
		privateGetAudioSource(eType.Bgm).Stop();

		return true;
	}

	public static float GetBgmLength()
	{
		return GetInstance().privateGetBgmLength();
	}

	float privateGetBgmLength()
	{
		var source = privateGetAudioSource(eType.Bgm);

		return source.clip.length;
	}

	public static bool IsBgmPlaying()
	{
		return GetInstance().privateIsBgmPlaying();
	}

	bool privateIsBgmPlaying()
	{
		var source = privateGetAudioSource(eType.Bgm);

		return source.isPlaying;
	}

	/// SEの再生
	/// ※事前にLoadSeでロードしておくこと
	public static bool PlaySe(string key, int channel = -1)
	{
		return GetInstance().privatePlaySe(key, channel);
	}

	bool privatePlaySe(string key, int channel = -1)
	{
		if (poolSe.ContainsKey(key) == false) {
			// 対応するキーがない
			return false;
		}

		// リソースの取得
		var data = poolSe[key];

		if (0 <= channel && channel < SE_CHANNEL) {
			// チャンネル指定
			var source = privateGetAudioSource(eType.Se, channel);
			source.clip = data.Clip;
			source.Play();
		}
		else {
			// デフォルトで再生
			var source = privateGetAudioSource(eType.Se);
			source.PlayOneShot(data.Clip);
		}

		return true;
	}

	// ボリュームの設定 (0.0～1.0)
	public static void SetVolumeSe(float volume, int channel = -1)
	{
		GetInstance().privateSetVolumeSe(volume, channel);
	}

	void privateSetVolumeSe(float volume, int channel = -1)
	{
		// ボリュームの設定
		privateGetAudioSourceSe(channel).volume = volume;
	}

	// ボリュームの取得 (0.0～1.0)
	public static float GetVolumeSe(int channel = -1)
	{
		return GetInstance().privateGetVolumeSe(channel);
	}

	float privateGetVolumeSe(int channel = -1)
	{
		// ボリュームの取得
		return privateGetAudioSourceSe(channel).volume;
	}

	// SEのオーディオソースを取得
	AudioSource privateGetAudioSourceSe(int channel = -1)
	{
		// ボリュームの設定
		if (0 <= channel && channel < SE_CHANNEL) {
			// チャンネル指定
			return privateGetAudioSource(eType.Se, channel);
		}

		// デフォルト
		return privateGetAudioSource(eType.Se);
	}
}
