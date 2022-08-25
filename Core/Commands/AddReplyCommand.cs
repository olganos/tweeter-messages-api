namespace Core.Commands
{
    public class AddReplyCommand
    {
        public string TweetId { get; set; }
        public string Text { get; set; }
        public string UserName { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}
