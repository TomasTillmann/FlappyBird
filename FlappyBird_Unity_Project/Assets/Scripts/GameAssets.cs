using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
