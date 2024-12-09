/*using UnityEngine;
using UnityEngine.UI;

public class BubbleManager : MonoBehaviour
{
    public GameObject bubblePrefab;
    private GameObject bubbleInstance;
    private Image bubbleImage;

    public void InitializeBubble(Transform parent)
    {
        bubbleInstance = Instantiate(bubblePrefab, parent);
        bubbleInstance.transform.localPosition = new Vector3(0, 3, 0);
        bubbleImage = bubbleInstance.GetComponentInChildren<Image>();
        bubbleInstance.SetActive(false);
    }

    public void UpdateBubble(Sprite sprite)
    {
        if (bubbleImage != null)
        {
            bubbleImage.sprite = sprite;
            bubbleInstance.SetActive(true);
        }
    }

    public void HideBubble()
    {
        if (bubbleInstance != null)
        {
            bubbleInstance.SetActive(false);
        }
    }
}
*/