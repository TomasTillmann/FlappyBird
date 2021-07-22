using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// FLAPPY BIRD - interactive game
// Tomas Tillmann, tomas.tilllmann@gmail.com
// august - 2021



public static class SoundHandler
{
    public enum Sound { BirdJump, Score, Lose}; public static Sound sound;


    private const int AUDIO_OBJECTS_LIMIT = 20;

    private static List<GameObject> gameObjectAudioList = new List<GameObject>();


    public static void PlaySound(Sound sound)
    {
        GameObject gameObject = new GameObject("Sound", typeof(AudioSource));
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();

        gameObjectAudioList.Add(gameObject);

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
    /// Cleans up the first half of created audioObjects
    /// </summary>
    private static void cleanUpAudioObjects()
    {
        if (gameObjectAudioList.Count > AUDIO_OBJECTS_LIMIT)
        {
            // Cannot delete the last one -> buggy sound. Deleting the first half is just convinient.
            for (int i = 0; i < (gameObjectAudioList.Count / 2); i++)
            {
                Object.Destroy(gameObjectAudioList[i]);
                gameObjectAudioList.Remove(gameObjectAudioList[i]);
            }
        }
    }
}
