[System.Serializable]
public class Booster 
{
    public string name;
    public BoosterTypes type;
    public Level avaliableAtLevel;
    public bool IsClosed
    {
        get
        {
            return _isClosed;
        }
    }
    private bool _isClosed = true;
}
