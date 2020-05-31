using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace DynamoDbBook.GitHub.Domain.Entities.Reactions
{
    public static class ReactionHelpers
	{
		private static Dictionary<string, string> _reactionTypes;

		public static Dictionary<string, string> ReactionTypes
		{
			get
			{
				if (_reactionTypes == null)
				{
					_reactionTypes = new Dictionary<string, string>(8);
					_reactionTypes.Add("+1", "+1");
					_reactionTypes.Add("-1", "-1");
					_reactionTypes.Add("smile", "Smile");
					_reactionTypes.Add("celebration", "Celebration");
					_reactionTypes.Add("disappointed", "Disappointed");
					_reactionTypes.Add("heart", "Heart");
					_reactionTypes.Add("rocket", "Rocket");
					_reactionTypes.Add("eyes", "Eyes");
				}

				return _reactionTypes;
			}
		}
    }
}
