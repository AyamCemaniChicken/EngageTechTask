using System;
using Shared.Common.Attributes;

namespace Shared.Mailchimp.Model.Entity.Response
{
	[Endpoint("/members")]
	public class MembersResponse<T> where T: BaseMergeFields
	{
		public List<Member<T>> Members { get; set; }
	}
}

