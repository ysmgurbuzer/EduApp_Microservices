using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;

namespace FreeCourse.Services.Catalog.Services
{
    public class CategoryService:ICategoryService
    {
        private readonly IMongoCollection<Category> _category;
        private readonly IMapper _mapper;

        public CategoryService( IMapper mapper,IDatabaseSettings databaseSettings)
        {
            
            _mapper = mapper;

            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _category = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);

        }

        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            var categories = await _category.Find(category=>true).ToListAsync();
            var mapped = _mapper.Map<List<CategoryDto>>(categories);
            return  Response<List<CategoryDto>>.Success(mapped,200);
             
        }


        public async Task<Response<CategoryDto>> CreateAsync(CategoryDto dto)
        {
            var category = _mapper.Map<Category>(dto);
            await _category.InsertOneAsync(category);
            
            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category),200);
        }

        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            var category = await _category.Find<Category>(x=>x.Id==id).FirstOrDefaultAsync();
            if (category == null) { return Response<CategoryDto>.Fail("Category Not Found", 404); }
            
            var mapped = _mapper.Map<CategoryDto>(category);
            return Response<CategoryDto>.Success(mapped, 200);

        }
    }
}
