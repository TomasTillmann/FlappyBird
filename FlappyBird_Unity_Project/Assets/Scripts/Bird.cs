using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using CodeMonkey;

public class Bird : MonoBehaviour
{
    public static Bird GetInstance()
    {
        return instance;
    }

    public event EventHandler OnDied;


    private static Bird instance;

    private const float JUMP_FACTOR = 80f;
    private const float MIN_VELOCITY = -120f;
    private const float MAX_VELOCITY = 80f; 

    private Rigidbody2D birdRigidBody2D;
    private Transform birdTransform;

    private readonly Quaternion START_ROTATION = Quaternion.Euler(0, 0, 30f);       // upper bound of bird's rotation
    private readonly Quaternion END_ROTATION = Quaternion.Euler(0, 0, -40f);        // lower bound of bird's rotation

    private void Awake()
    {
        birdRigidBody2D = GetComponent<Rigidbody2D>();
        birdTransform = GetComponent<Transform>();

        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            jump();
        }
        rotate();
    }

    /// <summary>
    /// called when bird's collider hits another collider with isTrigger set
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // makes the bird static - immovable
        birdRigidBody2D.bodyType = RigidbodyType2D.Static;

        // if (OnDied != Null) ...  
        OnDied?.Invoke(this, EventArgs.Empty);
    }

    private void jump()
    {
        birdRigidBody2D.velocity = new Vector2(0, JUMP_FACTOR);
        SoundHandler.PlaySound(SoundHandler.Sound.BirdJump);
    }

    private void rotate()
    {
        float birdVelocity = birdRigidBody2D.velocity.y;

        // Normalize birdVelocity between 0 and 1. Substracting from one in order to reverse the rotation.
        birdVelocity = 1 - Mathf.InverseLerp(MIN_VELOCITY, MAX_VELOCITY, birdVelocity);

        birdTransform.rotation = Quaternion.Slerp(START_ROTATION, END_ROTATION, birdVelocity);
    }

}
