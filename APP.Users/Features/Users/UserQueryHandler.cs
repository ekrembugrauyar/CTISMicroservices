using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Users
{
    public class UserQueryRequest : Request, IRequest<IQueryable<UserQueryResponse>>
    {
    }

    public class UserQueryResponse : QueryResponse
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string Active { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<int> SkillIds { get; set; }
        public string SkillNames { get; set; }
    }

    public class UserQueryHandler : UsersDbHandler, IRequestHandler<UserQueryRequest, IQueryable<UserQueryResponse>>
    {
        public UserQueryHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public Task<IQueryable<UserQueryResponse>> Handle(UserQueryRequest request, CancellationToken cancellationToken)
        {
            var entityQuery = _usersDb.Users
                .Include(u => u.Role)
                .Include(u => u.UserSkills)
                .ThenInclude(us => us.Skill)
                .OrderByDescending(u => u.IsActive)
                .ThenBy(u => u.UserName)
                .AsQueryable();

            var query = entityQuery.Select(u => new UserQueryResponse
            {
                Active = u.IsActive ? "Yes" : "No",
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = $"{u.FirstName} {u.LastName}",
                Id = u.Id,
                IsActive = u.IsActive,
                UserName = u.UserName,
                Password = u.Password,
                RoleId = u.RoleId,
                RoleName = u.Role.Name,
                SkillIds = u.SkillIds,
                SkillNames = string.Join(", ", u.UserSkills.Select(us => us.Skill.Name))
            });

            return Task.FromResult(query);
        }
    }
}
