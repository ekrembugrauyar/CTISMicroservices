using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Skills
{
    public class SkillDeleteRequest : Request, IRequest<CommandResponse>
    {
    }

    public class SkillDeleteHandler : UsersDbHandler, IRequestHandler<SkillDeleteRequest, CommandResponse>
    {
        public SkillDeleteHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public async Task<CommandResponse> Handle(SkillDeleteRequest request, CancellationToken cancellationToken)
        {
            var skill = await _usersDb.Skills.Include(s => s.UserSkills).SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (skill == null)
            {
                return Error($"Skill with id {request.Id} does not exist");
            }

            if (skill.UserSkills.Any())
            {
                return Error($"Skill {skill.Name} cannot be deleted because it is used by users");
            }

            _usersDb.Skills.Remove(skill);
            await _usersDb.SaveChangesAsync(cancellationToken);

            return Success($"Skill {skill.Name} deleted successfully", skill.Id);
        }
    }
} 