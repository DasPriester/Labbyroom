using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controller class for the player
/// </summary>
public class PlayerController : MonoBehaviour {
    public Vector3 PreviousOffsetFromPortal { get; set; }

    public bool IsSprinting => canSprint && Input.GetKey(settings.sprintKey);
    public bool ShouldJump => Input.GetKeyDown(settings.jumpKey) && characterController.isGrounded;
    public bool ShouldCrouch => Input.GetKeyDown(settings.crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    public Settings settings;

    [Header("Funktional Options")]
    public bool canMove = false;
    public bool canSprint = false;
    public bool canJump = false;
    public bool canCrouch = false;
    public bool canInteract = false;
    public bool canPickUp = false;
    public bool canBuild = false;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 30.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;
    public static bool hasTempPath = false;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 12f;
    [SerializeField] private float walkBobAmount = 0.02f;
    [SerializeField] private float sprintBobSpeed = 8f;
    [SerializeField] private float sprintBobAmount = 0.04f;
    [SerializeField] private float crouchBobSpeed = 14f;
    [SerializeField] private float crouchBobAmount = 0.01f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Interaction")]
    [SerializeField] private Vector3 interactionRayPoint = new Vector3(0.5f, 0.5f, 0);
    [SerializeField] private float interactionDistance = 4;
    [SerializeField] private LayerMask interactionLayer = default;
    [SerializeField] private LayerMask wallLayer = default;
    private Interactable currentInteractable;

    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float sprintStepMultiplier = 0.6f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] defaultClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] sandClips = default;
    [SerializeField] private AudioClip[] stoneClips = default;

    [SerializeField] private AudioMixer audioMixer = default;
    private float footstepTimer = 0;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : IsSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

    public float Pitch { get => pitch; set => pitch = value; }
    public float Yaw { get => yaw; set => yaw = value; }

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float pitch;
    private float yaw;

    private Volume pp;
    private ChromaticAberration ca;

    private void Start()
    {
        pp = FindObjectOfType<Volume>();
    }

    void Awake()
    {
        PreviousOffsetFromPortal = new Vector3();
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        settings = Resources.Load<Settings>("Settings/Current");
        foreach (InGameMenu menu in settings.menus)
        {
            if (!settings.liveMenus.ContainsKey(menu.name))
                settings.liveMenus.Add(menu.name, Instantiate(menu, FindObjectOfType<InventoryMenu>().transform));

            if (settings.liveMenus.ContainsKey(menu.name) && settings.liveMenus[menu.name] == null)
                settings.liveMenus[menu.name] = Instantiate(menu, FindObjectOfType<InventoryMenu>().transform);

        }
    }

    void Update()
    {
        List<PortalComponent> allPortals = FindObjectsOfType<PortalComponent>().ToList();
        pp.profile.TryGet(out ca);
        ca.intensity.overrideState = true;
        if (allPortals.Count > 0)
        {
            ca.intensity.value = Mathf.Clamp(1f - allPortals.Select(x => Vector3.Distance(x.transform.position, transform.position) - 1f).Min(), 0f, 1f);
        } else
        {
            ca.intensity.value = 0f;
        }

        audioMixer.SetFloat("Master Volume", Mathf.Log10(settings.masterVolume) * 20);
        audioMixer.SetFloat("Effects Volume", Mathf.Log10(settings.effectsVolume) * 20);
        audioMixer.SetFloat("Music Volume", Mathf.Log10(settings.musicVolume) * 20);

        if (canMove)
        {
            HandleMouseInput();
            HandleMovementInput();
        }
        if (canJump)
            HandleJump();

        if (canCrouch)
            HandleCrouch();


        if (canInteract)
            HandleInteractionInput();
        
        if (canBuild)
            HandleBuildInput();

        if (canPickUp)
            HandlePickUpInput();
        
        if(canInteract || canBuild || canPickUp)
            HandleFocus();

        if (settings.useHeadbob)
            HandleHeadbob();

        if (settings.useFootsteps)
            HandleFootsteps();

        ApplyFinalMovements();
        
    }

    /// <summary>
    /// Convert input to horizontal/vertical movement
    /// </summary>
    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"),
                                   (isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;

        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) +
                        (transform.TransformDirection(Vector3.right) * currentInput.y);

        moveDirection.y = moveDirectionY;


    }
    
    /// <summary>
    /// Turn camera with mouse
    /// </summary>
    private void HandleMouseInput()
    {
        float mX = Input.GetAxis("Mouse X");
        float mY = Input.GetAxis("Mouse Y");

        // Verrrrrry gross hack to stop camera swinging down at start
        float mMag = Mathf.Sqrt(mX * mX + mY * mY);
        if (mMag > 5)
        {
            mX = 0;
            mY = 0;
        }
        // pitch (up-down)
        pitch -= mY * lookSpeedY;
        pitch = Mathf.Clamp(pitch, -upperLookLimit, lowerLookLimit);

        // yaw (left-right)
        yaw += mX * lookSpeedX;

        // apply pitch and yaw 
        playerCamera.transform.localEulerAngles = Vector3.right * pitch;
        transform.eulerAngles = Vector3.up * yaw;

    }

    /// <summary>
    /// Apply jump force
    /// </summary>
    private void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }

    /// <summary>
    /// Start crouch corutine
    /// </summary>
    private void HandleCrouch()
    {
        if (ShouldCrouch)
            StartCoroutine(CrouchStand());
    }

    /// <summary>
    /// Do headbobing
    /// </summary>
    private void HandleHeadbob()
    {
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }


    }

    /// <summary>
    /// Send raycast and call the focus method for currently targeted object
    /// </summary>
    private void HandleFocus()
    {
        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            if (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.gameObject.GetInstanceID())
            {
                if (currentInteractable != null)
                {
                    currentInteractable.OnLoseFcous();
                }


                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                    currentInteractable.OnFocus();
                
            }
        }                                       
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFcous();
            currentInteractable = null;
        }


        if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit wall, interactionDistance, wallLayer))
        {
            SurfaceInteractable surface = wall.collider.gameObject.GetComponent<SurfaceInteractable>();
            Item item = GameObject.Find("Player").GetComponent<Inventory>().CurrentItem();

            try
            {
                if ((surface && item.prefab.GetComponent<KeyInteractable>()) && (!(!item.prefab.GetComponent<KeyInteractable>().temporary && surface.IsTemporary)) && !(item.prefab.GetComponent<KeyInteractable>().temporary && hasTempPath && !surface.IsTemporary))
                {
                    surface.OnViewedAtWithKey(wall.point, item.prefab.GetComponent<KeyInteractable>().MaxWidth());
                }
            }
            catch (System.NullReferenceException) { }
        }
    } 
    
    /// <summary>
    /// Call interact mothod on currently focused interactable
    /// </summary>
    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(settings.interactKey) && currentInteractable != null && 
            Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract(hit.point);
        }

    }
    private void HandleBuildInput()
    {
        if (Input.GetKeyDown(settings.buildKey) && 
            Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            Inventory inv = transform.GetComponent<Inventory>();
            Item item = inv.CurrentItem();

            if (item.prefab != null)
            {
                if(!item.prefab.GetComponent<KeyInteractable>())
                {
                    item.amount = 1;
                    if (inv.RemoveItem(item))
                    {
                        GameObject newItem = Instantiate(item.prefab, new Vector3(hit.point.x, hit.point.y, hit.point.z), gameObject.transform.rotation * item.prefab.transform.rotation);
                        newItem.GetComponent<PickUpInteractable>().OnBuild(hit.point);
                    }
                }
                else if (Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit _, interactionDistance, wallLayer) && currentInteractable != null)
                    currentInteractable.OnBuild(hit.point);
            }
        }
    }
    private void HandlePickUpInput()
    {
        if (Input.GetKeyDown(settings.pickUpKey) && currentInteractable != null && 
            Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnPickUp(hit.point);
        }
    }

    /// <summary>
    /// Play footsteps
    /// </summary>
    private void HandleFootsteps()
    {
        if (!characterController.isGrounded) return;
        if (currentInput == Vector2.zero) return;
        

        footstepTimer -= Time.deltaTime;

        if(footstepTimer <= 0)
        {
            if(Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/Wood":
                        footstepAudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length)]);
                        break;
                    case "Footsteps/Grass":
                        footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length)]);
                        break;
                    case "Footsteps/Sand":
                        footstepAudioSource.PlayOneShot(sandClips[Random.Range(0, sandClips.Length)]);
                        break;
                    case "Footsteps/Stone":
                        footstepAudioSource.PlayOneShot(stoneClips[Random.Range(0, stoneClips.Length)]);
                        break;
                    default:
                        footstepAudioSource.PlayOneShot(defaultClips[Random.Range(0, defaultClips.Length)]);
                        break;
                }
            }

            footstepTimer = GetCurrentOffset;
        }
    }

    /// <summary>
    /// Apply gravity and movement to controller
    /// </summary>
    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;


        characterController.Move(moveDirection * Time.deltaTime);
    }

    /// <summary>
    /// Crouch animation
    /// </summary>
    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    /// <summary>
    /// Teleport the player with a portal
    /// </summary>
    /// <param name="fromPortal">Start portal</param>
    /// <param name="toPortal">End portal</param>
    /// <param name="pos">Goal position</param>
    /// <param name="rot">Goal rotation</param>
    public void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        Vector3 eulerRot = rot.eulerAngles;
        float delta = Mathf.DeltaAngle(yaw, eulerRot.y);
        yaw += delta;
        transform.eulerAngles = Vector3.up * yaw;
        moveDirection = toPortal.TransformVector(fromPortal.InverseTransformVector(moveDirection));
        Physics.SyncTransforms();

        PortalComponent tp = toPortal.GetComponent<PortalComponent>();

        if (tp.IsTemporary && !tp.Room.IsTemporary)
        {
            Destroy(toPortal.gameObject);
            gameObject.layer = LayerMask.NameToLayer("Player");
            WallManager wm = tp.WallManager;
            wm.doors.Remove(tp.Door);
            wm.UpdateWall();
            PortalConnector.ZTemp = 0;

            foreach (Room rm in FindObjectsOfType<Room>())
            {
                if (Mathf.Round(rm.transform.position.x / 100) != 0)
                {
                    Destroy(rm.gameObject);
                }
            }

            foreach (PortalComponent pc in FindObjectsOfType<PortalComponent>())
            {
                if (Mathf.Round(pc.transform.position.x / 100) != 0)
                {
                    Destroy(pc.gameObject);
                }
            }

            hasTempPath = false;
        }
    }

    public void EnterPortalThreshold() { }
    public void ExitPortalThreshold() { }
}
