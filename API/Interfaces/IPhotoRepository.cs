using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<List<PhotoDto>> GetUnApprovedPhotosAsync();
        Task<Photo> GetPhotoByIdAsync(int photoId);
        void DeletePhoto(Photo photo);
    }
}