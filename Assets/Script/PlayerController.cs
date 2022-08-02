using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 targetPosition;
    private Vector3 prePosition;
    [SerializeField] private float tileLength = 1.0f;
    [SerializeField] private float speed = 0.1f;
    [SerializeField] private float movingTriggerTime = 0.2f;
    [SerializeField] private float movingDelayTime = 0.2f;
    private float movingTriggerDeltaTime;
    private float movingDelayDeltaTime;

    private bool buttonFlagUp = false;
    private bool buttonFlagDown = false;
    private bool buttonFlagLeft = false;
    private bool buttonFlagRight = false;

    private bool cannonballShot = false;
    private float shotDeltaTime = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        playerTurn(180.0f);
        this.targetPosition.x = transform.position.x;
        this.targetPosition.y = transform.position.y;
    }


    // Update is called once per frame
    void Update()
    {
        if (cannonballShot)
        {
            shotDeltaTime += Time.deltaTime;

            if (shotDeltaTime > 0.7f)
            {
                cannonballShot = false;
                shotDeltaTime = 0.0f;
            }

            return;
        }
        
        if (Mathf.Approximately(transform.position.x, targetPosition.x) && Mathf.Approximately(transform.position.y, targetPosition.y))
        {
            this.prePosition = transform.position;

            if (Input.GetKeyDown(KeyCode.UpArrow) ^ Input.GetKeyDown(KeyCode.W))
            {
                playerTurn(0.0f);
                this.buttonFlagUp = true;

                this.targetPosition.y += this.tileLength;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) ^ Input.GetKeyDown(KeyCode.S))
            {
                playerTurn(180.0f);
                this.buttonFlagDown = true;

                this.targetPosition.y -= this.tileLength;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) ^ Input.GetKeyDown(KeyCode.A))
            {
                playerTurn(90.0f);
                this.buttonFlagLeft = true;

                this.targetPosition.x -= this.tileLength;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) ^ Input.GetKeyDown(KeyCode.D))
            {
                playerTurn(-90.0f);
                this.buttonFlagRight = true;
                
                this.targetPosition.x += this.tileLength;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, this.targetPosition, this.speed);
        }


        if (this.buttonFlagUp && (Input.GetKey(KeyCode.UpArrow) ^ Input.GetKey(KeyCode.W)))
        {
            moving();
        }
        if (this.buttonFlagDown && (Input.GetKey(KeyCode.DownArrow) ^ Input.GetKey(KeyCode.S)))
        {
            moving();
        }
        if (this.buttonFlagLeft && (Input.GetKey(KeyCode.LeftArrow) ^ Input.GetKey(KeyCode.A)))
        {
            moving();
        }
        if (this.buttonFlagRight && (Input.GetKey(KeyCode.RightArrow) ^ Input.GetKey(KeyCode.D)))
        {
            moving();
        }
        

        /*
        // if direction keys are up
        if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            this.movingTriggerDeltaTime = 0.0f;
        }
        */
    }


    private void playerTurn(float direction)
    {
        transform.rotation = Quaternion.Euler(0, 0, direction);
        buttonFlagFalse();
        this.movingTriggerDeltaTime = 0.0f;
    }


    private void buttonFlagFalse()
    {
        this.buttonFlagUp = false;
        this.buttonFlagDown = false;
        this.buttonFlagLeft = false;
        this.buttonFlagRight = false;
    }


    private void moving()
    {
        this.movingTriggerDeltaTime += Time.deltaTime;
        this.movingDelayDeltaTime += Time.deltaTime;

        if (this.movingTriggerDeltaTime > this.movingTriggerTime)
        {
            if (Mathf.Approximately(transform.position.x, targetPosition.x) && Mathf.Approximately(transform.position.y, targetPosition.y))
            {
                if (this.movingDelayDeltaTime > this.movingDelayTime)
                    setTargetPosition();
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, this.targetPosition, this.speed);

                this.movingDelayDeltaTime = 0.0f;
            }
        }
    }


    private void setTargetPosition()
    {
        if (buttonFlagUp)
            this.targetPosition.y += this.tileLength;
        else if (buttonFlagDown)
            this.targetPosition.y -= this.tileLength;
        else if (buttonFlagLeft)
            this.targetPosition.x -= this.tileLength;
        else if (buttonFlagRight)
            this.targetPosition.x += this.tileLength;
    }


    public void getShot()
    {
        cannonballShot = true;
    }
}
