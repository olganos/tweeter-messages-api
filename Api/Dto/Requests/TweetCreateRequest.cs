using System.ComponentModel.DataAnnotations;

namespace Api.Dto.Requests
{
    public class TweetCreateRequest
    {
        [StringLength(144, ErrorMessage = "Not longer than 144 characters")]
        public string Text { get; set; }
    }
}