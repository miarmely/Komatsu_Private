﻿namespace Entities.DtoModels
{
    public class MachineDtoForCreate
    {
		public string? MainCategoryName { get; init; }
		public string? SubCategoryName { get; init; }
		public string? BrandName { get; init; }
        public string? Model { get; init; }
		public int? Year { get; init; }
		public bool IsSecondHand { get; init; }
        public int? Stock { get; init; }
		public string? ImagePath { get; init; }
	}
}
