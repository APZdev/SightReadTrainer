using UnityEngine.EventSystems;
using UnityEngine;

public class IntervalSlider : MonoBehaviour, IDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private float yStartCoord;
    public bool isMoving;

    private void Awake()
    {
        canvas = transform.root.GetComponent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        yStartCoord = transform.localPosition.y;
    }

    private void Update()
    {
        isMoving = Input.GetMouseButton(0) ? true : false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Get the position of the cursor based on the movement of the mouse per frame
        Vector2 finalPosition = (eventData.delta / (canvas.scaleFactor * 1.5f));
        rectTransform.anchoredPosition += finalPosition;
        //Reset the position of the object on the Y axis because it's not 
        //needed and I don't know how to make it in another way than this
        rectTransform.localPosition = new Vector2(transform.localPosition.x, yStartCoord);
    }
}
