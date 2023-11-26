﻿using Dapper;
using Entities.ViewModels;

namespace Repositories.Contracts
{
    public interface IFileRepository : IRepositoryBase
    {
        Task UploadSliderAsync(DynamicParameters parameters);
        Task<IEnumerable<SliderView>?> GetAllSlidersAsync(
            DynamicParameters parameters);
        Task TruncateAllSlidersAsync();
        Task<string?> GetSliderPathBySliderNoAsync(DynamicParameters parameters);
    }
}
