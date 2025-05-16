using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Skills
{
    public class SkillQueryRequest : Request, IRequest<IQueryable<SkillQueryResponse>> { }

    public class SkillQueryResponse : QueryResponse
    {
        public string Name { get; set; }
        public int UserCount { get; set; }
        public string UserNames { get; set; }
    }

    public class SkillQueryHandler : UsersDbHandler, IRequestHandler<SkillQueryRequest, IQueryable<SkillQueryResponse>>
    {
        public SkillQueryHandler(UsersDb usersDb) : base(usersDb) { }

        public Task<IQueryable<SkillQueryResponse>> Handle(SkillQueryRequest request, CancellationToken cancellationToken)
        {
            var query = _usersDb.Skills
                .Include(s => s.UserSkills).ThenInclude(us => us.User)
                .OrderBy(s => s.Name)
                .Select(s => new SkillQueryResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    UserCount = s.UserSkills.Count,
                    UserNames = string.Join(", ", s.UserSkills.Select(us => us.User.UserName))
                });

            return Task.FromResult(query);
        }
    }
}
