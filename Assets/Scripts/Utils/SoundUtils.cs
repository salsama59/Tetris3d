using UnityEngine;

public class SoundUtils : MonoBehaviour {

    public static void StopSound(string soundGameObjectTag)
    {
        GameObject soundController = GameObject.FindGameObjectWithTag(soundGameObjectTag);
        AudioSource source = soundController.GetComponent<AudioSource>();
        if (source.isPlaying)
        {
            source.Stop();
        }
    }


    public static void PlaySound(string soundGameObjectTag)
    {
        GameObject soundController = GameObject.FindGameObjectWithTag(soundGameObjectTag);
        AudioSource source = soundController.GetComponent<AudioSource>();
        if (!source.isPlaying)
        {
            source.Play();
        }
    }

}
