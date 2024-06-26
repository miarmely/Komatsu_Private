﻿namespace Entities.ViewModels
{
	public record MachineView
	{
		public Guid Id { get; init; }
		public string MainCategoryName { get; init; }
		public string SubCategoryName { get; init; }
		public string Model { get; init; }
		public string BrandName { get; init; }
		public string HandStatus { get; init; }
		public Int16 Stock { get; init; }
		public Int16 Rented { get; init; }
		public Int16 Sold { get; init; }
		public Int16 Year { get; init; }
		public string ImageName { get; init; }
		public string VideoName { get; init; }
		public string PdfName { get; init; }
		public DateTime CreatedAt { get; init; }
	}


	public record MachineViewForMobile : MachineView
	{
		public string Description { get; set; }	
	}


	public record MachineViewForPanel : MachineView
	{
		public IDictionary<string, string> Descriptions { get; } 
			= new Dictionary<string, string>();
	}


	public record DescriptionPartOfMachineView(
		string Language,
		string Description);
}