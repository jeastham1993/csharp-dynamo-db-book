using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.ViewModels
{
    public class CreateOrganizationDTO
    {
        public string OrganizationName { get; set; }

        public string OwnerName { get; set; }
    }
}
