public interface ILevel {

    void Open();

    void Close();

    void Pass();

    bool HasHiddenStar();

    Star GetHiddenStar();

    System.Collections.Generic.List<Star> GetHiddenStars();

    System.Collections.Generic.List<Star> GetSimpleStars();
}
