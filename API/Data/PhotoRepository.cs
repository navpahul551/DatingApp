using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly DataContext _context;

        public PhotoRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<List<PhotoDto>> GetUnApprovedPhotosAsync()
        {
            return await _context.Photos
                .IgnoreQueryFilters()
                .Where(p => !p.IsApproved)
                .Select(p => new PhotoDto
                {
                    Id = p.Id,
                    IsApproved = p.IsApproved,
                    IsMain = p.IsMain,
                    Url = p.Url
                }).ToListAsync();
        }

        public async Task<Photo> GetPhotoByIdAsync(int photoId)
        {
            return await _context.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == photoId);
        }

        public void DeletePhoto(Photo photo)
        {
            _context.Photos.Remove(photo);
        }
    }
}