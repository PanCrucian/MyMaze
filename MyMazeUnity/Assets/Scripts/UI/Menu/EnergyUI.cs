using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnergyUI : MonoBehaviour {

    public bool isGameUI = false;
    public float animationTime = 0.5f;
    public Image energyImage;
    public Text timerText;
    public GameObject premiumContainer;

    private Energy energy;
    private bool animate = false;
    private float iterationTime = 0.1f;

    void Start()
    {
        energy = MyMaze.Instance.Energy;
    }

    void Update()
    {
        if (!isGameUI)
        {
            timerText.text = GetTimerString();
            ControlPremiumContainer();
        }
        if (!animate)
            CalculateImageFillSlice();
    }

    /// <summary>
    /// Контролируем видимость премиума
    /// </summary>
    void ControlPremiumContainer()
    {
        EnergyBlock energyBlock = energy.GetFirstNotAvaliableBlock();
        if (energyBlock != null)
        {
            if (energyBlock.index <= 3)
            {
                if (!premiumContainer.activeSelf)
                    premiumContainer.SetActive(true);
            }
            else
            {
                if (premiumContainer.activeSelf)
                    premiumContainer.SetActive(false);
            }
        }
        else
        {
            if (premiumContainer.activeSelf)
                premiumContainer.SetActive(false);
        }
    }

    /// <summary>
    /// Запускаем процессы анимации блока, когда успешное использование
    /// </summary>
    public void AnimateNormal()
    {
        animate = true;
        StartCoroutine(AnimateNormalNumerator());
    }

    /// <summary>
    /// Запускаем процессы анимации блока, когда плохое использование
    /// </summary>
    public void AnimateBad()
    {
        animate = true;
        StartCoroutine(AnimateBadNumerator());
    }

    IEnumerator AnimateBadNumerator()
    {
        bool flag = false;
        for (float t = 0; t <= animationTime; t += iterationTime)
        {            
            float fillAmount = 0f;
            if (flag)
                fillAmount = 1f / (float)energy.MaxUnits;
            energyImage.fillAmount = fillAmount;

            flag = !flag;
            yield return new WaitForSeconds(iterationTime);
        }
        animate = false;
    }

    IEnumerator AnimateNormalNumerator()
    {
        bool flag = false;        
        for (float t = 0; t <= animationTime; t += iterationTime)
        {
            EnergyBlock avaliableBlock = energy.GetFirstAvaliableBlock();
            EnergyBlock notAvaliableBlock = energy.GetFirstNotAvaliableBlock();
            float fillAmount = 0f;
            if (flag)
            {
                if (avaliableBlock != null)
                    fillAmount = ((float)avaliableBlock.index + 1f) / (float)energy.MaxUnits;
                else
                    fillAmount = 0f;
            }
            else
            {
                if (notAvaliableBlock != null)
                    fillAmount = ((float)notAvaliableBlock.index + 1f) / (float)energy.MaxUnits;
                else
                    fillAmount = 1f;
            }
            energyImage.fillAmount = fillAmount;

            flag = !flag;
            yield return new WaitForSeconds(iterationTime);
        }
        animate = false;
    }

    /// <summary>
    /// Считаем обрезку изображения
    /// </summary>
    void CalculateImageFillSlice()
    {
        EnergyBlock block = energy.GetFirstAvaliableBlock();
        float fillAmount = 0f;
        if (block != null)
            fillAmount = ((float)block.index + 1f) / (float)energy.MaxUnits;

        energyImage.fillAmount = fillAmount;
    }

    /// <summary>
    /// Пересчитываем время до следующего восстановления энергии в человеческий вид
    /// </summary>
    /// <returns></returns>
    string GetTimerString()
    {
        int currentUnixTime = Timers.Instance.UnixTimestamp;
        int regenerationTime = energy.GetNextBlockRegenerationTime();

        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(Mathf.Abs(regenerationTime - currentUnixTime));

        if (regenerationTime > 0)
            return System.String.Format("{0}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        else
            return "0:00";
    }
}
