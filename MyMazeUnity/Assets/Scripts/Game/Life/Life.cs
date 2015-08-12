using UnityEngine;
using System.Collections;

public class Life : MonoBehaviour, ISavingElement {

    public Deligates.SimpleEvent OnEmptyLife;
    public Deligates.IntegerEvent OnRestoreLife;
    public Deligates.IntegerEvent OnUseLife;

    /// <summary>
    /// Время в секундах за которое восстановится 1 жизнь
    /// </summary>
    public int restoreTime = 600;

    /// <summary>
    /// Максимальное число жизней
    /// </summary>
    private int _maxUnits = 5;

    /// <summary>
    /// Возвращаяет максимальное число жизней
    /// </summary>
    public int MaxUnits
    {
        get
        {
            return _maxUnits;
        }
    }
    
    /// <summary>
    /// Массив жизней
    /// </summary>
    private LifeBlock[] blocks;

    /// <summary>
    /// Возвращяет текущее количество жизней
    /// </summary>
    public int Units
    {
        get
        {
            int units = 0;
            foreach (LifeBlock block in blocks)
            {
                if (block.IsAvaliable)
                    units++;
            }
            return units;
        }
    }
    void Awake()
    {
        SetDefaultValues();
    }

    /// <summary>
    /// Устанавливает значения по умолчанию, для первого запуска
    /// </summary>
    void SetDefaultValues()
    {
        blocks = new LifeBlock[_maxUnits];
        for (int i = 0; i < _maxUnits; i++)
        {
            LifeBlock block = new LifeBlock();
            block.index = i;
            blocks[i] = block;
        }
    }

    void Update()
    {
        Regeneration();
    }

    /// <summary>
    /// Проверяет можно ли востановить жизнь и регенерирует его если возможно
    /// </summary>
    void Regeneration()
    {
        int unixTimestamp = Timers.Instance.UnixTimestamp;
        foreach (LifeBlock block in blocks)
        {
            if (!block.IsAvaliable && block.regenerationTime <= unixTimestamp)
                RestoreOneUnit();
        }
    }

    /// <summary>
    /// Используем жизнь
    /// </summary>
    /// <returns>true если можно, false если нельзя</returns>
    public bool Use()
    {
        LifeBlock block = GetFirstAvaliableBlock();
        if (block != null)
        {
            int j = 1;
            for (int i = block.index; i < _maxUnits; i++)
            {
                blocks[i].IsAvaliable = false;
                blocks[i].regenerationTime = Timers.Instance.UnixTimestamp + restoreTime * j;
                j++;
            }
            if (OnUseLife != null)
                OnUseLife(Units);
            Save();
            return true;
        }
        if (OnEmptyLife != null)
            OnEmptyLife();
        Debug.Log("Жизни закончились");
        return false;
    }

    /// <summary>
    /// Возвращает первый доступный блок жизни в сортировке справа налево
    /// </summary>
    /// <returns></returns>
    public LifeBlock GetFirstAvaliableBlock()
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
    /// Возвращает первый НЕ доступный блок жизней в сортировке слева направо
    /// </summary>
    /// <returns></returns>
    public LifeBlock GetFirstNotAvaliableBlock()
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
        LifeBlock block = GetFirstNotAvaliableBlock();
        if (block != null)
            return block.regenerationTime;
        else
            return 0;
    }

    /// <summary>
    /// Восстановить 1 Жизнь
    /// </summary>
    public void RestoreOneUnit()
    {
        LifeBlock block = GetFirstNotAvaliableBlock();
        if (block == null)
        {
            Debug.Log("Жизни полные");
            return;
        }
        block.Restore();
        if (OnRestoreLife != null)
            OnRestoreLife(block.index);
        int j = 1;
        for (int i = block.index + 1; i < _maxUnits; i++)
        {
            blocks[i].IsAvaliable = false;
            blocks[i].regenerationTime = Timers.Instance.UnixTimestamp + restoreTime * j;
            j++;
        }
    }

    /// <summary>
    /// Востанавливает все ячейки
    /// </summary>
    public void RestoreAllUnits()
    {
        int startIndex = 0;
        LifeBlock avBlock = GetFirstAvaliableBlock();
        if (avBlock != null)
            startIndex = System.Array.IndexOf(blocks, avBlock);
        for (int i = startIndex; i < _maxUnits; i++)
        {
            RestoreOneUnit();
        }
        Save();
    }

    /// <summary>
    /// Сохранить в PlayerPrefs значения жизней
    /// </summary>
    public void Save()
    {
        foreach (LifeBlock block in blocks)
        {
            PlayerPrefs.SetInt("Life#" + block.index.ToString() + "#IsAvaliable", System.Convert.ToInt32(block.IsAvaliable));
            PlayerPrefs.SetInt("Life#" + block.index.ToString() + "#regenerationTime", block.regenerationTime);
        }
    }

    /// <summary>
    /// Загрузить из PlayerPrefs данные о жизнях
    /// </summary>
    public void Load()
    {
        foreach (LifeBlock block in blocks)
        {
            if (PlayerPrefs.HasKey("Life#" + block.index.ToString() + "#IsAvaliable"))
                block.IsAvaliable = System.Convert.ToBoolean(PlayerPrefs.GetInt("Life#" + block.index.ToString() + "#IsAvaliable"));
            if (PlayerPrefs.HasKey("Life#" + block.index.ToString() + "#regenerationTime"))
                block.regenerationTime = PlayerPrefs.GetInt("Life#" + block.index.ToString() + "#regenerationTime");
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
