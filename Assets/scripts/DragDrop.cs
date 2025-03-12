using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _dragging;

    public void OnPointerDown(PointerEventData data)
    {
        _dragging = true;
    }

    public void OnPointerUp(PointerEventData data)
    {
        _dragging = false;
    }

    private void Update()
    {
        if (_dragging == true)
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
    }
}

