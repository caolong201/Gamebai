using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using DG.Tweening;

public class SceneLoadManager 
{

	private static string mLastScene;

	public static void LoadLevel(string scene)
	{
		//UIManager.Instance.HideAllDialog();
		LastScene = SceneManager.GetActiveScene ().name;
		Debug.Log("LoadLevel " + scene);
		SceneManager.LoadScene(scene, LoadSceneMode.Single);

	}

	public static IEnumerator LoadLevelAsync(string scene, Action callback = null)
	{
//		yield return new WaitForSeconds(20);

		DOTween.Clear(true);
        DOTween.KillAll();
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
			yield return null;
		}

		yield return new WaitForSeconds(0.2f);
		callback?.Invoke();
	}    

    public static void ReLoadLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().name);
    }

    public static string GetActiveScene()
    {
       return SceneManager.GetActiveScene().name;
    }

	public static string LastScene
	{
		private set{ mLastScene = value; }
		get{ return  mLastScene;}
	}
		
}

public class SceneName
{
	public const string Loading = "LoadingScene";
	public const string Title = "TitleScene";
    public const string GamePlay = "GameScene";
}