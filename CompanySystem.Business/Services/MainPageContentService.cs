using CompanySystem.Business.DTOs;
using CompanySystem.Business.Interfaces;
using CompanySystem.Data.Context;
using CompanySystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanySystem.Business.Services
{
    public class MainPageContentService : IMainPageContentService
    {
        private readonly CompanySystemDbContext _context;

        public MainPageContentService(CompanySystemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MainPageContentListDto>> GetAllMainPageContentsAsync()
        {
            return await _context.MainPageContents
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.SectionName)
                .Select(m => new MainPageContentListDto
                {
                    ContentId = m.ContentId,
                    SectionName = m.SectionName,
                    Title = m.Title,
                    ContentPreview = m.Content.Length > 100 ? m.Content.Substring(0, 100) + "..." : m.Content,
                    CreatedDate = m.CreatedDate,
                    UpdatedDate = m.UpdatedDate
                })
                .ToListAsync();
        }

        public async Task<MainPageContentDto?> GetMainPageContentByIdAsync(int id)
        {
            var content = await _context.MainPageContents
                .Where(m => m.ContentId == id && !m.IsDeleted)
                .FirstOrDefaultAsync();

            if (content == null)
                return null;

            return new MainPageContentDto
            {
                ContentId = content.ContentId,
                SectionName = content.SectionName,
                Title = content.Title,
                Content = content.Content,
                CreatedDate = content.CreatedDate,
                UpdatedDate = content.UpdatedDate,
                CreatedBy = content.CreatedBy,
                UpdatedBy = content.UpdatedBy
            };
        }

        public async Task<MainPageContentDto?> GetMainPageContentBySectionAsync(string sectionName)
        {
            var content = await _context.MainPageContents
                .Where(m => m.SectionName == sectionName && !m.IsDeleted)
                .FirstOrDefaultAsync();

            if (content == null)
                return null;

            return new MainPageContentDto
            {
                ContentId = content.ContentId,
                SectionName = content.SectionName,
                Title = content.Title,
                Content = content.Content,
                CreatedDate = content.CreatedDate,
                UpdatedDate = content.UpdatedDate,
                CreatedBy = content.CreatedBy,
                UpdatedBy = content.UpdatedBy
            };
        }

        public async Task<MainPageContentDto> CreateMainPageContentAsync(CreateMainPageContentDto createDto)
        {
            var content = new MainPageContent
            {
                SectionName = createDto.SectionName,
                Title = createDto.Title,
                Content = createDto.Content,
                CreatedBy = "Admin", // TODO: Get from current user context
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.MainPageContents.Add(content);
            await _context.SaveChangesAsync();

            return new MainPageContentDto
            {
                ContentId = content.ContentId,
                SectionName = content.SectionName,
                Title = content.Title,
                Content = content.Content,
                CreatedDate = content.CreatedDate,
                UpdatedDate = content.UpdatedDate,
                CreatedBy = content.CreatedBy,
                UpdatedBy = content.UpdatedBy
            };
        }

        public async Task<MainPageContentDto?> UpdateMainPageContentAsync(int id, UpdateMainPageContentDto updateDto)
        {
            var content = await _context.MainPageContents
                .Where(m => m.ContentId == id && !m.IsDeleted)
                .FirstOrDefaultAsync();

            if (content == null)
                return null;

            content.SectionName = updateDto.SectionName;
            content.Title = updateDto.Title;
            content.Content = updateDto.Content;
            content.UpdatedBy = "Admin"; // TODO: Get from current user context
            content.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new MainPageContentDto
            {
                ContentId = content.ContentId,
                SectionName = content.SectionName,
                Title = content.Title,
                Content = content.Content,
                CreatedDate = content.CreatedDate,
                UpdatedDate = content.UpdatedDate,
                CreatedBy = content.CreatedBy,
                UpdatedBy = content.UpdatedBy
            };
        }

        public async Task<bool> DeleteMainPageContentAsync(int id)
        {
            var content = await _context.MainPageContents
                .Where(m => m.ContentId == id && !m.IsDeleted)
                .FirstOrDefaultAsync();

            if (content == null)
                return false;

            content.IsDeleted = true;
            content.UpdatedBy = "Admin"; // TODO: Get from current user context
            content.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PublicMainPageDto>> GetPublicMainPageContentsAsync()
        {
            return await _context.MainPageContents
                .Where(m => !m.IsDeleted)
                .OrderBy(m => m.SectionName)
                .Select(m => new PublicMainPageDto
                {
                    SectionName = m.SectionName,
                    Title = m.Title,
                    Content = m.Content
                })
                .ToListAsync();
        }

        public async Task<bool> MainPageContentExistsAsync(int id)
        {
            return await _context.MainPageContents
                .AnyAsync(m => m.ContentId == id && !m.IsDeleted);
        }

        public async Task<bool> SectionExistsAsync(string sectionName, int? excludeId = null)
        {
            var query = _context.MainPageContents
                .Where(m => m.SectionName == sectionName && !m.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(m => m.ContentId != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
