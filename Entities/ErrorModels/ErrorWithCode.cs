﻿using System.Text.Json;

namespace Entities.ErrorModels
{
	public class ErrorWithCode
	{
        public int StatusCode { get; set; }
		public string ErrorCode { get; set; }
		public string? ErrorDescription { get; set; }
		public string? Message { get; set; }

		public override string ToString() =>
			JsonSerializer.Serialize(this);
	}
}
