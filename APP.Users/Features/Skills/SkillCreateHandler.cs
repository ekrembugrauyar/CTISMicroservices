using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using APP.Users.Domain;
using CORE.APP.Features;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace APP.Users.Features.Skills
{
    public class SkillCreateRequest : Request, IRequest<CommandResponse>
    {
        [JsonIgnore]
        public override int Id { get => base.Id; set => base.Id = value; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string Name { get; set; }
    }

    public class SkillCreateHandler : UsersDbHandler, IRequestHandler<SkillCreateRequest, CommandResponse>
    {
        public SkillCreateHandler(UsersDb usersDb) : base(usersDb)
        {
        }

        public async Task<CommandResponse> Handle(SkillCreateRequest request, CancellationToken cancellationToken)
        {
            if (await _usersDb.Skills.AnyAsync(s => s.Name.ToLower() == request.Name.ToLower().Trim(), cancellationToken: cancellationToken))
            {
                return Error($"Skill {request.Name} already exists");
            }

            var skill = new Skill
            {
                Name = request.Name.Trim()
            };

            _usersDb.Skills.Add(skill);
            await _usersDb.SaveChangesAsync(cancellationToken);

            return Success($"Skill {request.Name} created successfully", skill.Id);
        }
    }
} 