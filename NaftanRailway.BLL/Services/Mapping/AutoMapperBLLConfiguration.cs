using AutoMapper;
using NaftanRailway.BLL.DTO.Nomenclature;
using NaftanRailway.Domain.Concrete.DbContexts.ORC;

namespace NaftanRailway.BLL.Services.Mapping {
    public class AutoMapperBLLConfiguration {
        public static void Configure() {
            Mapper.Initialize(cfg => {
                //x.CreateMap<krt_Naftan, ScrollLineDTO>().ReverseMap());
                //x.AddProfile<EntityToDTOMappingProfile>();
                cfg.CreateMap<krt_Naftan, ScrollLineDTO>();
                cfg.CreateMap<krt_Naftan_orc_sapod, ScrollDetailDTO>();
            });
        }
    }

    /// <summary>
    /// Mapping from EF6 entity to dto object (data transfer)
    /// </summary>
    //public class EntityToDTOMappingProfile : Profile {
    //    public EntityToDTOMappingProfile() : base("EntityToDTOMappings") { }

    //    protected override void Configure() {
    //        Mapper.CreateMap<krt_Naftan, ScrollLineDTO>();
    //        Mapper.CreateMap<krt_Naftan_orc_sapod, ScrollDetailDTO>();
    //    }
    //}
}