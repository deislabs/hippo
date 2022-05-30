using AutoMapper;
using Hippo.Application.Common.Extensions;

namespace Hippo.Application.Common.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType()).IgnoreNoMap(GetType());
}
