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
    private LifeBlock[] _blocks;

    /// <summary>
    /// Возвращяет текущее количество жизней
    /// </summary>
    public int Units
    {
        get
        {
            int units = 0;
            foreach (LifeBlock block in _blocks)
            {
                if (block.IsAvaliable)
                    units++;
            }
            return units;
        }
    }

    public LifeBlock[] Blocks
    {
        get
        {
            return _blocks;
        }
    }

    void Awake()
    {
        SetDefaultValues();
    }

    void Start()
    {
        MyMaze.Instance.InApps.OnUnlimitedLivesBuyed += OnUnlimitedLivesBuyed;
        MyMaze.Instance.InApps.OnFiveLivesBuyed += OnFiveLivesBuyed;
    }

    /// <summary>
    /// Купили 5 жизней
    /// </summary>
    void OnFiveLivesBuyed()
    {
        if (MyMaze.Instance.InApps.ConsumeFiveLives())
            RestoreAllUnits();
    }

    /// <summary>
    /// Купили бесконечные жизни
    /// </summary>
    void OnUnlimitedLivesBuyed()
    {
        RestoreAllUnits();
    }

    /// <summary>
    /// Устанавливает значения по умолчанию, для первого запуска
    /// </summary>
    void SetDefaultValues()
    {
        _blocks = new LifeBlock[_maxUnits];
        for (int i = 0; i < _maxUnits; i++)
        {
            LifeBlock block = new LifeBlock();
            block.index = i;
            _blocks[i] = block;
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
        foreach (LifeBlock block in _blocks)
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
        //не удалось пройти уровень
        MyMaze.Instance.LastSelectedLevel.Fail();

        //если купили бесконечные жизни, то не тратим их
        if (MyMaze.Instance.InApps.IsOwned(ProductTypes.UnlimitedLives))
            return true;

        LifeBlock blockAvb = GetFirstAvaliableBlock();
        if (blockAvb != null)
        {
            int timeOffset = 0; //отступ для перенятия остатка таймера от следующего сердца
            LifeBlock blockNotAvb = GetFirstNotAvaliableBlock();
            if (blockNotAvb != null)
                timeOffset = restoreTime - Mathf.Abs(blockNotAvb.regenerationTime - Timers.Instance.UnixTimestamp);

            int j = 1;
            for (int i = blockAvb.index; i < _maxUnits; i++)
            {
                _blocks[i].IsAvaliable = false;
                _blocks[i].regenerationTime = Timers.Instance.UnixTimestamp + restoreTime * j - timeOffset;
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
            if (_blocks[i].IsAvaliable)
            {
                return _blocks[i];
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
            if (!_blocks[i].IsAvaliable)
            {
                return _blocks[i];
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
    public void RestoreOneUnit(bool save)
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
            _blocks[i].IsAvaliable = false;
            _blocks[i].regenerationTime = Timers.Instance.UnixTimestamp + restoreTime * j;
            j++;
        }
        if (save)
            Save();
    }

    public void RestoreOneUnit()
    {
        RestoreOneUnit(true);
    }

    /// <summary>
    /// Востанавливает все ячейки
    /// </summary>
    public void RestoreAllUnits()
    {
        int startIndex = 0;
        LifeBlock avBlock = GetFirstAvaliableBlock();
        if (avBlock != null)
            startIndex = System.Array.IndexOf(_blocks, avBlock);
        for (int i = startIndex; i < _maxUnits; i++)
        {
            RestoreOneUnit(false);
        }
        Save();
    }

    /// <summary>
    /// Сохранить в PlayerPrefs значения жизней
    /// </summary>
    public void Save()
    {
        foreach (LifeBlock block in _blocks)
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
        foreach (LifeBlock block in _blocks)
        {
            if (PlayerPrefs.HasKey("Life#" + block.index.ToString() + "#IsAvaliable"))
                block.IsAvaliable = System.Convert.ToBoolean(PlayerPrefs.GetInt("Life#" + block.index.ToString() + "#IsAvaliable"));
            if (PlayerPrefs.HasKey("Life#" + block.index.ToString() + "#regenerationTime"))
                block.regenerationTime = PlayerPrefs.GetInt("Life#" + block.index.ToString() + "#regenerationTime");
        }

        RestoreLivesAfterLoad();
    }

    /// <summary>
    /// Восстанавливаем жизни после загрузки сохранений
    /// Используем этот метод вместо Regeneration() потому что метод RestoreOneUnit(),
    /// Перезаписывает следующие блоки в линейке жизней и может возникнуть ситуация что мы были оффлайн 1 час,
    /// Но восстановится только 1 ячейка жизней
    /// Короче юзаем этот метод после загрузки и все будет ОК
    /// </summary>
    void RestoreLivesAfterLoad()
    {
        int unixTimestamp = Timers.Instance.UnixTimestamp;
        for (int i = _maxUnits - 1; i >= 0; i--)
        {
            LifeBlock block = _blocks[i];
            if (!block.IsAvaliable && block.regenerationTime <= unixTimestamp)
                block.Restore();
        }
    }

    /// <summary>
    /// Если жизни чануть багать, возможно это надо будет убрать
    /// </summary>
    /// <param name="pause"></param>
    void OnApplicationPause(bool pause)
    {
        if (pause)
            Save();
        else
            Load();
    }

    /// <summary>
    /// Получить последний блок жизней
    /// </summary>
    /// <returns></returns>
    public LifeBlock GetLastBlock()
    {
        return _blocks[MaxUnits - 1];
    }

    /// <summary>
    /// Сбросить сохранения
    /// </summary>
    public void ResetSaves()
    {
        throw new System.NotImplementedException();
    }
}
