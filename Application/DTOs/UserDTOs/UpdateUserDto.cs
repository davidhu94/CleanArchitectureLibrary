namespace Application.DTOs.UserDTOs
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string PasswordHash { get; set; }
    }
}
