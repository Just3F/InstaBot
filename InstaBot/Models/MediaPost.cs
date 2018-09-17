using System.Collections;
using System.Collections.Generic;

namespace InstaBot.Service.Models
{
    public class MediaPost
    {
        public MediaPost()
        {
            URICollection = new List<string>();
        }
        public IEnumerable<string> URICollection { get; set; }
        public string Caption { get; set; }
    }
}
