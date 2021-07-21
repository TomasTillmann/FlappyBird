using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using CodeMonkey;

public class Bird : MonoBehaviour
{

    public event EventHandler OnDied;

    public static Bird GetInstance()
    {
        return instance;
    }


    private static Bird instance;

    private Rigidbody2D birdRigidBody2D;
    private Transform birdTransform;

    private const float JUMP_FACTOR = 80f;
    private const float MIN_VELOCITY = -120f;            // -140           
    private const float MAX_VELOCITY = 80f; 

    private readonly Quaternion START_ROTATION = Quaternion.Euler(0, 0, 30f);
    private readonly Quaternion END_ROTATION = Quaternion.Euler(0, 0, -40f);

    private void Awake()
    {
        birdRigidBody2D = GetComponent<Rigidbody2D>();
        birdTransform = GetComponent<Transform>();

        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            jump();
        }
        rotate();
    }

    private void jump()
    {
        birdRigidBody2D.velocity = new Vector2(0, JUMP_FACTOR);
        SoundHandler.PlaySound(SoundHandler.Sound.BirdJump);
    }

    private void rotate()
    {
        float birdVelocity = birdRigidBody2D.velocity.y;

        // Normalize birdVelocity between 0 and 1. 
        birdVelocity = 1 - Mathf.InverseLerp(MIN_VELOCITY, MAX_VELOCITY, birdVelocity);

        birdTransform.rotation = Quaternion.Slerp(START_ROTATION, END_ROTATION, birdVelocity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        birdRigidBody2D.bodyType = RigidbodyType2D.Static;

        OnDied?.Invoke(this, EventArgs.Empty);

    }
}
