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
	AudioSource sourceBgm = null;
	
	// BGM
	AudioSource sourceSeDefault = null;
	
	// SE (デフォルト)
	AudioSource[] sourceSeArray;
	
	// SE (チャンネル)
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
	AudioSource GetAudioSource(eType type, int channel = -1)
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
		GetInstance().LoadBgmProc(key, resName);
	}

	public static void LoadSe(string key, string resName)
	{
		GetInstance().LoadSeProc(key, resName);
	}

	void LoadBgmProc(string key, string resName)
	{
		if (poolBgm.ContainsKey(key)) {
			// すでに登録済みなのでいったん消す
			poolBgm.Remove(key);
		}
		poolBgm.Add(key, new Data(key, resName));
	}

	void LoadSeProc(string key, string resName)
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
		return GetInstance().PlayBgmProc(key, bLoop);
	}

	bool PlayBgmProc(string key, bool bLoop = true)
	{
		if (poolBgm.ContainsKey(key) == false) {
			// 対応するキーがない
			return false;
		}

		// いったん止める
		StopBgmProc();

		// リソースの取得
		var data = poolBgm[key];

		// 再生
		var source = GetAudioSource(eType.Bgm);
		source.loop = bLoop;
		source.clip = data.Clip;
		source.Play();

		return true;
	}

	/// BGMの停止
	public static bool StopBgm()
	{
		return GetInstance().StopBgmProc();
	}

	bool StopBgmProc()
	{
		GetAudioSource(eType.Bgm).Stop();

		return true;
	}

	/// SEの再生
	/// ※事前にLoadSeでロードしておくこと
	public static bool PlaySe(string key, int channel = -1)
	{
		return GetInstance().PlaySeProc(key, channel);
	}

	bool PlaySeProc(string key, int channel = -1)
	{
		if (poolSe.ContainsKey(key) == false) {
			// 対応するキーがない
			return false;
		}

		// リソースの取得
		var data = poolSe[key];

		if (0 <= channel && channel < SE_CHANNEL) {
			// チャンネル指定
			var source = GetAudioSource(eType.Se, channel);
			source.clip = data.Clip;
			source.Play();
		}
		else {
			// デフォルトで再生
			var source = GetAudioSource(eType.Se);
			source.PlayOneShot(data.Clip);
		}

		return true;
	}

	// ボリュームの設定 (0.0～1.0)
	public static void SetVolumeSe(float volume, int channel = -1)
	{
		GetInstance().SetVolumeSeProc(volume, channel);
	}

	void SetVolumeSeProc(float volume, int channel = -1)
	{
		// ボリュームの設定
		GetAudioSourceSe(channel).volume = volume;
	}

	// ボリュームの取得 (0.0～1.0)
	public static float GetVolumeSe(int channel = -1)
	{
		return GetInstance().GetVolumeSeProc(channel);
	}

	float GetVolumeSeProc(int channel = -1)
	{
		// ボリュームの取得
		return GetAudioSourceSe(channel).volume;
	}

	// SEのオーディオソースを取得
	AudioSource GetAudioSourceSe(int channel = -1)
	{
		// ボリュームの設定
		if (0 <= channel && channel < SE_CHANNEL) {
			// チャンネル指定
			return GetAudioSource(eType.Se, channel);
		}

		// デフォルト
		return GetAudioSource(eType.Se);
	}
}
