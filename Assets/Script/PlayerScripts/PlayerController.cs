using TileScripts;
using Script.ObjectScripts;
using UnityEngine;
using System.Linq;
using System.Net.NetworkInformation;

namespace PlayerScripts
{
    public class PlayerController : MonoBehaviour
    {
        private Vector3 targetPosition;
        private Vector3 prePosition;
        [SerializeField] private float tileLength = 0.83f;
        [SerializeField] private float speed;
        [SerializeField] private float movingTriggerTime;
        [SerializeField] private float movingDelayTime;
        private float movingTriggerDeltaTime;
        private float movingDelayDeltaTime;
        private float movingSpeed;
        private float moveSpeed;
        private bool isMoving;

        private bool isShiftDown;
        private bool isAcceleration;
        [SerializeField] private float accelDelayTime;
        [SerializeField] private float accelDuration;
        private float accelDurationDeltaTime;
        private float playerAccelDelayTime;
        private float playerMovingDelayTime;

        private float delayDeltaTime;

        private float turnDelayTime;
        private float turnDelayDeltaTime;

        private bool keyDownFlag;
        private bool buttonFlagUp;
        private bool buttonFlagDown;
        private bool buttonFlagLeft;
        private bool buttonFlagRight;

        private bool cannonballHit;
        private float hitDeltaTime;
        private float stunTime;

        [SerializeField] private bool isObstruct;

        private GameObject frontObject;
        public GameObject RoadTile;
        public GameObject Anchor;
        private bool isExistFrontObject;
        private bool isInteracting;
        private bool frontIsBreakable;
        private float interactDelayTime;

        [SerializeField] float breakableDelayTime;
        [SerializeField] float turretDelayTime;
        [SerializeField] float portalDelayTime;
        [SerializeField] float holeDelayTime;
        [SerializeField] float tileDelayTIme;
        [SerializeField] float slowTileDelayTime;
        private float breakDelayTime;
        private float breakDelayDeltaTime;
        
        private CheckBoolUp Up;
        private CheckBoolRight Right;
        private CheckBoolDown Down;
        private CheckBoolLeft Left;

        private PlayerAnchor _playerAnchor;
        
        private readonly bool[] Direction = {false, false, false, false, false};
        private bool isTileAround;
        private bool cannotMove;
        
    
        // Start is called before the first frame update
        private void Start()
        {
            this.moveSpeed = 0.1f;
            this.movingSpeed = 0.2f;
            this.speed = this.moveSpeed;
            this.movingTriggerTime = 0.2f;
            this.movingDelayTime = 0.13f;
            this.playerMovingDelayTime = this.movingDelayTime;
            this.turnDelayTime = 0.05f;
            this.tileDelayTIme = 0.3f;

            this.accelDelayTime = 0.5f;
            this.accelDuration = 30.0f;
            this.playerAccelDelayTime = 0.06f;

            this.stunTime = 0.7f;

            this.breakableDelayTime = 1.0f;
            this.turretDelayTime = 1.0f;
            this.portalDelayTime = 0.5f;
            this.holeDelayTime = 2.0f;
            this.breakDelayTime = 0.2f;
            this.slowTileDelayTime = 0.4f;

            playerTurn(180.0f);
            var position = transform.position;
            this.targetPosition.x = position.x;
            this.targetPosition.y = position.y;
            
            Up = GetComponentInChildren<CheckBoolUp>();
            Right = GetComponentInChildren<CheckBoolRight>();
            Down = GetComponentInChildren<CheckBoolDown>();
            Left = GetComponentInChildren<CheckBoolLeft>();

            _playerAnchor = Anchor.GetComponent<PlayerAnchor>();
        }

        
        private void Update()
        {
            // if player is hit by cannonball, player stun
            if (this.cannonballHit)
            {
                this.hitDeltaTime += Time.deltaTime;

                if (!(this.hitDeltaTime > this.stunTime)) return;
                this.cannonballHit = false;
                this.hitDeltaTime = 0.0f;

                this.isInteracting = false;
                this.isShiftDown = false;

                this.delayDeltaTime = 0.0f;
            }

            // if player is acceleration state
            if (this.isAcceleration)
            {
                this.accelDurationDeltaTime += Time.deltaTime;

                if (this.accelDurationDeltaTime > this.accelDuration)
                {
                    this.isAcceleration = false;
                    this.movingDelayTime = this.playerMovingDelayTime;
                    this.accelDurationDeltaTime = 0.0f;
                }

            }

            // if key down code is shift, player accelerate
            if (this.isShiftDown)
            {
                this.delayDeltaTime += Time.deltaTime;

                if (!(this.delayDeltaTime > accelDelayTime)) return;
                acceleration();
                this.delayDeltaTime = 0.0f;
            }

            // if player is interacting
            if (this.isInteracting)
            {
                this.delayDeltaTime += Time.deltaTime;

                if (this.frontIsBreakable)
                {
                    this.breakDelayDeltaTime += Time.deltaTime;

                    if (this.breakDelayDeltaTime > this.breakDelayTime)
                    {
                        /*
                        AudioManager.instance.PlaySFX("BreakingBrick");
                        */
                        this.breakDelayDeltaTime = 0.0f;
                    }
                }

                if (!(this.delayDeltaTime > interactDelayTime)) return;
                interact();
                this.delayDeltaTime = 0.0f;
                this.frontIsBreakable = false;
            }
        
            //if moving, keep moving
            if (!(Mathf.Approximately(transform.position.x, targetPosition.x) && Mathf.Approximately(transform.position.y, targetPosition.y)))
            {
                if (isMoving) this.speed = this.movingSpeed;
                else this.speed = this.moveSpeed;

                transform.position = Vector3.MoveTowards(transform.position, this.targetPosition, this.speed);
                return;
            }

            if (this.keyDownFlag)
            {
                this.turnDelayDeltaTime += Time.deltaTime;
                if (!(turnDelayDeltaTime > turnDelayTime)) return;
                setTargetPosition();
                this.keyDownFlag = false;
                this.turnDelayDeltaTime = 0;
            }

            
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (cannotMove) return;
                playerTurn(0.0f);
                keyDownInitialize();
                this.buttonFlagUp = true;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (cannotMove) return;
                playerTurn(180.0f);
                keyDownInitialize();
                this.buttonFlagDown = true;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (cannotMove) return;
                playerTurn(90.0f);
                keyDownInitialize();
                this.buttonFlagLeft = true;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (cannotMove) return;
                playerTurn(-90.0f);
                keyDownInitialize();
                this.buttonFlagRight = true;
            }


            if (this.buttonFlagUp && Input.GetKey(KeyCode.W))
            {
                if (cannotMove) return;
                moving();
            }
            if (this.buttonFlagDown && Input.GetKey(KeyCode.S))
            {
                if (cannotMove) return;
                moving();
            }
            if (this.buttonFlagLeft && Input.GetKey(KeyCode.A))
            {
                if (cannotMove) return;
                moving();
            }
            if (this.buttonFlagRight && Input.GetKey(KeyCode.D))
            {
                if (cannotMove) return;
                moving();
            }


            if (Input.GetKeyDown(KeyCode.E))
            {
                checkFrontObject();
            }


            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                this.isShiftDown = true;
            }


            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckBool();
                BuildTile();
            }
        

            /*
        // if direction keys are up
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            this.movingTriggerDeltaTime = 0.0f;
        }
        */
        }


        private void BuildTile()
        {
            Instantiate(RoadTile, transform.position, Quaternion.identity);

            /*
            AudioManager.instance.PlaySFX("SetNormalTile");
            */
        }
        
        private void playerTurn(float direction)
        {
            transform.rotation = Quaternion.Euler(0, 0, direction);
        }


        private void keyDownInitialize()
        {
            buttonFlagFalse();
            this.movingTriggerDeltaTime = 0.0f;
            this.keyDownFlag = true;
            this.isMoving = false;
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

            if (!(this.movingTriggerDeltaTime > this.movingTriggerTime)) return;
            if (!(Mathf.Approximately(transform.position.x, targetPosition.x) && Mathf.Approximately(transform.position.y, targetPosition.y)))
                return;
            
            if (this.movingDelayDeltaTime > this.movingDelayTime)
            {
                setTargetPosition();
                this.isMoving = true;
                this.movingDelayDeltaTime = 0.0f;
            }
        }
        
        private void setTargetPosition()
        {
            this.prePosition = transform.position;

            if (this.isObstruct) return;

            if (this.buttonFlagUp)
                this.targetPosition.y += this.tileLength;
            else if (this.buttonFlagDown)
                this.targetPosition.y -= this.tileLength;
            else if (this.buttonFlagLeft)
                this.targetPosition.x -= this.tileLength;
            else if (this.buttonFlagRight)
                this.targetPosition.x += this.tileLength;
        }
        
        public void setCannonballHit()
        {
            this.cannonballHit = true;

            /*
            AudioManager.instance.PlaySFX("HitByBullet");
            */
        }


        public void setIsObstruct(bool front)
        {
            this.isObstruct = front;
        }


        public void setFrontObject(GameObject front)
        {
            this.frontObject = front;
            this.isExistFrontObject = true;
        }


        public void resetFrontObject()
        {
            this.isExistFrontObject = false;
        }


        private void checkFrontObject()
        {
            if (!isExistFrontObject) return;

            this.isInteracting = true;

            if (this.frontObject.CompareTag("Breakable"))
            {
                this.interactDelayTime = this.breakableDelayTime;
                this.frontIsBreakable = true;
            }
            else if (this.frontObject.CompareTag("Turret"))
                this.interactDelayTime = this.turretDelayTime;
            else if (this.frontObject.CompareTag("InPortal"))
                this.interactDelayTime = this.portalDelayTime;
            else if (this.frontObject.CompareTag("Hole"))
                this.interactDelayTime = this.holeDelayTime;
            else if (this.frontObject.CompareTag("Tile"))
                this.interactDelayTime = this.slowTileDelayTime;
        }


        // ReSharper disable Unity.PerformanceAnalysis
        private void interact()
        {
            if (this.frontObject.CompareTag("Breakable"))
            {
                Destroy(this.frontObject);
                /*
                AudioManager.instance.PlaySFX("DestroyingBrick");
                */
            }
            else if (this.frontObject.CompareTag("Turret"))
            {
                this.frontObject.GetComponent<TurretController>().setIsPause();
            }
            else if (this.frontObject.CompareTag("InPortal"))
            {
                transform.position = this.frontObject.GetComponent<InPortal>().getDestinationPosition();
                /*
                AudioManager.instance.PlaySFX("InPortal");
                */
                this.targetPosition = transform.position;
            }
            else if (this.frontObject.CompareTag("Hole"))
            {
                HoleAnchor holeScript = this.frontObject.GetComponent<HoleAnchor>();
                
                if (holeScript.getIsClose()) return;
                holeScript.holeClose();
                this.isObstruct = false;
            }
            else if (this.frontObject.CompareTag("Tile"))
            {
                this.frontObject.GetComponent<ToSlowTile>().setIsSlow();
            }

            this.isInteracting = false;
        }


        private void acceleration()
        {
            this.movingDelayTime = this.playerAccelDelayTime;
            this.isAcceleration = true;
            this.isShiftDown = false;
        }
        
        private void CheckBool()
        {
            if (Up.Flag) Direction[1] = true;
            else Direction[1] = false;
            if (Right.Flag) Direction[2] = true;
            else Direction[2] = false;
            if (Down.Flag) Direction[3] = true;
            else Direction[3] = false;
            if (Left.Flag) Direction[4] = true;
            else Direction[4] = false;

            isTileAround = Direction.Count(c => c) > 0;
        }
    }
}
