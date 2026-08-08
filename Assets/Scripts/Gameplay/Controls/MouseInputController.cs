using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private float deadZone = 10f;
    [SerializeField] private float maxRadius = 100f;

    private IMovable movable;
    private bool isDragging = false;
    private bool controlsEnabled = true;
    private float baseY;
    [SerializeField] private float currentDirection = 0f;
    private Vector2 handleStartPosition;

    private void Start()
    {
        if (targetObject != null)
            movable = targetObject.GetComponent<IMovable>();

        GameManager.OnSetControls += GameManager_OnSetControls;

        if (joystickHandle != null)
        {
            handleStartPosition = joystickHandle.anchoredPosition;
            joystickHandle.gameObject.SetActive(false);
        }
    }

    private void GameManager_OnSetControls(bool controlsEnabled)
    {
        this.controlsEnabled = controlsEnabled;
        if (!controlsEnabled)
        {
            currentDirection = 0f;
            if (movable != null)
                movable.SetDirection(0f);
            if (joystickHandle != null)
                joystickHandle.gameObject.SetActive(false);
            isDragging = false;
        }
    }

    private void Update()
    {
        if (!controlsEnabled)
        {
            if (movable != null)
                movable.SetDirection(0f);
            return;
        }

        if (!isDragging)
        {
            currentDirection = 0f;
        }

        if (movable != null)
            movable.SetDirection(currentDirection);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!controlsEnabled) return;

        isDragging = true;
        baseY = eventData.position.y;
        if (joystickHandle != null)
        {
            joystickHandle.gameObject.SetActive(true);
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!controlsEnabled) return;

        isDragging = false;
        currentDirection = 0f;
        if (joystickHandle != null)
        {
            joystickHandle.gameObject.SetActive(false);
            joystickHandle.anchoredPosition = Vector2.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!controlsEnabled || !isDragging) return;

        float currentY = eventData.position.y;
        float delta = currentY - baseY;

        if (delta > maxRadius)
            baseY = currentY - maxRadius;
        else if (delta < -maxRadius)
            baseY = currentY + maxRadius;

        delta = currentY - baseY;

        if (Mathf.Abs(delta) < deadZone)
        {
            currentDirection = 0f;
            if (joystickHandle != null)
                joystickHandle.anchoredPosition = Vector2.zero;
        }
        else
        {
            currentDirection = Mathf.Clamp(delta / maxRadius, -1f, 1f);
            if (joystickHandle != null)
            {
                float handleY = Mathf.Clamp(delta, -maxRadius, maxRadius);
                joystickHandle.anchoredPosition = new Vector2(0f, handleY);
            }
        }
    }
}