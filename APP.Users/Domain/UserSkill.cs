using CORE.APP.Domain;

namespace APP.Users.Domain
{
    public class UserSkill : Entity
    {
        public int UserId { get; set; }

        public User User { get; set; }

        public int SkillId { get; set; }

        public Skill Skill { get; set; }
    }
}
