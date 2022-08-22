namespace Infrastructure.Exceptions
{
    public class TweetNotFoundExeption : Exception
    {
        public TweetNotFoundExeption(string message) : base(message)
        {

        }
    }
}
