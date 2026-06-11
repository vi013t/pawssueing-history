using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuImage : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler, IPointerClickHandler
{
    public Sprite normalSprite;
    public Sprite hoverSprite;

    public bool isStartButton;
    public string sceneName = "Scene1";

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        image.sprite = normalSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.sprite = normalSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isStartButton)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("Escaped");
            Application.Quit();
        }
    }
}