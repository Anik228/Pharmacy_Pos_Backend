using AutoMapper;
using pharmacy_pos_system.module.medicine.model;
using pharmacy_pos_system.module.role.model;
using pharmacy_pos_system.module.user.model;

namespace pharmacy_pos_system.common
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {

            CreateMap<AddMedicineDto, Medicine>().ReverseMap();
            CreateMap<RoleDto, Role>().ReverseMap();
            CreateMap<CreateRoleDto, Role>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
          

        }
    }
}
