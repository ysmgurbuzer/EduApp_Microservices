using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Shared.Dtos;

namespace FreeCourse.Services.Catalog.Services
{
    public interface ICourseService
    {
        Task<Response<List<CourseDto>>> GetAllAsync();
        Task<Response<CourseDto>> GetByIdAsync(string id);
        Task<Response<List<CourseDto>>> GetAllByUserId(string userId);
        Task<Response<CourseDto>> CreateAsync(CreateCourseDto dto);
        Task<Response<NoContent>> UpdateAsync(UpdateCourseDto dto);
        Task<Response<NoContent>> DeleteAsync(string id);
    }
}
