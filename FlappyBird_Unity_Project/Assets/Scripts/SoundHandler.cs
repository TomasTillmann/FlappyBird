using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundHandler
{

    public enum Sound { BirdJump, Score, Lose}; public static Sound sound;


    private const int AUDIO_OBJECTS_LIMIT = 20;
    private static List<GameObject> gameObjectAudioList = new List<GameObject>();

    
    public static void PlaySound(Sound sound)
    {
        GameObject gameObject = new GameObject("Sound", typeof(AudioSource));
        gameObjectAudioList.Add(gameObject);
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        switch(sound)
        {
            case (Sound.BirdJump):
                audioSource.PlayOneShot(GameAssets.GetInstance().BirdJump);
                break;
            case (Sound.Score):
                audioSource.PlayOneShot(GameAssets.GetInstance().Score);
                break;
            case (Sound.Lose):
                audioSource.PlayOneShot(GameAssets.GetInstance().Lose);
                break;
        }
        cleanUpAudioObjects();
    }

    /// <summary>
    /// cleans up the first half of created audioObjects
    /// </summary>
    private static void cleanUpAudioObjects()
    {
        if (gameObjectAudioList.Count > AUDIO_OBJECTS_LIMIT)
        {
            Debug.Log("cleanup");

            // cannot delete the last one -> buggy sound. The first half is just convinient.
            for (int i = 0; i < (gameObjectAudioList.Count / 2); i++)
            {
                Object.Destroy(gameObjectAudioList[i]);
                gameObjectAudioList.Remove(gameObjectAudioList[i]);
            }
        }
    }
}
