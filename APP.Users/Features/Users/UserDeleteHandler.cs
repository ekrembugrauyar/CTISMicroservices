using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Users
{
    public class UserDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class UserDeleteHandler : UsersDbHandler, IRequestHandler<UserDeleteRequest, CommandResponse>
    {
        public UserDeleteHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public async Task<CommandResponse> Handle(UserDeleteRequest request, CancellationToken cancellationToken)
        {
            var user = await _usersDb.Users.Include(u => u.UserSkills).SingleOrDefaultAsync(u => u.Id == request.Id, cancellationToken);
            if (user == null)
            {
                return Error($"User with id {request.Id} does not exist");
            }

            _usersDb.UserSkills.RemoveRange(user.UserSkills);
            _usersDb.Users.Remove(user);
            await _usersDb.SaveChangesAsync(cancellationToken);

            return Success($"User {user.UserName} deleted successfully", user.Id);
        }
    }
} 