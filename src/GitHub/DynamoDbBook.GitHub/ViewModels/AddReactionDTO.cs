using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.ViewModels
{
    public class AddReactionDTO
    {
        public string Username { get; set; }

        public string Reaction { get; set; }
    }
}
