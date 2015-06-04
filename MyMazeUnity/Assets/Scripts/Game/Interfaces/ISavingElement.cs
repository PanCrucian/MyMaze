using System.Collections;

/// <summary>
/// етоды сохранения и загрузки в PlayerPrefs
/// </summary>
public interface ISavingElement {
    void Save();
    void Load();
    void ResetSaves();
}
