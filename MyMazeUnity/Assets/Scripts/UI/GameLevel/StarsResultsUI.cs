using UnityEngine;
using System.Collections;

public class StarsResultsUI : MonoBehaviour {

    Animator[] stars;

    public static StarsResultsUI Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Не могу найти экземпляр класса StarsResultsUI");
            return _instance;
        }
    }

    private static StarsResultsUI _instance;

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        stars = GetComponentsInChildren<Animator>();
    }

    /// <summary>
    /// Возвращает аниматор серой звезды, которая не подобрана
    /// </summary>
    public Animator GetNotCollectedStar()
    {
        foreach (Animator star in stars)
        {
            if (!star.GetBool("IsCollected"))
                return star;
        }
        return null;
    }

    public void Reset()
    {
        foreach (Animator star in stars)
        {
            star.SetBool("IsCollected", false);
        }
    }
}
