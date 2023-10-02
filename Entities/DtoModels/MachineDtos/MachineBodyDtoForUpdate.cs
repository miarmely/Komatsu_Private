﻿namespace Entities.DtoModels.MachineDtos
{
    public record MachineDtoForUpdate
    {
        public string? MainCategoryName { get; init; }
        public string? SubCategoryName { get; init; }
        public string? Model { get; init; }
        public string? BrandName { get; init; }
        public Int16? Stock { get; init; }
        public Int16? Rented { get; init; }
        public Int16? Sold { get; init; }
        public Int16? Year { get; init; }
        public string? HandStatus { get; init; }
        public string? DescriptionInTR { get; init; }
        public string? DescriptionInEN { get; init; }
        public string? ImagePath { get; init; }
        public string? PdfPath { get; init; }
    }
}