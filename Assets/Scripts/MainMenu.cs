using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private bool canRestart;

    private void Start()
    {
        PlayerManager playerManager = FindObjectOfType<PlayerManager>();

        if(playerManager != null) { Destroy(playerManager.gameObject); }
    }

    private void Update()
    {
        if (canRestart && Input.anyKeyDown)
        {
            SceneManager.LoadScene(0);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
