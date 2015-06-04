using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PackUI : MonoBehaviour {
    public Pack pack;
    public Text numberText;
    public Text starsText;
    public Image progressImage;
    public CanvasGroup contentCG;
    public CanvasGroup lockCG;
    public Text requiredStarsText;
    public float contentLockAlpha = 0.5f;

    private int saveCPURate = 6; //пересчитывать тяжелую логику раз в 6 кадров
    private int frames;

    void Start()
    {
        GetComponentInParent<PageUI>().containers.levelsContainer.gameObject.SetActive(false);
    }

    void Update()
    {
        if (frames % saveCPURate == 0)
            GentleUpdate();
        
        frames++;
    }

    void GentleUpdate()
    {
        FindMissingPacks();
        if (pack == null)
        {
            Debug.LogWarning("Для " + name + " не установленна ссылка на пак уровней");
            return;
        }

        progressImage.fillAmount = (float)((float)pack.StarsRecived * 100f / (float)pack.StarsCount) / 100f;
        starsText.text = System.String.Format("{0:0}", pack.StarsRecived);
        numberText.text = (pack.transform.GetSiblingIndex() + 1).ToString();

        Transform content = transform.FindChild("Content");
        Animator[] animators = content.GetComponentsInChildren<Animator>();
        if (pack.IsClosed)
        {
            if (contentCG.alpha != contentLockAlpha)
                contentCG.alpha = contentLockAlpha;
            foreach (Animator anim in animators)
                anim.speed = 0f;
        }
        else
        {
            if (contentCG.alpha != 1f)
                contentCG.alpha = 1f;
            foreach (Animator anim in animators)
                anim.speed = 1f;
        }

        requiredStarsText.text = System.String.Format("{0:00}", pack.StarsRequired);
    }

    /// <summary>
    /// Если сцена была перезагружена ссылки на паки слетают, 
    /// этот метод находит их снова исходя из 
    /// имени контроллера пака и имени представления пака
    /// </summary>
    void FindMissingPacks()
    {
        if (pack == null)
        {
            GameObject obj = GameObject.Find(name.Replace("UI", ""));
            try
            {
                pack = obj.GetComponent<Pack>();
            }
            catch { }

        }
    }

    /// <summary>
    /// Если на пак кликнули
    /// </summary>
    public void OnClick()
    {
        if (pack.IsClosed)
        {
            lockCG.alpha = 1f;
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("Lock");
            GetComponent<SoundsPlayer>().PlayOneShootSound(SoundNames.MenuPackLocked);
        }
        else
        {
            GetComponentInParent<PageUI>().containers.levelsContainer.gameObject.SetActive(true);
            CGSwitcher.Instance.SetShowObject(
                GetComponentInParent<PageUI>().containers.levelsContainer.GetComponent<Animator>()
                );
            CGSwitcher.Instance.SetHideObject(
                GetComponentInParent<PageUI>().containers.packsContainer.GetComponent<Animator>()
                );
            CGSwitcher.Instance.Switch();
            MyMaze.Instance.LastSelectedPack = pack;
            GetComponent<SoundsPlayer>().PlayOneShootSound(SoundNames.MenuBtnFwdE);
        }
    }
}
