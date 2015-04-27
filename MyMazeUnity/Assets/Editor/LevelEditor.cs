using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;


[CustomEditor(typeof(Level))]
[CanEditMultipleObjects]
public class LevelEditor : Editor
{
    public bool allowAutoLevelNaming = true;
    public bool allowAutoSort = true;
    int starIndexForActions;

    GUIStyle textStyle01 = new GUIStyle();
    private bool[] foldout = {true};

    public override void OnInspectorGUI()
    {
        textStyle01.normal.textColor = Color.yellow;
        DrawDefaultInspector();

        Level level = (Level)target;
        if (level == null)
            return;
        string newname = "Level_" + level.levelName;
        if (!target.name.Equals(newname))
        {
            target.name = "Level_" + level.levelName;
        }
        StarControl();

        EditorGUILayout.Space();
        allowAutoLevelNaming = EditorGUILayout.Toggle("Авто имя:", allowAutoLevelNaming);
        if (allowAutoLevelNaming)
        {
            Transform parent = level.transform.parent;
            newname = parent.GetComponent<Pack>().packName + (level.transform.GetSiblingIndex() + 1).ToString();
            if (level.levelName == null)
                return;
            if (!level.levelName.Equals(newname))
            {
                level.levelName = newname;
            }

            newname = (parent.GetSiblingIndex() + 1).ToString() + "-" + (level.transform.GetSiblingIndex() + 1).ToString();
            if (!level.displayText.Equals(newname))
            {
                level.displayText = newname;
            }
        }

        //EditorGUILayout.Space();
        foldout[0] = EditorGUILayout.Foldout(foldout[0], new GUIContent() { text = "Управление звездами" });
        if (foldout[0])
        {
            if (level.GetHiddenStar() == null)
            {
                if (GUILayout.Button("Добавить секретную звезду"))
                {
                    AddHiddenStar();
                }
            }
            else
            {
                GUILayout.Label("Здесь есть секретная звезда :3", textStyle01);
                if (GUILayout.Button("Удалить секретную звезду"))
                {
                    RemoveHiddenStar();
                }
            }
            allowAutoSort = EditorGUILayout.Toggle("Авто сортировка:", allowAutoSort);
            if (!allowAutoSort)
            {
                if (GUILayout.Button("Сортировать звезды"))
                {
                    level.stars.Sort();
                }
            }
            //starIndexForActions = EditorGUILayout.IntField("ID звезды:", starIndexForActions);
            starIndexForActions = (int)EditorGUILayout.Slider("ID звезды:", (float)starIndexForActions, 0, (float)(level.stars.Count - 1));
            if (starIndexForActions > level.stars.Count - 1)
                starIndexForActions = level.stars.Count - 1;
            if (starIndexForActions < 0)
                starIndexForActions = 0;
            EditorGUILayout.Toggle("Подобрана:", level.stars[starIndexForActions].IsCollected);
            if (!level.stars[starIndexForActions].IsCollected)
            {
                if (GUILayout.Button("Подобрать"))
                {
                    level.stars[starIndexForActions].Collect();
                }
            }
            else
            {
                if (GUILayout.Button("Бросить"))
                {
                    level.stars[starIndexForActions].Lose();
                }
            }
        }
        
        //автосортировка звезд
        if (allowAutoSort)
            level.stars.Sort();
        if(!Application.isPlaying)
            EditorUtility.SetDirty(level);
    }

    void AddHiddenStar()
    {
        Star star = new Star();
        star.IsHidden = true;
        Level level = (Level)target;
        level.stars.Add(star);
    }

    void RemoveHiddenStar()
    {
        Level level = (Level)target;
        foreach (Star star in level.stars)
        {
            if (star.IsHidden)
            {
                level.stars.Remove(star);
                break;
            }                
        }
    }

    void StarControl()
    {
        Level level = (Level)target;
        string newname;
        List<Star> simpleStars = level.GetSimpleStars();

        //нехватки звезд
        if (simpleStars.Count < 3) { 
            Debug.LogWarning("Звезд должно быть не меньше 3-х!");
            for (int i = 0; i < 3 - simpleStars.Count; i++)
            {
                Star star = new Star();
                level.stars.Add(star);
            }
        }
        
        //Проверим чтобы значения ходов не были отрицательными
        foreach (Star star in level.stars)
        {
            //Звезды не могут быть отрицательными
            if (star.movesToGet < 0)
                star.movesToGet = 0;
        }

        List<Star> hiddenStars = level.GetHiddenStars();
        if (hiddenStars.Count > 0)
        {
            //Если скрытых звезд больше 2-х то выключим все
            if (hiddenStars.Count > 1)
                foreach (Star star in hiddenStars)
                    star.IsHidden = false;

            //поставим число ходов на 0
            hiddenStars[0].movesToGet = 0;
            newname = "HiddenStar";
            if (!hiddenStars[0].starName.Equals(newname))
                hiddenStars[0].starName = newname;
        }
        hiddenStars = level.GetHiddenStars();

        //Если звезд больше положеного удалим их
        if (simpleStars.Count > 3)
        {
            Debug.LogWarning("Удалил лишние звезды");
            for (int i = simpleStars.Count - 1; i >= 3; i--)
            {
                level.stars.Remove(simpleStars[i]);
            }
        }

        newname = "Star";
        foreach (Star star in simpleStars)
        {
            if (!star.starName.Equals(newname))
                star.starName = newname;
        }
    }

}
