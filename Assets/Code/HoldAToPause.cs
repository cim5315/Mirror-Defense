using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class HoldAToPause : MonoBehaviour
{
    [Header("Input System")]
    [Tooltip("Assign your HoldConfirm InputActionReference here.")]
    public InputActionReference holdAction;

    [Header("Visuals")]
    [Tooltip("The Image component that swaps sprites as you hold.")]
    public Image targetImage;
    [Tooltip("Add your images in order from start (0%) to finish (100%).")]
    public Sprite[] progressSprites;

    [Header("Settings")]
    [Tooltip("Seconds required to complete the hold.")]
    public float holdDuration = 1.5f;

    [Header("Action")]
    public UnityEvent onHoldComplete;
    private float holdTimer = 0f;
    private bool hasTriggered = false;
    public GameObject pauseMenu;

    private void OnEnable()
    {
        if (holdAction != null && holdAction.action != null)
            holdAction.action.Enable();
    }

    private void OnDisable()
    {
        if (holdAction != null && holdAction.action != null)
            holdAction.action.Disable();
    }

    private void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        ResetButton();
    }

    private void Update()
    {
        // Checks if the Input System button is held OR the UI is clicked/touched
        bool isInputHeld = holdAction != null && holdAction.action != null && holdAction.action.IsPressed();
        bool isHolding = isInputHeld;

        if (isHolding && !hasTriggered)
        {
            holdTimer += Time.deltaTime;
            UpdateSpriteProgress();

            if (holdTimer >= holdDuration)
            {
                hasTriggered = true;
                onHoldComplete?.Invoke();
            }
        }
        else if (!isHolding && holdTimer > 0f)
        {
            // Reset if the player lets go before finishing
            ResetButton();
        }
    }

    private void UpdateSpriteProgress()
    {
        if (progressSprites == null || progressSprites.Length == 0 || targetImage == null)
            return;

        float progress = Mathf.Clamp01(holdTimer / holdDuration);
        int spriteIndex = Mathf.FloorToInt(progress * (progressSprites.Length - 1));

        targetImage.sprite = progressSprites[spriteIndex];
    }

    private void ResetButton()
    {
        holdTimer = 0f;
        hasTriggered = false;

        if (progressSprites != null && progressSprites.Length > 0 && targetImage != null)
        {
            targetImage.sprite = progressSprites[0];
        }
    }
    public void HeldButton()
    {
        pauseMenu.SetActive(true);
    }
}
