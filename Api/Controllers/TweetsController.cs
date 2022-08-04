using Api.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/v1.0/tweets/[controller]")]
    [ApiController]
    public class TweetsController : ControllerBase
    {
        [HttpGet]
        public List<Tweet> Get()
        {
            var tweets = new List<Tweet> {
                new Tweet
                {
                    Id =Guid.NewGuid(),
                    Text = "Text",
                },
                new Tweet
                {
                    Id = Guid.NewGuid(),
                    Text = "Text 2",
                }};

            return tweets;
        }
    }
}
