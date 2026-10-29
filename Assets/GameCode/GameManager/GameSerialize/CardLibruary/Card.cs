public class Card
{
    public string key;
    public bool discover;
    Card() { }
    public static Card Create(string key)
    {
        var instance = new Card();
        instance.key = key;
        return instance;
    }
}
