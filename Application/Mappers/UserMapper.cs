using Application.Commands.UserCommands.UpdateUser;
using Application.DTOs.UserDTOs;
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
                UserName = user.UserName,
                Password = user.PasswordHash
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
                PasswordHash = updateUserDto.Password
            };
        }

        public static UpdateUserCommand ToUpdateCommand(UpdateUserDto updateUserDto)
        {
            return new UpdateUserCommand(
                updateUserDto.Id,
                updateUserDto.UserName,
                updateUserDto.Password);
        }
    }
}
