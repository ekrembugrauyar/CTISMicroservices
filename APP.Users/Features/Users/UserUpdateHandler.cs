using System.ComponentModel.DataAnnotations;
using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Users
{
    public class UserUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public int RoleId { get; set; }

        public List<int> SkillIds { get; set; }
    }

    public class UserUpdateHandler : UsersDbHandler, IRequestHandler<UserUpdateRequest, CommandResponse>
    {
        public UserUpdateHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public async Task<CommandResponse> Handle(UserUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await _usersDb.Users.AnyAsync(u => u.Id != request.Id && u.UserName.ToLower() == request.UserName.ToLower().Trim(), cancellationToken: cancellationToken))
            {
                return Error($"User with username {request.UserName} already exists");
            }

            var role = await _usersDb.Roles.FindAsync(new object[] { request.RoleId }, cancellationToken);
            if (role == null)
            {
                return Error($"Role with id {request.RoleId} does not exist");
            }

            var user = await _usersDb.Users.Include(u => u.UserSkills).SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user == null)
            {
                return Error($"User with id {request.Id} does not exist");
            }

            _usersDb.UserSkills.RemoveRange(user.UserSkills);

            user.UserName = request.UserName.Trim();
            user.Password = request.Password.Trim();
            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.IsActive = request.IsActive;
            user.RoleId = request.RoleId;
            user.SkillIds = request.SkillIds;

            _usersDb.Users.Update(user);
            await _usersDb.SaveChangesAsync(cancellationToken);

            return Success($"User {request.UserName} updated successfully", user.Id);
        }
    }
} 