﻿namespace Entities.ViewModels.FormViews
{
	public record UnansweredGeneralCommFormViewForOneUser
	{
		public int FormId { get; init; }
		public string FirstName { get; init; }
		public string LastName { get; init; }
		public string Company { get; init; }
		public string Phone { get; init; }
		public string Email { get; init; }
		public string CityName { get; init; }
		public string County { get; init; }
		public string Subject { get; init; }
		public string Message { get; init; }
		public DateTime CreatedAt { get; init; }
	}

	public record AnsweredGeneralCommFormViewForOneUser
		: UnansweredGeneralCommFormViewForOneUser
	{
		public string AnswererFirstName { get; init; }
		public string AnswererLastName { get; init; }
		public string AnswererEmail { get; init; }
		public DateTime AnsweredDate { get; init; }
	}

	public record AllGeneralCommFormViewForOneUser 
		: AnsweredGeneralCommFormViewForOneUser
	{
        public bool IsAnswered { get; init; }
	}
}