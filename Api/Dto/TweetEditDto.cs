using System.ComponentModel.DataAnnotations;

namespace Api.Dto
{
    public class TweetEditDto
    {
        [StringLength(144, ErrorMessage = "Not longer than 144 characters")]
        public string Text { get; set; }
    }
}