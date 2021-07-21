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
                    // resets GameScene
                    UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
                }
                break;
        }
    }

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

        private const float CLOUD_HEIGHT = -3f;
        private const float CLOUD_OFF_SCREEN_POSITION = -320f;
        private const float CLOUD_1_INIT_POSITION = -95f;
        private const float CLOUD_2_INIT_POSITION = 300f;
        private const float CLOUD_RIGHT_OFFSET = 170;
        private const float CLOUD_MOVING_SPEED = 20f;  // HACK / little less than ground and pipe moving -> parallex effect
        public CloudHandler()
        {
            clouds_1 = Instantiate(GameAssets.GetInstance().pfClouds);
            clouds_1.position = new Vector3(CLOUD_1_INIT_POSITION, CLOUD_HEIGHT, 0);

            clouds_2 = Instantiate(GameAssets.GetInstance().pfClouds);
            clouds_2.position = new Vector3(CLOUD_2_INIT_POSITION, CLOUD_HEIGHT, 0);

            currentClouds = clouds_1;
        }

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

        private const float GROUND_HEIGHT = -49.1f;
        private const float GROUND_1_INIT_POSITION = -122.5f;
        private const float GROUND_2_INIT_POSITION = 115f;

        private const float GROUND_OFF_SCREEN_POSITION = -360f;
        private const float GROUND_MOVING_SPEED = 26f;          // HACK

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
        public ScoreHandler scoreHandler = new ScoreHandler();

        private const float BIRD_POSITION = 0f;

        private const float CANVAS_SIZE = 100f;
        private const float CAMERA_ORT_SIZE = 50f;

        private const float PIPE_WIDTH = 7.8f;
        private const float PIPE_HEAD_HEIGHT = 3.75f;

        private const float PIPES_MOVING_SPEED = 26f;

        private const float PIPE_SPAWNING_TIME = 1.4f;

        private const float PIPE_OFF_SCREEN_POSITION = -130f;
        private const float PIPE_SPAWN_POSITION = 116f;

        private const float PIPE_MIN_GAP_SIZE = 20f;
        private const float PIPE_MAX_GAP_SIZE = 25;

        private const float PIPE_EDGE_OFFSET = 10f;


        private List<PipeSet> pipeSetList = new List<PipeSet>();

        private Repeater repeaterSpawnPipes = new Repeater(PIPE_SPAWNING_TIME);

        private float pipesMovingSpeed = PIPES_MOVING_SPEED;


        public void SpawnPipes()
        {
            if(repeaterSpawnPipes.IsInterval())
            {
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

                pipeSet.Move(pipesMovingSpeed);

                // if the pipe was right to the bird and now, after moving it, is on the left
                if (isPipeRight && pipeSet.GetPosition() <= BIRD_POSITION)
                {
                    //Debug.Log(score);
                    scoreHandler.Update();
                }

                if (pipeSet.GetPosition() < PIPE_OFF_SCREEN_POSITION)
                {
                    pipeSet.DestroySelf();
                    pipeSetList.Remove(pipeSet);

                    // in order not to skip any pipes 
                    i--;
                }
            }
        }


        /// <summary>
        /// generates a pipe with a gap.
        /// </summary>
        private void createPipeSet(float xPosition, float heightBottomPipe, float gapSize)
        {
            // heightBottomPipe can be think of as the y position of the gap from the bottom of the canvas ( not from (0,0,0) )
            float heightTopPipe = CANVAS_SIZE - heightBottomPipe - gapSize;

            // creates pipe facing up
            createPipe(xPosition, heightBottomPipe, false);

            // creates pipe facing down
            createPipe(xPosition, heightTopPipe, true);
        }

        private void createPipe(float xPosition, float h, bool up) 
        {
            float yAxisPipe;
            float yAxisHead; 

            // making clones of our prefabs - procedural generation
            Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
            Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);


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


            // get a reference on sprite renderer component of pipeBody
            SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
            pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, h);


            // get a reference on pipe box collider 
            BoxCollider2D pipeBoxCollider = pipeBody.GetComponent<BoxCollider2D>();

            // change box collider's size to the same as the pipe's size
            pipeBoxCollider.size = new Vector2(PIPE_WIDTH, h);


            // change the offset - pipe's pivot is from the bottom. Box collider's pivot is in the center -> h / 2.
            pipeBoxCollider.offset = new Vector2(0f, h / 2);


            // puts together pipe head and pipe body in one entity - pipe set
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
            score += 0.5f;
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
        ///  deletes itself ( as an gameObject ) from the scene
        /// </summary>
        public void DestroySelf()
        {
            Destroy(pipeHead.gameObject);
            Destroy(pipeBody.gameObject);
        }
    }


    public class Repeater
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
