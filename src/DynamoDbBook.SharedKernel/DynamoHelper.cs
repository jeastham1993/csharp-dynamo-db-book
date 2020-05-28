using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using Newtonsoft.Json;

namespace DynamoDbBook.SharedKernel
{
    public static class DynamoHelper
    {
	    public static T CreateFromItem<T>(Dictionary<string, AttributeValue> item)
	    {
		    var data = item.FirstOrDefault(p => p.Key == "Data");

		    return JsonConvert.DeserializeObject<T>(Document.FromAttributeMap(data.Value.M).ToJson());
	    }
    }
}
