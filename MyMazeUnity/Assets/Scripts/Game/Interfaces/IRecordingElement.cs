/// <summary>
/// Интерфейс помошник, от которого должны наследоваться элементы участвующие в ведение истории состояний элемента
/// </summary>
public interface IRecordingElement {
    void Record(int move);

    void RecordsReset();

    void ReturnToMove(int move);

}
