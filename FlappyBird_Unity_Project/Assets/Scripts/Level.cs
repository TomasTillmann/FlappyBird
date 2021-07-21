using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level GetInstance()
    {
        return instance;
    }

    public float GetScore()
    {
        return pipeHandler.scoreHandler.score;
    }

    // CONSTANTS CLOUDS
    private const float CLOUD_HEIGHT                    = -3f;
    private const float CLOUD_OFF_SCREEN_POSITION       = -320f;
    private const float CLOUD_1_INIT_POSITION           = -95f;
    private const float CLOUD_2_INIT_POSITION           = 300f;
    private const float CLOUD_RIGHT_OFFSET              = 170f;
    private const float CLOUD_MOVING_SPEED              = 20f;  

    // CONSTANTS GROUND
    private const float GROUND_HEIGHT                   = -49.1f;
    private const float GROUND_1_INIT_POSITION          = -122.5f;
    private const float GROUND_2_INIT_POSITION          = 115f;

    private const float GROUND_OFF_SCREEN_POSITION      = -360f;
    private const float GROUND_MOVING_SPEED             = 26f;

    // CONSTANTS PIPES
    private const float BIRD_POSITION                   = 0f;

    private const float CANVAS_SIZE                     = 100f;
    private const float CAMERA_ORT_SIZE                 = 50f;

    private const float PIPE_WIDTH                      = 7.8f;
    private const float PIPE_HEAD_HEIGHT                = 3.75f;

    private const float PIPES_MOVING_SPEED              = 26f;

    private const float PIPE_SPAWNING_TIME              = 1.4f;

    private const float PIPE_OFF_SCREEN_POSITION        = -130f;
    private const float PIPE_SPAWN_POSITION             = 116f;

    private const float PIPE_MIN_GAP_SIZE               = 20f;
    private const float PIPE_MAX_GAP_SIZE               = 25;

    private const float PIPE_EDGE_OFFSET                = 10f;



    private static Level instance;

    private PipeHandler pipeHandler;
    private GroundHandler groundHandler;
    private CloudHandler cloudHandler;


    private enum State { Alive, Dead }; private State currentState;

    private void Awake()
    {
        pipeHandler = new PipeHandler();
        groundHandler = new GroundHandler();
        cloudHandler = new CloudHandler();

        instance = this;
        currentState = State.Alive;
    }

    private void Start()
    {
        // Subscribes to Bird's OnDied Event, in order to stop the scene from moving (ground, pipes, clouds) when the bird dies (hits a collider)
        Bird.GetInstance().OnDied += level_OnDied;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Alive:
                pipeHandler.MovePipes();
                pipeHandler.SpawnPipes();
                groundHandler.MoveGround();
                cloudHandler.MoveClouds();
                break;

            case State.Dead:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                { 
                    // Resets GameScene
                    UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                }
                break;
        }
    }

    /// <summary>
    /// Gets called when bird dies. Triggered by event in Bird class.
    /// </summary>
    private void level_OnDied(object sender, System.EventArgs e)
    {
        currentState = State.Dead;
        SoundHandler.PlaySound(SoundHandler.Sound.Lose);
    }

    private class CloudHandler
    {
        private Transform clouds_1;
        private Transform clouds_2;
        private Transform currentClouds;


        /// <summary>
        /// Instantiates clouds on to the scene and sets up their init positions.
        /// </summary>
        public CloudHandler()
        {
            clouds_1 = Instantiate(GameAssets.GetInstance().pfClouds);
            clouds_1.position = new Vector3(CLOUD_1_INIT_POSITION, CLOUD_HEIGHT, 0);

            clouds_2 = Instantiate(GameAssets.GetInstance().pfClouds);
            clouds_2.position = new Vector3(CLOUD_2_INIT_POSITION, CLOUD_HEIGHT, 0);

            currentClouds = clouds_1;
        }

        /// <summary>
        /// Moves the clouds to the left. Makes sure the clouds will move 
        /// on the scene correctly and correctly reposition offscreen. 
        /// </summary>
        public void MoveClouds()
        {
            clouds_1.position += new Vector3(-1, 0, 0) * CLOUD_MOVING_SPEED * Time.deltaTime;
            clouds_2.position += new Vector3(-1, 0, 0) * CLOUD_MOVING_SPEED * Time.deltaTime;

            if (currentClouds.position.x < CLOUD_OFF_SCREEN_POSITION)
            {
                currentClouds.position = new Vector3(CLOUD_2_INIT_POSITION + CLOUD_RIGHT_OFFSET, CLOUD_HEIGHT, 0);
                if (currentClouds == clouds_1)
                {
                    currentClouds = clouds_2;
                }
                else
                {
                    currentClouds = clouds_1;
                }
            }
        }
    }


    private class GroundHandler
    {
        private Transform ground_1;
        private Transform ground_2;
        private Transform currentGround;


        public GroundHandler()
        {
            ground_1 = Instantiate(GameAssets.GetInstance().pfGround);
            ground_1.position = new Vector3(GROUND_1_INIT_POSITION, GROUND_HEIGHT, 0);

            ground_2 = Instantiate(GameAssets.GetInstance().pfGround);
            ground_2.position = new Vector3(GROUND_2_INIT_POSITION, GROUND_HEIGHT, 0);

            currentGround = ground_1;
        }

        public void MoveGround()
        {

            ground_1.position += new Vector3(-1, 0, 0) * GROUND_MOVING_SPEED * Time.deltaTime;
            ground_2.position += new Vector3(-1, 0, 0) * GROUND_MOVING_SPEED * Time.deltaTime;

            if(currentGround.position.x < GROUND_OFF_SCREEN_POSITION)
            {
                currentGround.position = new Vector3(GROUND_2_INIT_POSITION, GROUND_HEIGHT, 0);
                if (currentGround == ground_1)
                {
                    currentGround = ground_2;
                }
                else
                {
                    currentGround = ground_1;
                }
            }
        }
    }

    private class PipeHandler
    {
        public ScoreHandler scoreHandler;


        private List<PipeSet> pipeSetList;

        private Repeater repeaterSpawnPipes;


        public PipeHandler()
        {
            pipeSetList = new List<PipeSet>();
            scoreHandler = new ScoreHandler();
            repeaterSpawnPipes = new Repeater(PIPE_SPAWNING_TIME);
        }


        public void SpawnPipes()
        {
            if(repeaterSpawnPipes.IsInterval())
            {
                // The height of the pipe gap is between PIPE_EDGE_OFFSET from the bottom as well from the top.
                float height = Random.Range(PIPE_EDGE_OFFSET, CANVAS_SIZE - PIPE_EDGE_OFFSET - ((PIPE_MIN_GAP_SIZE + PIPE_MAX_GAP_SIZE)/2));

                float pipeGapSize = Random.Range(PIPE_MIN_GAP_SIZE, PIPE_MAX_GAP_SIZE);
                createPipeSet(PIPE_SPAWN_POSITION, height, pipeGapSize);
            }
        }

        public void MovePipes()
        {
            for (int i = 0; i < pipeSetList.Count; i++)
            {
                PipeSet pipeSet = pipeSetList[i];

                bool isPipeRight = pipeSet.GetPosition() > BIRD_POSITION;

                pipeSet.Move(PIPES_MOVING_SPEED);

                // If the pipe was right to the bird and now, after moving it, is on the left
                if (isPipeRight && pipeSet.GetPosition() <= BIRD_POSITION)
                {
                    // The bird just passed between pipes. The score is increased.
                    scoreHandler.Update();
                }

                // If the pipe is offscreen
                if (pipeSet.GetPosition() < PIPE_OFF_SCREEN_POSITION)
                {
                    pipeSet.DestroySelf();
                    pipeSetList.Remove(pipeSet);

                    // In order not to skip any pipes 
                    i--;
                }
            }
        }


        /// <summary>
        /// Generates a pipe with a gap.
        /// </summary>
        private void createPipeSet(float xPosition, float heightBottomPipe, float gapSize)
        {
            // HeightBottomPipe can be think of as the y position of the gap from the bottom of the canvas ( not from (0,0,0) )
            float heightTopPipe = CANVAS_SIZE - heightBottomPipe - gapSize;

            // Creates pipe facing up
            createPipe(xPosition, heightBottomPipe, false);

            // Creates pipe facing down
            createPipe(xPosition, heightTopPipe, true);
        }

        private void createPipe(float xPosition, float h, bool up) 
        {
            float yAxisPipe;
            float yAxisHead; 

            // Making clones of our prefabs. Putting it on the scene.
            Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
            Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);


            // Should the pipe be placed upwards or downwards?
            if (up)
            {
                yAxisPipe = CAMERA_ORT_SIZE - h + 1;
                yAxisHead = CAMERA_ORT_SIZE - h + (PIPE_HEAD_HEIGHT / 2);
            }
            else
            {
                yAxisPipe = -CAMERA_ORT_SIZE;
                yAxisHead = -CAMERA_ORT_SIZE + h - (PIPE_HEAD_HEIGHT / 2);
            }

            pipeBody.position = new Vector2(xPosition, yAxisPipe);
            pipeHead.position = new Vector2(xPosition, yAxisHead);


            SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
            // Changes the height of the pipe accordingly. Width is always the same.
            pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, h);


            BoxCollider2D pipeBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
            // Change box collider's size to the same as the pipe's size
            pipeBoxCollider.size = new Vector2(PIPE_WIDTH, h);


            // Change the offset - pipe's pivot is from the bottom. Box collider's pivot is in the center -> h / 2.
            pipeBoxCollider.offset = new Vector2(0f, h / 2);


            // Puts together pipe head and pipe body in one entity - pipe set
            PipeSet pipeSet = new PipeSet(pipeHead, pipeBody);
            pipeSetList.Add(pipeSet);
        }
    }

    private class ScoreHandler
    {
        public float score;
        public ScoreHandler()
        {
            score = 0;
        }
        public void Update()
        {
            // The bird passes by two pipeSets (pipeBody + pipeHead) every time the score should be increased by one. Therefore we increase by 0.5.
            score += 0.5f;

            // Plays the score sound.
            SoundHandler.PlaySound(SoundHandler.Sound.Score);
        }
    }

    private class PipeSet
    {
        private Transform pipeHead;
        private Transform pipeBody;


        public PipeSet(Transform pipeHead, Transform pipeBody)
        {
            this.pipeHead = pipeHead;
            this.pipeBody = pipeBody;
        }

        public void Move(float speed)
        {
            pipeHead.position += new Vector3(-1, 0, 0) * speed * Time.deltaTime;
            pipeBody.position += new Vector3(-1, 0, 0) * speed * Time.deltaTime;
        }

        public float GetPosition()
        {
            return pipeHead.position.x;
        }

        /// <summary>
        ///  Deletes itself ( as an gameObject ) from the scene
        /// </summary>
        public void DestroySelf()
        {
            Destroy(pipeHead.gameObject);
            Destroy(pipeBody.gameObject);
        }
    }

    private class Repeater
    {
        private float timeInterval;
        private float tempTime; 

        public Repeater(float timeInterval)
        {
            this.timeInterval = timeInterval;
        }

        public bool IsInterval()
        {
            tempTime -= Time.deltaTime;
            if (tempTime < 0)
            {
                tempTime = timeInterval;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
