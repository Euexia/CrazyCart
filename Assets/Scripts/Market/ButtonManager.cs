using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.Events;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioClip buttonClickSound;
    private AudioSource audioSource;   

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }


        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayButtonSound); 
        }
    }

    void PlayButtonSound()
    {
        if (buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound); 
        }
    }
}
