using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    public float maxDistance = 100f;
    public bool useRightSide = false;
    public bool isStatic = false;
    public float activationRadius = 150f;

    public Vector2 Direction { get; private set; }
    public float Speed { get; private set; }

    private Vector2 startPosition;
    private bool isActive = false;

    private void Start()
    {
        if (isStatic)
        {
            startPosition = joystickBackground.position;
            joystickBackground.gameObject.SetActive(true);
            joystickHandle.gameObject.SetActive(true);
        }
    }
    private void Update()
    {
        if (!Application.isMobilePlatform)
        {
            if (!isStatic)
            {
                if (!isActive && Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePos = Input.mousePosition;
                    if (!useRightSide)
                    {
                        if (mousePos.x > Screen.width / 2)
                            return;
                    }
                    else
                    {
                        if (mousePos.x < Screen.width / 2)
                            return;
                    }
                    ActivateJoystick(mousePos);
                }
                if (isActive && Input.GetMouseButton(0))
                {
                    UpdateJoystick(Input.mousePosition);
                }
                if (isActive && Input.GetMouseButtonUp(0))
                {
                    DeactivateJoystick();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Vector2 mousePos = Input.mousePosition;
                    if (Vector2.Distance(mousePos, joystickBackground.position) <= activationRadius)
                        isActive = true;
                }
                if (isActive && Input.GetMouseButton(0))
                {
                    UpdateJoystick(Input.mousePosition);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (isActive)
                    {
                        ResetStaticJoystick();
                        isActive = false;
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isStatic)
        {
            if (!useRightSide)
            {
                if (eventData.position.x > Screen.width / 2)
                    return;
            }
            else
            {
                if (eventData.position.x < Screen.width / 2)
                    return;
            }
            ActivateJoystick(eventData.position);
        }
        else
        {
            if (Vector2.Distance(eventData.position, joystickBackground.position) > activationRadius)
                return;
            isActive = true;
            UpdateJoystick(eventData.position);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isStatic)
        {
            if (isActive)
                UpdateJoystick(eventData.position);
        }
        else
        {
            if (isActive)
                UpdateJoystick(eventData.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isStatic)
        {
            if (!isActive)
                return;
            DeactivateJoystick();
        }
        else
        {
            if (isActive)
            {
                ResetStaticJoystick();
                isActive = false;
            }
        }
    }

    private void ActivateJoystick(Vector2 position)
    {
        isActive = true;
        startPosition = position;
        joystickBackground.position = startPosition;
        joystickHandle.position = startPosition;
        joystickBackground.gameObject.SetActive(true);
        joystickHandle.gameObject.SetActive(true);
    }

    private void UpdateJoystick(Vector2 currentPosition)
    {
        Vector2 offset = currentPosition - startPosition;
        float distance = Mathf.Clamp(offset.magnitude, 0, maxDistance);
        Vector2 direction = offset.normalized;
        joystickHandle.position = startPosition + direction * distance;
        Direction = direction;
        Speed = distance / maxDistance;
    }

    private void DeactivateJoystick()
    {
        isActive = false;
        Direction = Vector2.zero;
        Speed = 0;
        joystickHandle.position = joystickBackground.position;
        joystickBackground.gameObject.SetActive(false);
        joystickHandle.gameObject.SetActive(false);
    }

    private void ResetStaticJoystick()
    {
        Direction = Vector2.zero;
        Speed = 0;
        joystickHandle.position = joystickBackground.position;
    }
}
