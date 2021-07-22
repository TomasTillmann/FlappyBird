using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// FLAPPY BIRD - interactive game
// Tomas Tillmann, tomas.tilllmann@gmail.com
// august - 2021



public class Score : MonoBehaviour
{
    private Text scoreText;

    private void Start()
    {
        scoreText = GetComponent<Text>();
    }

    private void Update()
    {
        scoreText.text = Level.GetInstance().GetScore().ToString();
    }
}
