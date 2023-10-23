using UnityEngine;
using UnityEngine.UI;

public class WinScreenManager : MonoBehaviour
{
    public GameObject winScreen; 
    public GameObject loseScreen;
    public AudioSource winSound;
    private void Start()
    {
        
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
        winSound.Play();
    }

    public void ShowLoseScreen()
    {
        loseScreen.SetActive(true);
        winSound.Play();
    }
}
