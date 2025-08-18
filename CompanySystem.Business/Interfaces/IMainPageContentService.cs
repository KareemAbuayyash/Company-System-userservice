using CompanySystem.Business.DTOs;

namespace CompanySystem.Business.Interfaces
{
    public interface IMainPageContentService
    {
        Task<IEnumerable<MainPageContentListDto>> GetAllMainPageContentsAsync();
        Task<MainPageContentDto?> GetMainPageContentByIdAsync(int id);
        Task<MainPageContentDto?> GetMainPageContentBySectionAsync(string sectionName);
        Task<MainPageContentDto> CreateMainPageContentAsync(CreateMainPageContentDto createDto);
        Task<MainPageContentDto?> UpdateMainPageContentAsync(int id, UpdateMainPageContentDto updateDto);
        Task<bool> DeleteMainPageContentAsync(int id);
        Task<IEnumerable<PublicMainPageDto>> GetPublicMainPageContentsAsync();
        Task<bool> MainPageContentExistsAsync(int id);
        Task<bool> SectionExistsAsync(string sectionName, int? excludeId = null);
    }
}
