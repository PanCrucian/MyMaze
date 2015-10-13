using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CanvasGroup))]
public class ScreenOverlayUI : GentleMonoBeh {

    public float FadeDelay
    {
        get
        {
            return  0.825f;
        }
    }

    public static ScreenOverlayUI Instance
    {
        get
        {
            return _instance;
        }
    }

    private static ScreenOverlayUI _instance;
    private Image image;
    private Animator animator;
    private Canvas canvas;
    public List<string> actionsQueue = new List<string>();
    
    void Awake()
    {
        if (!IsExist())
        {
            //Сохраняем инстанс в каждой сцене
            if (Application.isPlaying)
                DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
        canvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// Проверяет на копии себя же
    /// </summary>
    bool IsExist()
    {
        ScreenOverlayUI[] objects = GameObject.FindObjectsOfType<ScreenOverlayUI>();
        if (objects.Length > 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Показать черный экран
    /// </summary>
    public void FadeIn()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ScreenOverlayFadeInStay"))
            actionsQueue.Add("FadeIn");
    }

    /// <summary>
    /// Спрятать черный экран
    /// </summary>
    public void FadeOut()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("ScreenOverlayFadeOutStay"))
            actionsQueue.Add("FadeOut");
    }

    /// <summary>
    /// Если потеряли камеру то найдем ее снова
    /// </summary>
    public override void Update()
    {
        base.Update();
        if (canvas.worldCamera == null)
            canvas.worldCamera = Camera.main; 
    }

    /// <summary>
    /// Слушаем очередь действий
    /// </summary>
    public override void GentleUpdate()
    {
        base.GentleUpdate();
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ScreenOverlayFadeInStay") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("ScreenOverlayFadeOutStay"))
            foreach (string action in actionsQueue)
            {
                animator.SetTrigger(action);
                actionsQueue.Remove(action);
                break;
            }
    }
}
