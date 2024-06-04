using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    public class CourseService:ICourseService
    {
        private readonly IMongoCollection<Course> _course;
        private readonly IMongoCollection<Category> _category;
        private readonly IMapper _mapper;

        public CourseService(IMapper mapper,IDatabaseSettings databaseSettings)
        {
            _mapper = mapper;

            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _course = database.GetCollection<Course>(databaseSettings.CourseCollectionName);
            _category = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
        }

        public async Task<Response<List<CourseDto>>> GetAllAsync()
        {
            var courses=await _course.Find(course=>true).ToListAsync();
   
            if (courses.Any())
            {
                foreach (var course in courses) { course.Category = await _category.Find<Category>(x => x.Id == course.CategoryId).FirstAsync(); }

            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses),200);
        }


        public async Task<Response<CourseDto>> GetByIdAsync(string id)
        {
            var course=await _course.Find<Course>(x => x.Id == id).FirstOrDefaultAsync();
            if(course == null)
            {
                return Response<CourseDto>.Fail("Course not found",400);
            }

            course.Category = await _category.Find<Category>(x => x.Id == course.CategoryId).FirstAsync();

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(course),200);
        }

        public async Task<Response<List<CourseDto>>> GetAllByUserId(string userId)
        {

            var courses = await _course.Find<Course>(x => x.UserId == userId).ToListAsync();
            if (courses.Any())
            {
                foreach (var course in courses) { course.Category = await _category.Find<Category>(x => x.Id == course.CategoryId).FirstAsync(); }

            }
            else
            {
                courses = new List<Course>();
            }

            return Response<List<CourseDto>>.Success(_mapper.Map<List<CourseDto>>(courses), 200);
        }


        public async Task<Response<CourseDto>> CreateAsync(CreateCourseDto dto)
        {
            var newCourse=_mapper.Map<Course>(dto);
            newCourse.CreatedTime=DateTime.Now;
            await _course.InsertOneAsync(newCourse);

            return Response<CourseDto>.Success(_mapper.Map<CourseDto>(newCourse), 200);
        }


        public async Task<Response<NoContent>> UpdateAsync(UpdateCourseDto dto)
        {   //önemli
            var updateCourse = _mapper.Map<Course>(dto);

            var result = await _course.FindOneAndReplaceAsync(x => x.Id == dto.Id, updateCourse);
            if (result == null)
            {
                return Response<NoContent>.Fail("Course not found",404);
            }

            return Response<NoContent>.Success(204);
          
        }

        public async Task<Response<NoContent>> DeleteAsync(string id)
        {
            var result=await _course.DeleteOneAsync(x => x.Id == id);
            if (result.DeletedCount>0)
            {
                return Response<NoContent>.Success(204);
            }
            else
            {
                return Response<NoContent>.Fail("Course not found", 404);
            }
        }
    }
}
