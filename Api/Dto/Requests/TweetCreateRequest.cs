using System.ComponentModel.DataAnnotations;

namespace Api.Dto.Requests
{
    public class TweetCreateRequest
    {
        [StringLength(144, ErrorMessage = "No longer than 144 characters")]
        public string Text { get; set; }

        [StringLength(50, ErrorMessage = "No longer than 50 characters")]
        public string Tag { get; set; }
    }
}