using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private static PlayerController _instance;
    public static PlayerController Instance { get { return _instance; } }

    GameObject fpsCamera;
    [SerializeField] GameObject rtsCamera;
    [SerializeField] GameObject selectionSystem;
    [SerializeField] Text exitRtsText;

    [SerializeField] GameObject projectilePrefab;
    GameObject projectileSpawnPoint;

    [SerializeField] Slider slider;

    [SerializeField] AudioClip attackClip = null;
    [SerializeField] List<AudioClip> movementAudioClipList = new List<AudioClip>();

    Animator playerAnimator;

    private float walkSpeed = 5.0F;
    private float runSpeed = 10.0F;
    private float gravity = 30.0F;
    private Vector3 moveDirection = Vector3.zero;
    private float countdown = 0f;
    private float castSpeed = 0.5f;

    private bool isCloseToTower = false;
    
    private HUDUIHandler hudUIHandler;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        fpsCamera = gameObject.transform.GetChild(0).gameObject;
        projectileSpawnPoint = gameObject.transform.GetChild(1).gameObject;
    }
    private void Start()
    {
        playerAnimator = gameObject.GetComponentInChildren<Animator>();
        hudUIHandler = HUDUIHandler.Instance;
        SetMaxHealth();
    }
    // Update is called once per frame
    void Update()
    {
        Movement();
        if (countdown <= 0)
        {
            CastSpell();
        }
        countdown -= Time.deltaTime;

        if(isCloseToTower)
        {
            Interact();
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            gameObject.GetComponent<Unit>().TakeDamage(30);
        }
    }

    public AudioClip GetRandomFootstepClip() => movementAudioClipList[Random.Range(0, movementAudioClipList.Count)];

    public void Movement()
    {
        CharacterController controller = gameObject.GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
            {
                moveDirection *= runSpeed;
            }
            else
            {
                moveDirection *= walkSpeed;
            }
        }
        moveDirection.y -= gravity * Time.deltaTime;
        playerAnimator.SetFloat("moveX", moveDirection.x);
        playerAnimator.SetFloat("moveZ", moveDirection.z);
        controller.Move(moveDirection * Time.deltaTime);
    }

    public void CastSpell()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(Cursor.lockState == CursorLockMode.None)
            {
                return;
            }
            if(BuildingSelection.Instance.SelectedBuilding == null &&
                UnitSelections.Instance.GetSelectedUnitsList().Count <= 0)
            {
                if (attackClip != null)
                {
                    gameObject.GetComponent<AudioSource>().PlayOneShot(attackClip);
                }
                countdown = 1f / castSpeed;
                GameObject projectileGO = Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, gameObject.transform.GetChild(0).transform.rotation);
                Projectile projectile = projectileGO.GetComponent<Projectile>();
                projectile.SetDamage(gameObject.GetComponent<Unit>().GetRangeDamage());
                projectile.SetRange(gameObject.GetComponent<Unit>().GetRangeAttackRange());
            }
        }
    }

    private void OnDisable() => Crosshair.Instance.ActivateCrosshair(false);

    private void OnEnable() => Crosshair.Instance.ActivateCrosshair(true);

    private void Interact()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            hudUIHandler.GetInteractText().gameObject.SetActive(false);
            exitRtsText.gameObject.SetActive(true);
            fpsCamera.SetActive(false);
            rtsCamera.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            selectionSystem.transform.GetChild(1).GetComponent<UnitClick>().ChangeCameraForClick(rtsCamera.transform.GetChild(0).GetComponent<Camera>());
            selectionSystem.transform.GetChild(2).GetComponent<UnitDrag>().ChangeCameraForDrag(rtsCamera.transform.GetChild(0).GetComponent<Camera>());
            this.enabled = false;
        }
    }

    public void ActvateFPS()
    {
        hudUIHandler.GetInteractText().gameObject.SetActive(true);
        exitRtsText.gameObject.SetActive(false);
        rtsCamera.SetActive(false);
        fpsCamera.SetActive(true);
        selectionSystem.transform.GetChild(1).GetComponent<UnitClick>().ChangeCameraForClick(transform.GetChild(0).GetComponent<Camera>());
        selectionSystem.transform.GetChild(2).GetComponent<UnitDrag>().ChangeCameraForDrag(transform.GetChild(0).GetComponent<Camera>());
    }

    public bool IsPlayerInRTS()
    {
        if(rtsCamera != null)
        {
            if (rtsCamera.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Special"))
        {
            isCloseToTower = true;
            hudUIHandler.GetInteractText().gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Special"))
        {
            isCloseToTower = false;
            hudUIHandler.GetInteractText().gameObject.SetActive(false);
        }
    }

    public void SetHealth(float health) => slider.value = health;
    public void SetMaxHealth()
    {
        slider.maxValue = gameObject.GetComponent<Unit>().GetMaxHelath();
        slider.value = slider.maxValue;
    }
}
