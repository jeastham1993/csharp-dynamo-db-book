using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using DynamoDbBook.GitHub.Domain.Entities;

using Newtonsoft.Json;

namespace DynamoDbBook.GitHub.Infrastructure.Extensions
{
    public static class RepositoryExtensions
    {
	    public static string GetPk(
		    this Repository repository)
	    {
		    return $"REPO#{repository.OwnerName}#{repository.RepositoryName}";
	    }

	    public static string GetSk(
		    this Repository repository)
	    {
		    return $"REPO#{repository.OwnerName}#{repository.RepositoryName}";
	    }

		public static Dictionary<string, AttributeValue> AsKeys(this Repository repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(Repository));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			attributeMap.Add("PK", new AttributeValue(repository.GetPk()));
			attributeMap.Add("SK", new AttributeValue(repository.GetSk()));

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsItem(this Repository repository)
		{
			if (repository == null)
			{
				throw new ArgumentNullException(nameof(Repository));
			}

			var attributeMap = new Dictionary<string, AttributeValue>(5);

			foreach(var map in repository.AsKeys())
			{
				attributeMap.Add(map.Key, map.Value);	
			}

			foreach (var map in repository.AsGsi1())
			{
				attributeMap.Add(
					map.Key,
					map.Value);
			}

			foreach (var map in repository.AsGsi2())
			{
				attributeMap.Add(
					map.Key,
					map.Value);
			}

			foreach (var map in repository.AsGsi3())
			{
				attributeMap.Add(
					map.Key,
					map.Value);
			}

			attributeMap.Add("Type", new AttributeValue("Repository"));
			attributeMap.Add(
				"Data",
				new AttributeValue(){ M = repository.AsData()});

			return attributeMap;
		}

		public static Dictionary<string, AttributeValue> AsData(
			this Repository repository)
		{
			var document = Document.FromJson(JsonConvert.SerializeObject(repository));
			var documentAttributeMap = document.ToAttributeMap();

			return documentAttributeMap;
		}

		public static string GetGsi1Pk(this Repository repository)
		{
			return $"REPO#{repository.OwnerName.ToLower()}#{repository.RepositoryName.ToLower()}";
		}

		public static string GetGsi1Sk(this Repository repository)
		{
			return $"REPO#{repository.OwnerName.ToLower()}#{repository.RepositoryName.ToLower()}";
		}

		public static Dictionary<string, AttributeValue> AsGsi1(this Repository repository)
		{
			return new Dictionary<string, AttributeValue>(2)
			{
				{ "GSI1PK", new AttributeValue(repository.GetGsi1Pk()) },
				{ "GSI1SK", new AttributeValue(repository.GetGsi1Sk()) }
			};
		}

		public static string GetGsi2Pk(this Repository repository)
		{
			if (string.IsNullOrEmpty(repository.ForkOwner))
			{
				return $"REPO#{repository.OwnerName.ToLower()}#{repository.RepositoryName.ToLower()}";
			}
			else
			{
				return $"REPO#{repository.ForkOwner.ToLower()}#{repository.RepositoryName.ToLower()}";
			}
		}

		public static string GetGsi2Sk(this Repository repository)
		{
			if (string.IsNullOrEmpty(repository.ForkOwner))
			{
				return $"REPO#{repository.OwnerName.ToLower()}#{repository.RepositoryName.ToLower()}";
			}
			else
			{
				return $"FORK#{repository.OwnerName.ToLower()}";
			}
		}

		public static Dictionary<string, AttributeValue> AsGsi2(this Repository repository)
		{
			return new Dictionary<string, AttributeValue>(2)
			{
				{ "GSI2PK", new AttributeValue(repository.GetGsi2Pk()) },
				{ "GSI2SK", new AttributeValue(repository.GetGsi2Sk()) }
			};
		}

		public static Dictionary<string, AttributeValue> AsGsi3(this Repository repository)
		{
			return new Dictionary<string, AttributeValue>(2)
			{
				{ "GSI3PK", new AttributeValue($"ACCOUNT#{repository.OwnerName}") },
				{ "GSI3SK", new AttributeValue($"#{repository.UpdatedAt.ToString("o")}") }
			};
		}
    }
}
