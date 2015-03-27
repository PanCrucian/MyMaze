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

    public CGSwitcherStruct switcherData;

    void Start()
    {
        switcherData.showObject = GetComponentInParent<PageUI>().containers.levelsContainer.GetComponent<Animator>();
        switcherData.hideObject = GetComponentInParent<PacksContainerUI>().GetComponent<Animator>();
    }

    void Update()
    {
        if (pack == null)
        {
            Debug.LogWarning("Для " + name + " не установленна ссылка на пак уровней");
            return;
        }
        progressImage.fillAmount = (float)((float)pack.StarsRecived * 100f / (float)pack.StarsCount) / 100f;
        starsText.text = System.String.Format("{0:00}", pack.StarsRecived);
        numberText.text = (pack.transform.GetSiblingIndex() + 1).ToString();

        if (pack.IsClosed)
        {
            if (contentCG.alpha != contentLockAlpha)
                contentCG.alpha = contentLockAlpha;
        }
        else
        {
            if (contentCG.alpha != 1f)
                contentCG.alpha = 1f;
        }

        requiredStarsText.text = System.String.Format("{0:00}", pack.StarsRequired);
    }

    public void OnClick()
    {
        if (pack.IsClosed)
        {
            lockCG.alpha = 1f;
            Animator animator = GetComponent<Animator>();
            animator.SetTrigger("Lock");
        }
        else
        {
            CGSwitcher switcher = switcherData.switcher;
            switcher.SetHideObject(switcherData.hideObject);
            switcher.SetShowObject(switcherData.showObject);
            switcher.SetDelayTime(switcherData.delay);
            switcher.Switch();
            MyMaze.Instance.LastSelectedPack = pack;
        }
    }
}
