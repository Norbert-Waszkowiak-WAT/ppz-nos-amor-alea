using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsMenuController : MonoBehaviour
{
    public RectTransform settingsPanel; 
    public float animationDuration = 0.5f; 

    private Vector2 hiddenPosition; 
    private Vector2 shownPosition; 

    private void Start()
    {
        hiddenPosition = new Vector2(-settingsPanel.rect.width, settingsPanel.anchoredPosition.y);
        shownPosition = settingsPanel.anchoredPosition;

        settingsPanel.anchoredPosition = hiddenPosition; 
    }

    public void ToggleSettingsMenu()
    {
        if (settingsPanel.anchoredPosition == hiddenPosition)
        {
            StartCoroutine(AnimatePanel(shownPosition));
        }
        else
        {
            StartCoroutine(AnimatePanel(hiddenPosition));
        }
    }

    private IEnumerator AnimatePanel(Vector2 targetPosition)
    {
        float elapsedTime = 0f;
        Vector2 startingPosition = settingsPanel.anchoredPosition;

        while (elapsedTime < animationDuration)
        {
            settingsPanel.anchoredPosition = Vector2.Lerp(startingPosition, targetPosition, (elapsedTime / animationDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        settingsPanel.anchoredPosition = targetPosition; 
    }
}
