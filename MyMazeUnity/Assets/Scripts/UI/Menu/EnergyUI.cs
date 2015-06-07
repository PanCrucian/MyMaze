using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnergyUI : MonoBehaviour {

    public bool isGameUI = false;
    public float emptyAnimationTime = 0.75f;
    public float smoothTime = 14.75f;
    public Image energyImage;
    public Text timerText;
    public GameObject premiumContainer;

    private Energy energy;
    private InApps inApps;
    private bool animate = false;
    private float barSmoothVelocity;

    void Start()
    {
        energy = MyMaze.Instance.Energy;
        inApps = MyMaze.Instance.InApps;
        CalculateImageFillSlice(false);
    }

    void Update()
    {
        if (inApps.IsPremium)
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
            return;
        }
        if (!isGameUI)
        {
            timerText.text = GetTimerString();
            ControlPremiumContainer();
        }
        if (!animate)
            CalculateImageFillSlice(true);
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
    /// Запускаем процессы анимации блока, когда плохое использование
    /// </summary>
    public void AnimateBad()
    {
        if (animate)
            return;
        animate = true;
        GetComponent<SoundsPlayer>().PlayOneShootSound();
        StartCoroutine(AnimateBadNumerator());
    }

    IEnumerator AnimateBadNumerator()
    {
        Color color = energyImage.color;
        energyImage.fillAmount = 1f;
        for (float t = 0; t <= emptyAnimationTime; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0f, 1f, t / emptyAnimationTime);
            ApplyImageColor(color);
            yield return new WaitForEndOfFrame();
        }
        color.a = 1f;
        ApplyImageColor(color);
        yield return new WaitForEndOfFrame();
        for (float t = 0; t <= emptyAnimationTime; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1f, 0f, t / emptyAnimationTime);
            ApplyImageColor(color);
            yield return new WaitForEndOfFrame();
        }
        color.a = 1f;
        ApplyImageColor(color);
        energyImage.fillAmount = 0f;
        animate = false;
    }

    void ApplyImageColor(Color color)
    {
        energyImage.color = color;
    }

    /// <summary>
    /// Считаем обрезку изображения
    /// </summary>
    void CalculateImageFillSlice(bool smooth)
    {
        EnergyBlock block = energy.GetFirstAvaliableBlock();
        float fillAmount = energyImage.fillAmount;
        if (block != null)
        {
            if (smooth)
                fillAmount = Mathf.SmoothDamp(fillAmount, ((float)block.index + 1f) / (float)energy.MaxUnits, ref barSmoothVelocity, smoothTime * Time.deltaTime);
            else
                fillAmount = ((float)block.index + 1f) / (float)energy.MaxUnits;
        }
        else
        {
            if (smooth)
                fillAmount = Mathf.SmoothDamp(fillAmount, 0f, ref barSmoothVelocity, smoothTime * Time.deltaTime);
            else
                fillAmount = 0f;
        }

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
