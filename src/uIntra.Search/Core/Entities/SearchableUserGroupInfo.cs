﻿using System;

namespace Uintra.Search
{
	public class SearchableUserGroupInfo
	{
		public Guid GroupId { get; set; }
		public bool IsAdmin { get; set; }
		public bool IsCreator { get; set; }
	}
}