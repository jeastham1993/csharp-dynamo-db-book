using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.ViewModels
{
    public class AddMemberToOrgDTO
    {
        public string Username { get; set; }

        public string Role { get; set; }
    }
}
