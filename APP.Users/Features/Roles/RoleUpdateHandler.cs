using System.ComponentModel.DataAnnotations;
using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Roles
{
    public class RoleUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string Name { get; set; }
    }

    public class RoleUpdateHandler : UsersDbHandler, IRequestHandler<RoleUpdateRequest, CommandResponse>
    {
        public RoleUpdateHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public async Task<CommandResponse> Handle(RoleUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await _usersDb.Roles.AnyAsync(r => r.Id != request.Id && r.Name.ToLower() == request.Name.ToLower().Trim(), cancellationToken: cancellationToken))
            {
                return Error($"Role {request.Name} already exists");
            }

            var role = await _usersDb.Roles.Include(r => r.Users).SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (role == null)
            {
                return Error($"Role with id {request.Id} does not exist");
            }

            role.Name = request.Name.Trim();
            _usersDb.Roles.Update(role);
            await _usersDb.SaveChangesAsync(cancellationToken);

            return Success($"Role {request.Name} updated successfully", role.Id);
        }
    }
} 