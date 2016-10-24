using AutoMapper;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;

namespace NaftanRailway.BLL.Services.Mapping {
    public class AutoMapperConfiguration {
        public static void Configure() {
            Mapper.Initialize(x => {
                x.AddProfile<EntityToDTOMappingProfile>();
            });
        }
    }

    /// <summary>
    /// Mapping from EF6 entity to dto object (data transfer)
    /// </summary>
    public class EntityToDTOMappingProfile : Profile {
        public EntityToDTOMappingProfile() : base("EntityToDTOMappingProfile") {
        }
        protected override void Configure() {
            Mapper.CreateMap<ScrollLineDTO, krt_Naftan>();
            Mapper.CreateMap<ScrollDetailDTO, krt_Naftan_orc_sapod>();
        }
    }
}