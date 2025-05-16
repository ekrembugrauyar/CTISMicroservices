using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Roles
{
    public class RoleQueryRequest : Request, IRequest<IQueryable<RoleQueryResponse>>
    {
    }

    public class RoleQueryResponse : QueryResponse
    {
        public string Name { get; set; }
        public int UserCount { get; set; }
        public string UserNames { get; set; }
    }

    public class RoleQueryHandler : UsersDbHandler, IRequestHandler<RoleQueryRequest, IQueryable<RoleQueryResponse>>
    {
        public RoleQueryHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public Task<IQueryable<RoleQueryResponse>> Handle(RoleQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _usersDb.Roles
                .Include(r => r.Users)
                .OrderBy(r => r.Name)
                .Select(r => new RoleQueryResponse
                {
                    Id = r.Id,
                    Name = r.Name,
                    UserCount = r.Users.Count(),
                    UserNames = string.Join(", ", r.Users.Select(u => u.UserName))
                });

            return Task.FromResult(query);
        }
    }
}
