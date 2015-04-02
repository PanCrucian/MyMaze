using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LevelUI : MonoBehaviour {

    [System.Serializable]
    public class VisualSettings
    {
        public float closedAlpha = 0.35f;
        public float openedAlpha = 1f;
    }

    public Level level;

    public Text numberText;
    public StarsUI starsui;
    public VisualSettings visualSettings;

    private CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        //ссылку на level устанавливает LevelsUI исходя из последнего выбранного пака
    }

    void Update()
    {
        if (level == null)
        {
            return;
        }
        if (!MyMaze.Instance.LastSelectedPack.IsYourLevel(level))
            return;
        numberText.text = level.displayText;
        if (level.IsClosed)
            CloseLevel();
        else
            OpenLevel();
        SetupStars();
    }

    void CloseLevel()
    {
        canvasGroup.alpha = visualSettings.closedAlpha;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void OpenLevel()
    {
        canvasGroup.alpha = visualSettings.openedAlpha;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Смотрит какие звезды получены и изменяет вид на экране
    /// </summary>
    void SetupStars()
    {
         List<Star> simplestars = level.GetSimpleStars();
         for (int i = 0; i < simplestars.Count; i++)
         {
             Star star = simplestars[i];
             StarUI starui = starsui.stars[i];

             if (star.IsCollected)
                 starui.SetRecivedStar();
             else
                 starui.SetLostStar();
         }
         Star starHidden = level.GetHiddenStar();
         if (starHidden != null)
         {
             if (starHidden.IsCollected)
                 starsui.starHidden.SetRecivedStar();
             else
                 starsui.starHidden.SetLostStar();
         }
         else
         {
             starsui.starHidden.SetNotAvalible();
         }

    }
}
