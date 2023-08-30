﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.RelationModels
{
	public class UserAndRole
	{
		[Key]
        public int Id { get; set; }
		public Guid UserId { get; set; }
		
		[Column(TypeName = "tinyint")]
		public int RoleId { get; set; }
	}
}