namespace Api.Dto.Responses;

public class TweetCreateResponse
{
    public string Text { get; set; }
    public string UserName { get; set; }
    public DateTimeOffset Created { get; set; }
}