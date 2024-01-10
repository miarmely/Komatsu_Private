﻿using Entities.Attributes;
using System.ComponentModel.DataAnnotations;


namespace Entities.QueryParameters
{
	public record MachineParamsForUpdate : LanguageParams
	{
		[Required] public Guid Id { get; init; }
		[Required] public string OldMainCategoryName { get; init; }
		[Required] public string OldSubCategoryName { get; init; }
	}

	public record MachineParamsForDisplaySubCategoryNames : LanguageParams
	{
		[Required] public string? MainCategoryName { get; init; }
	}

	public record MachineParamsForCreate : LanguageParams
	{
		[Required] public string ImageFolderPathAfterWwwroot { get; init; }
		[Required] public string VideoFolderPathAfterWwwroot { get; init; }
		[Required] public string PdfFolderPathAfterWwwroot { get; init; }
	}

	public record MachineParamsForDelete : LanguageParams
	{
		[Required] public string ImageFolderPathAfterWwwroot { get; init; }
		[Required] public string PdfFolderPathAfterWwwroot { get; init; }
	}


	public record MachineParamsForDisplayOneMachine : LanguageParams
	{
		[Required] public Guid MachineId { get; init; }
	}
	
	public record MachineParamsForUpdateFile : LanguageParams
	{
		[Required] [MiarLength(1, 50, "Dosya Adı", "File Name")]
		public string NewFileName { get; init; }


		[Required] [MiarLength(1, 50, "Dosya Adı", "File Name")]
		public string OldFileName { get; init; }

		[Required] 
		public string FileFolderPathAfterWwwroot { get; init; }
	}
}
