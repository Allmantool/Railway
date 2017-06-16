using AutoMapper;

namespace NaftanRailway.WebUI.Infrastructure.Mapping {
    public class AutoMapperWebConfiguration {
        public static void Configure() {
            Mapper.Initialize(x => {
                x.AddProfile<DomainToViewModelMappingProfile>();
                x.AddProfile<ViewModelToDomainMappingProfile>();
            });
        }
    }

    public class ViewModelToDomainMappingProfile : Profile {
        public ViewModelToDomainMappingProfile() : base("ViewModelToDomainMappings") {

        }
        //protected override void Configure() {
        //    //Mapper.CreateMap<GadgetFormViewModel, Gadget>()
        //    //    .ForMember(g => g.Name, map => map.MapFrom(vm => vm.GadgetTitle))
        //    //    .ForMember(g => g.Description, map => map.MapFrom(vm => vm.GadgetDescription))
        //    //    .ForMember(g => g.Price, map => map.MapFrom(vm => vm.GadgetPrice))
        //    //    .ForMember(g => g.Image, map => map.MapFrom(vm => vm.File.FileName))
        //    //    .ForMember(g => g.CategoryID, map => map.MapFrom(vm => vm.GadgetCategory));
        //}
    }

    public class DomainToViewModelMappingProfile : Profile {
        public DomainToViewModelMappingProfile() : base("DomainToViewModelMappings") {

        }

        //protected override void Configure() {
        //    //Mapper.CreateMap<Category, CategoryViewModel>();
        //    //Mapper.CreateMap<Gadget, GadgetViewModel>();
        //}
    }
}