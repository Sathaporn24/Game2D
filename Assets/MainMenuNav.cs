using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class MainMenuNavigate : MonoBehaviour
{
    public void play()
    {
        SceneManager.LoadSceneAsync(1);
    }

}