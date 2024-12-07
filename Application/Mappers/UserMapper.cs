using Application.DTOs.UserDTOs;
using Application.Users.UserCommands.UpdateUser;
using Domain.Models;

namespace Application.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName
            };
        }

        public static User ToModel(AddUserDto addUserDto)
        {
            return new User
            {
                UserName = addUserDto.UserName,
                PasswordHash = addUserDto.Password
            };
        }

        public static User ToModel(UpdateUserDto updateUserDto)
        {
            return new User
            {
                Id = updateUserDto.Id,
                UserName = updateUserDto.UserName,
                PasswordHash = updateUserDto.PasswordHash
            };
        }

        public static UpdateUserCommand ToUpdateCommand(UpdateUserDto updateUserDto)
        {
            return new UpdateUserCommand(
                updateUserDto.Id,
                updateUserDto.UserName,
                updateUserDto.PasswordHash);
        }

        public static User ToModel(LoginDto loginDto)
        {
            return new User
            {
                UserName = loginDto.UserName
                
            };
        }
    }
}
