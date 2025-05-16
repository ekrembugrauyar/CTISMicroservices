using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Users
{
    public class UserCreateRequest : Request, IRequest<CommandResponse>
    {
        [JsonIgnore]
        public override int Id { get => base.Id; set => base.Id = value; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
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

    public class UserCreateHandler : UsersDbHandler, IRequestHandler<UserCreateRequest, CommandResponse>
    {
        public UserCreateHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public async Task<CommandResponse> Handle(UserCreateRequest request, CancellationToken cancellationToken)
        {
            if (await _usersDb.Users.AnyAsync(u => u.UserName.ToLower() == request.UserName.ToLower().Trim(), cancellationToken: cancellationToken))
            {
                return Error($"User with username {request.UserName} already exists");
            }

            var role = await _usersDb.Roles.FindAsync(new object[] { request.RoleId }, cancellationToken);
            if (role == null)
            {
                return Error($"Role with id {request.RoleId} does not exist");
            }

            if (request.SkillIds != null && request.SkillIds.Any())
            {
                var existingSkillIds = await _usersDb.Skills
                    .Where(s => request.SkillIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                var invalidSkillIds = request.SkillIds.Except(existingSkillIds).ToList();
                if (invalidSkillIds.Any())
                {
                    return Error($"Skills with ids {string.Join(", ", invalidSkillIds)} do not exist");
                }
            }

            var user = new User
            {
                UserName = request.UserName.Trim(),
                Password = request.Password.Trim(),
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                IsActive = request.IsActive,
                RoleId = request.RoleId,
                SkillIds = request.SkillIds ?? new List<int>()
            };

            _usersDb.Users.Add(user);
            await _usersDb.SaveChangesAsync(cancellationToken);

            return Success($"User {request.UserName} created successfully", user.Id);
        }
    }
} 