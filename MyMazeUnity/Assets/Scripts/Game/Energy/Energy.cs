using UnityEngine;
using System.Collections;

public class Energy : MonoBehaviour, ISavingElement
{

    /// <summary>
    /// Время в секундах за которое восстановится 1 блок энергии
    /// </summary>
    public int restoreTime = 300;

    /// <summary>
    /// Количество блоков энергии, необходимое для запуска уровня
    /// </summary>
    public int levelCost = 1;    

    /// <summary>
    /// Максимальное число энергоблоков
    /// </summary>
    private int _maxUnits = 8;

    /// <summary>
    /// Возвращаяет максимальное число энергоблоков
    /// </summary>
    public int MaxUnits
    {
        get
        {
            return _maxUnits;
        }
    }

    /// <summary>
    /// Возвращяет текущее количество энергоблоков
    /// </summary>
    public int Units
    {
        get
        {
            int units = 0;
            foreach (EnergyBlock block in blocks)
            {
                if (block.IsAvaliable)
                    units++;
            }
            return units;
        }
    }

    /// <summary>
    /// Массив энергоблоков
    /// </summary>
    private EnergyBlock[] blocks;

    void Start()
    {
        SetDefaultValues();
    }

    void Update()
    {
        Regeneration();
    }

    /// <summary>
    /// Проверяет можно ли востановить энергоблок и регенерирует его если возможно
    /// </summary>
    void Regeneration()
    {
        int unixTimestamp = Timers.Instance.UnixTimestamp;
        foreach (EnergyBlock block in blocks)
        {
            if (!block.IsAvaliable && block.regenerationTime <= unixTimestamp)
                block.IsAvaliable = true;
        }
    }

    /// <summary>
    /// Восстановить 1 еденицу энергии
    /// </summary>
    public void RestoreOneUnit()
    {
        EnergyBlock block = GetFirstNotAvaliableBlock();
        if (block == null)
        {
            Debug.Log("Энергия полная");
            return;
        }
        block.Restore();
        int j = 1;
        for (int i = block.index + 1; i < _maxUnits; i++)
        {
            blocks[i].IsAvaliable = false;
            blocks[i].regenerationTime = Timers.Instance.UnixTimestamp + restoreTime * j;
            j++;
        }
    }

    /// <summary>
    /// Устанавливает значения по умолчанию, для первого запуска
    /// </summary>
    void SetDefaultValues()
    {
        blocks = new EnergyBlock[_maxUnits];
        for (int i = 0; i < _maxUnits; i++)
        {
            EnergyBlock block = new EnergyBlock();
            block.index = i;
            blocks[i] = block;
        }
    }

    /// <summary>
    /// Используем энергию
    /// </summary>
    /// <returns>true если можно, false если нельзя</returns>
    public bool Use()
    {
        EnergyBlock block = GetFirstAvaliableBlock();
        if (block != null)
        {
            int j = 1;
            for (int i = block.index; i < _maxUnits; i++)
            {
                blocks[i].IsAvaliable = false;
                blocks[i].regenerationTime = Timers.Instance.UnixTimestamp + restoreTime * j;
                j++;
            }
            return true;
        }
        Debug.Log("Энергия закончилась");
        return false;
    }

    /// <summary>
    /// Возвращает первый доступный энергоблок в сортировке справа налево
    /// </summary>
    /// <returns></returns>
    public EnergyBlock GetFirstAvaliableBlock()
    {
        for (int i = _maxUnits - 1; i >= 0; i--)
        {
            if (blocks[i].IsAvaliable)
            {
                return blocks[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Возвращает первый НЕ доступный энергоблок в сортировке слева направо
    /// </summary>
    /// <returns></returns>
    public EnergyBlock GetFirstNotAvaliableBlock()
    {
        for (int i = 0; i < _maxUnits; i++)
        {
            if (!blocks[i].IsAvaliable)
            {
                return blocks[i];
            }
        }
        return null;
    }

    /// <summary>
    /// Возвращает время регенирации до следующего блока
    /// </summary>
    /// <returns>Unix TimeStamp</returns>
    public int GetNextBlockRegenerationTime()
    {
        EnergyBlock block = GetFirstNotAvaliableBlock();
        if (block != null)
            return block.regenerationTime;
        else
            return 0;
    }

    /// <summary>
    /// Время регенерации последнего блока
    /// </summary>
    /// <returns>Unix TimeStamp</returns>
    public int GetLastBlockRegenerationTime()
    {
        if (blocks[_maxUnits - 1].IsAvaliable)
            return 0;
        return blocks[_maxUnits - 1].regenerationTime;
    }

    /// <summary>
    /// Сохранить в PlayerPrefs значения энергии
    /// </summary>
    public void Save()
    {
        foreach (EnergyBlock block in blocks)
        {
            PlayerPrefs.SetInt("Energy#" + block.index.ToString() + "#IsAvaliable", System.Convert.ToInt32(block.IsAvaliable));
            PlayerPrefs.SetInt("Energy#" + block.index.ToString() + "#regenerationTime", block.regenerationTime);
        }
    }

    /// <summary>
    /// Загрузить из PlayerPrefs данные о энергии
    /// </summary>
    public void Load()
    {
        foreach (EnergyBlock block in blocks)
        {
            if (PlayerPrefs.HasKey("Energy#" + block.index.ToString() + "#IsAvaliable"))
                block.IsAvaliable = System.Convert.ToBoolean(PlayerPrefs.GetInt("Energy#" + block.index.ToString() + "#IsAvaliable"));
            if (PlayerPrefs.HasKey("Energy#" + block.index.ToString() + "#regenerationTime"))
                block.regenerationTime = PlayerPrefs.GetInt("Energy#" + block.index.ToString() + "#regenerationTime");
        }
    }

    /// <summary>
    /// Сбросить сохранения
    /// </summary>
    public void ResetSaves()
    {
        throw new System.NotImplementedException();
    }
}
