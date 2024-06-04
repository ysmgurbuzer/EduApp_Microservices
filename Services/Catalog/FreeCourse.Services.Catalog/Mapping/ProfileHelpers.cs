using AutoMapper;

namespace FreeCourse.Services.Catalog.Mapping
{
    public class ProfileHelpers
    {

        public static List<Profile> GetProfiles()
        {

            return new List<Profile>
            {
                new GeneralMapping(),
             
            };




        }

    }
}
