using System.ComponentModel.DataAnnotations;

namespace Api.Dto.Requests
{
    public class TweetEditRequest
    {
        [StringLength(144, ErrorMessage = "No longer than 144 characters")]
        public string Text { get; set; }
    }
}