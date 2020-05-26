using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.ViewModels
{
    public class AddCommentDTO
    {
        public string Username { get; set; }

        public string Content { get; set; }
    }
}
