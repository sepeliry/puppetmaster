using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EnhancedButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField]
    [Tooltip("Time of the press to be recognised as a longpress")]
    private float pressingTime = 0.3f;
    private bool pressedDown = false;
    public UnityEvent onClick = new UnityEvent();

    public UnityEvent onLongPress = new UnityEvent();

    public void OnPointerDown(PointerEventData eventData)
    {
        pressedDown = false;
        Invoke("OnLongPress", pressingTime);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelInvoke("OnLongPress");

        if (!pressedDown)
        {
            onClick.Invoke();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelInvoke("OnLongPress");
    }

    private void OnLongPress()
    {
        pressedDown = true;
        onLongPress.Invoke();
    }
}