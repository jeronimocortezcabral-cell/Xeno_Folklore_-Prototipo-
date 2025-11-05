using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Jugar() 
        {
        Debug.Log("se iso clik");
        SceneManager.LoadScene("MainScene");
    }
    
    // Update is called once per frame
    void Update()
    {



        
    }
}

