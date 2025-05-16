using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Users.Domain
{
    public class Skill : Entity
    {
        [Required, StringLength(50)]
        public string Name { get; set; }

        public List<UserSkill> UserSkills { get; set; } = new List<UserSkill>();

        [NotMapped]
        public List<int> UserIds
        {
            get => UserSkills?.Select(us => us.UserId).ToList();
            set => UserSkills = value?.Select(userId => new UserSkill { UserId = userId }).ToList();
        }
    }
}
