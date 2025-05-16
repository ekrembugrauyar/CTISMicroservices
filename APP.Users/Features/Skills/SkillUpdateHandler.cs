using System.ComponentModel.DataAnnotations;
using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Skills
{
    public class SkillUpdateRequest : Request, IRequest<CommandResponse>
    {
        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string Name { get; set; }
    }

    public class SkillUpdateHandler : UsersDbHandler, IRequestHandler<SkillUpdateRequest, CommandResponse>
    {
        public SkillUpdateHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public async Task<CommandResponse> Handle(SkillUpdateRequest request, CancellationToken cancellationToken)
        {
            if (await _usersDb.Skills.AnyAsync(s => s.Id != request.Id && s.Name.ToLower() == request.Name.ToLower().Trim(), cancellationToken: cancellationToken))
            {
                return Error($"Skill {request.Name} already exists");
            }

            var skill = await _usersDb.Skills.Include(s => s.UserSkills).SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (skill == null)
            {
                return Error($"Skill with id {request.Id} does not exist");
            }

            skill.Name = request.Name.Trim();
            _usersDb.Skills.Update(skill);
            await _usersDb.SaveChangesAsync(cancellationToken);

            return Success($"Skill {request.Name} updated successfully", skill.Id);
        }
    }
} 