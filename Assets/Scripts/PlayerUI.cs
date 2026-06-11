using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerControls player;

    public Image heart1;
    public Image heart2;
    public Image heart3;

    public Image staminaBar;

    void Update()
    {
        int hp = player.Health;

        heart1.enabled = hp >= 1;
        heart2.enabled = hp >= 2;
        heart3.enabled = hp >= 3;

        staminaBar.fillAmount = player.stamina / player.maxStamina;
    }
}