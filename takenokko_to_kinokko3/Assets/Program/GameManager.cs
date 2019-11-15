using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject PauseUIPrefab;

    private GameObject PauseUIInstance;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(PauseUIInstance == null)
            {
                PauseUIInstance = GameObject.Instantiate(PauseUIPrefab) as GameObject;
                Time.timeScale = 0f;
            }
            else
            {
                Destroy(PauseUIInstance);
                Time.timeScale = 1f;
            }
        }
    }

   void Pause()
    {
        Time.timeScale = 0f;
    }
}
