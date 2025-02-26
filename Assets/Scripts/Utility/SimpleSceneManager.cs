using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneManager : MonoBehaviour
{
    public static void ChangeScene(string pSceneName)
    {
        SceneManager.LoadScene(pSceneName);
        if ((SceneManager.GetSceneByName(pSceneName)).IsValid())
        {
        }
        else
        {
            Debug.LogError("Scene is not valid");
        }
    }
    public static Scene GetCurrentScene()
    {
        return SceneManager.GetActiveScene();
    }
}
