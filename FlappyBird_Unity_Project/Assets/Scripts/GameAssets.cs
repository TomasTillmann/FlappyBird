using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// FLAPPY BIRD - interactive game
// Tomas Tillmann, tomas.tilllmann@gmail.com
// august - 2021



/// <summary>
/// All assets in one place, accessible through this class using GetInstance().
/// </summary>
public class GameAssets : MonoBehaviour
{
    public Transform pfPipeBody;
    public Transform pfPipeHead;
    public Transform pfGround;
    public Transform pfClouds;

    public AudioClip BirdJump;
    public AudioClip Score;
    public AudioClip Lose;
    
    public static GameAssets GetInstance()
    {
        return instance;
    }


    private static GameAssets instance;

    private void Awake()
    {
        instance = this;
    }
}
