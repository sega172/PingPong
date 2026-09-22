using UnityEngine;
using UnityEngine.EventSystems;

public class PaddleInputPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private Paddle _paddle;
    [SerializeField] private Camera _camera;

    private bool _isPointerDown;
    private Vector2 _pointerScreenPosition;

    private void Awake()
    {
        if (_camera == null)
            _camera = Camera.main;
    }

    private void FixedUpdate()
    {
        if (_isPointerDown == false || _paddle == null)
            return;

        UpdatePaddleTarget();
    }

    private void OnDisable()
    {
        _isPointerDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPointerDown = true;
        _pointerScreenPosition = eventData.position;
        UpdatePaddleTarget();
    }

    public void OnDrag(PointerEventData eventData)
    {
        _pointerScreenPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPointerDown = false;
    }

    private void UpdatePaddleTarget()
    {
        if (_camera == null || _paddle == null)
            return;

        float distanceToPlane = Mathf.Abs(_camera.transform.position.z - _paddle.transform.position.z);
        Vector3 screenPosWithDepth = new Vector3(_pointerScreenPosition.x, _pointerScreenPosition.y, distanceToPlane);
        Vector3 worldPoint = _camera.ScreenToWorldPoint(screenPosWithDepth);

        _paddle.SetTargetY(worldPoint.y);
    }
}
