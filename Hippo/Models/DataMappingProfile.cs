using AutoMapper;

namespace Hippo.Models
{
    public class DataMappingProfile: Profile
    {
        public DataMappingProfile()
        {
            CreateMap<Channel, Snapshot>();
        }
    }
}
