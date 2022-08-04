using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dto
{
    public class Tweet
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public Guid User { get; set; }
    }
}
