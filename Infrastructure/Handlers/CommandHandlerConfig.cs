namespace Infrastructure.Handlers
{
    public class CommandHandlerConfig
    {
        public CommandHandlerConfig(string createTweetTopicName, string addReplyTopicName)
        {
            CreateTweetTopicName = createTweetTopicName;
            AddReplyTopicName = addReplyTopicName;
        }

        public string CreateTweetTopicName { get; }
        public string AddReplyTopicName { get; }
    }
}
