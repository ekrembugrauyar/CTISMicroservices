using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Roles
{
    public class RoleDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class RoleDeleteHandler : UsersDbHandler, IRequestHandler<RoleDeleteRequest, CommandResponse>
    {
        public RoleDeleteHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public async Task<CommandResponse> Handle(RoleDeleteRequest request, CancellationToken cancellationToken)
        {
            var role = await _usersDb.Roles.Include(r => r.Users).SingleOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (role == null)
            {
                return Error($"Role with id {request.Id} does not exist");
            }

            if (role.Users.Any())
            {
                return Error($"Role {role.Name} cannot be deleted because it is used by users");
            }

            _usersDb.Roles.Remove(role);
            await _usersDb.SaveChangesAsync(cancellationToken);

            return Success($"Role {role.Name} deleted successfully", role.Id);
        }
    }
} 