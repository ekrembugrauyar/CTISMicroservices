using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Roles
{
    public class RoleCreateRequest : Request, IRequest<CommandResponse>
    {
        [JsonIgnore]
        public override int Id { get => base.Id; set => base.Id = value; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string Name { get; set; }
    }

    public class RoleCreateHandler : UsersDbHandler, IRequestHandler<RoleCreateRequest, CommandResponse>
    {
        public RoleCreateHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public async Task<CommandResponse> Handle(RoleCreateRequest request, CancellationToken cancellationToken)
        {
            if (await _usersDb.Roles.AnyAsync(r => r.Name.ToLower() == request.Name.ToLower().Trim(), cancellationToken: cancellationToken))
            {
                return Error($"Role {request.Name} already exists");
            }

            var role = new Role
            {
                Name = request.Name.Trim()
            };

            _usersDb.Roles.Add(role);
            await _usersDb.SaveChangesAsync(cancellationToken);

            return Success($"Role {request.Name} created successfully", role.Id);
        }
    }
} 