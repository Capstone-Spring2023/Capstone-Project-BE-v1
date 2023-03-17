using Business.Constants;
using Business.UserService.Interfaces;
using Business.UserService.Models;
using Data.Models;
using Data.Repositories.Interface;

namespace Business.UserService.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;


        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ResponseModel> GetUser(int id)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
            {
                return new()
                {
                    StatusCode = (int) StatusCode.NOTFOUND
                };
            }
            
            UserModel userModel = new UserModel(
                user.FullName, user.Phone.Trim(), user.Address, user.RoleId
            );
           
            return new()
            {
                StatusCode = (int) StatusCode.OK,
                Data = userModel
            };
        }

        public async Task<ResponseModel> UpdateUser(int id, UserModel userModel)
        {
            var user = await _userRepository.GetUserAsync(id);
            if (user == null)
            {
                return new()
                {
                    StatusCode = 400,
                };
            }
            user.FullName = userModel.fullName;
            user.Phone = userModel.phone.Trim();
            user.Address = userModel.address;
            user.RoleId = userModel.roldId;
            await _userRepository.UpdateUserAsync(id, user);
            return new()
            {
                StatusCode = (int) StatusCode.SUCCESS,
                Data = userModel
            };

        }

        public async Task<ResponseModel> GetAllLeaders()
        {
            var listUser = await _userRepository.GetAllAsync();
            var listLeader = new List<UserModel>();
            foreach (var user in listUser)
            {
                if(user.RoleId == ((int)Constants.Role.Leader))
                {
                    listLeader.Add(new UserModel(
                        user.FullName, user.Phone.Trim(), user.Address, user.RoleId
                    ));
                }
            }
            return new()
            {
                StatusCode= (int) StatusCode.OK,
                Data = listLeader
            };
        }

        
    }
}
