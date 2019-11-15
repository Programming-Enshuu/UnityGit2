using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    public void Stage1()
    {
        Debug.Log("Stage1");
        SceneManager.LoadScene("Stage1");
    }
    public void Stage2()
    {
        Debug.Log("Stage2");
        SceneManager.LoadScene("Stage2");
    }
    public void Stage3()
    {
        Debug.Log("Stage3");
        SceneManager.LoadScene("Stage3");
    }
    public void Stage4()
    {
        Debug.Log("Stage4");
        SceneManager.LoadScene("Stage4");
    }
    public void Stage5()
    {
        Debug.Log("Stage5");
        SceneManager.LoadScene("Stage5");
    }
}
