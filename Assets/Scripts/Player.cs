using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Move")]
    [SerializeField] private float speed;
    [SerializeField] private float laneSpeed;


    [Header("Jump")]
    [SerializeField] private float jumpLength;
    [SerializeField] private float jumpHeight;


    [Header("Slide")]
    [SerializeField] private float slideLength;


    [Header("Life")]
    [SerializeField] private int maxLife;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float invincibleTime;
    [SerializeField] private GameObject model;


    [HideInInspector] public float score;
    [HideInInspector] public int coins;





    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Vector3 boxColliderSize;
    private bool isSwipping = false;
    private Vector2 staringTouch;
    




    private Animator anim;
    private int currentLane = 1;
    private bool isJumping = false;
    private Vector3 verticalTargetPosition;
    private float jumpStart;
    private bool isSliding = false;
    private bool invincible = false;
    static int blinkingValue;
    private UIManager uiManager;
    private int Coins;
    private int currentLife;
    private float Score;
    private bool isMagneting=false;
   


    private float slideStart;


    //life






    // luot bang tay
  //  private bool isSwipping = false; //
    //  private Vector2 startingTouch;  //




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        boxColliderSize = boxCollider.size;
        anim.Play("runStart");
        currentLife = maxLife;
        speed = minSpeed;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        uiManager = FindObjectOfType<UIManager>();
         
    }


    // Update is called once per frame
    void Update()
    {

        score += Time.deltaTime * speed;
        uiManager.UpdateScore((int)score);
        // luot bang nut
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }

        if (Input.touchCount == 1)
        {
            if (isSwipping)
            {
                Vector2 diff = Input.GetTouch(0).position - staringTouch;
                diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);
                if (diff.magnitude > 0.01f)
                {
                    if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
                    {
                        if (diff.y < 0)
                        {
                            Slide();
                        }
                        else
                        {
                            Jump();
                        }
                    }

                    else
                    {
                        if (diff.x < 0)
                        {
                            ChangeLane(-1);
                        }
                        else
                        {
                            ChangeLane(1);
                        }

                    }
                    isSwipping = false;
                }

                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    staringTouch = Input.GetTouch(0).position;
                    isSwipping = true;
                }
                else if (Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    isSwipping = false;
                }
            }


            // luot bang ngon tay
            #region control by hand
            if (Input.touchCount == 1)
            {
                if (isSwipping)
                {
                    Vector2 diff = Input.GetTouch(0).position - staringTouch;
                    diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);
                    if (diff.magnitude > 0.01f)
                    {
                        if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
                        {
                            if (diff.y < 0)
                            {
                                Slide();
                            }
                            else
                            {
                                Jump();
                            }
                        }
                        else
                        {
                            if (diff.x < 0)
                            {
                                ChangeLane(-1);
                            }
                            else
                            {
                                ChangeLane(1);
                            }
                        }
                    }


                    isSwipping = false;
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                staringTouch = Input.GetTouch(0).position;
                isSwipping = true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isSwipping = false;
            }
        }
        #endregion

        if (isJumping)
        {
            float ratio = (transform.position.z - jumpStart) / jumpLength;
            if (ratio >= 1f)
            {
                isJumping = false;
                anim.SetBool("Jumping", false);
            }
            else
            {
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
            }
        }
        else
        {
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5 * Time.deltaTime);
        }


        if (isSliding)
        {
            float ratio = (transform.position.z - slideStart) / slideLength;
            if (ratio >= 1f)
            {
                isSliding = false;
                anim.SetBool("Sliding", false);
                boxCollider.size = boxColliderSize;
            }
        }


        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        this.transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
    }


    private void FixedUpdate()
    {
        rb.velocity = Vector3.forward * speed;
    }


    private void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane < 0 || targetLane > 2) return;
        currentLane = targetLane;
        verticalTargetPosition = new Vector3((currentLane - 1), 0, 0);
    }


    private void Jump()
    {
        if (!isJumping)
        {
            jumpStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / jumpLength);
            anim.SetBool("Jumping", true);
            isJumping = true;
        }
    }


    private void Slide()
    {
        if (!isJumping && !isSliding)
        {
            slideStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / slideLength);
            anim.SetBool("Sliding", true);
            Vector3 newSize = boxCollider.size;
            newSize.y = newSize.y / 2;
            boxCollider.size = newSize;
            isSliding = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Magnet"))
        {
            isMagneting = true;
            Coin[] coins = FindObjectsOfType<Coin>();
            List<Coin> allCoin = coins.ToList();

            for (int i = 0; i < allCoin.Count; i++)
            {
                allCoin[i].SetNewSize();

            } 

        }
        if (other.CompareTag("Coin"))
        {

            if (isMagneting)
            {
                other.GetComponent<Coin>().SetIsFly(true);
            }
            
            coins++;
            uiManager.UpdateCoins(coins);
          /*  other.transform.parent.gameObject.SetActive(false);*/


        }



        if (invincible)
            return;

        if (other.CompareTag("Obstacle"))
        {
            currentLife--;
            uiManager.UpdateLives(currentLife);
            anim.SetTrigger("Hit");
            speed = 0;
            if (currentLife <= 0)
            {
                speed = 0;
                anim.SetBool("Dead", true);

                uiManager.gameOverPanel.SetActive(true);  // bi loi gameOverPanel //
                Invoke("CallMenu", 2f);

            }
            else
            {
                StartCoroutine(Blinking(invincibleTime));  // phut 11:25 clip 6 //
            }
        }
    }
        IEnumerator Blinking(float time)
        {
            invincible = true;
            float timer = 0;
            float currentBlink = 1f;
            float lastBlink = 0;
            float blinkPeriod = 0.1f;
            bool enabled = false;
            yield return new WaitForSeconds(1f);
            speed = minSpeed;
            while(timer < time && invincible)
            {
                model.SetActive(enabled);
                Shader.SetGlobalFloat(blinkingValue, currentBlink);
                yield return null;
                timer += Time.deltaTime;
                lastBlink += Time.deltaTime;
                if (blinkPeriod < lastBlink)
                {
                    lastBlink = 0;
                    currentBlink = 1f - currentBlink;
                    enabled = !enabled;
                }
            }
            model.SetActive(true);
            Shader.SetGlobalFloat(blinkingValue, 0);
            invincible = false;

        }

    void  CallMenu()
    {
        GameManager.gm.EndRun();
    }

    public void IncreaseSpeed()
    {
        speed *= 1.15f;
        if (speed >= maxSpeed)
            speed = maxSpeed;
    }
    }



    //Collision

