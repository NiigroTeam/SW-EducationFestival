using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Start_button : MonoBehaviour
{
    private GameObject player;
    public void OnClickStartButton()
    {
        SceneManager.LoadScene("Job_Select");
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)Destroy(player);
    }
    
    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
