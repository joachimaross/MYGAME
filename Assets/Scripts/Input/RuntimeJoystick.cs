using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Simple on-screen joystick. Attach to a UI Image and use the Value field from other scripts.
/// Implements IPointer interfaces to capture drag and pointer events.
/// </summary>
public class RuntimeJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform background;
    public RectTransform handle;

    Vector2 input = Vector2.zero;

    public Vector2 Value => input;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (background == null || handle == null) return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out pos);
        Vector2 pivotAdjusted = new Vector2(pos.x / background.sizeDelta.x, pos.y / background.sizeDelta.y) * 2f;
        input = Vector2.ClampMagnitude(new Vector2(pivotAdjusted.x, pivotAdjusted.y), 1f);

        handle.anchoredPosition = new Vector2(input.x * (background.sizeDelta.x / 2f), input.y * (background.sizeDelta.y / 2f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        input = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }
}
