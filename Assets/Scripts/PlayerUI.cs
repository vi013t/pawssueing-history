using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerControls player;

    private float currentAngle = 0f;
    private float rewindAngle = -360f;

    public Image heart1;
    public Image heart2;
    public Image heart3;

    public Image staminaBar;
    public RectTransform clockHand;

    void Update()
    {
        int hp = player.Health;

        heart1.enabled = hp >= 1;
        heart2.enabled = hp >= 2;
        heart3.enabled = hp >= 3;

        staminaBar.fillAmount = player.stamina / player.maxStamina;

        if (player.recordingTime > 0)
        {
            float percent = player.recordingTime / player.recordTime;

            currentAngle = -360f * (1f - percent);
            rewindAngle = currentAngle;
        }
        else
        {
            rewindAngle = Mathf.MoveTowards(
                rewindAngle,
                0f,
                (360f / player.recordTime) * Time.deltaTime
            );

            currentAngle = rewindAngle;
        }

        clockHand.localRotation =
    Quaternion.Euler(0, 0, currentAngle);
        }
    }
    