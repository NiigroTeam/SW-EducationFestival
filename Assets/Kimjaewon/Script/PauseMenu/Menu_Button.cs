using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Button : MonoBehaviour
{
    private GameObject player;
    public void OnClickMenuButton()
    {
        SceneManager.LoadScene(0);
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)Destroy(player);
    }
}
