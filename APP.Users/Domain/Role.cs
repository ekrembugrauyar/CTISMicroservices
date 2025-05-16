using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace APP.Users.Domain
{
    public class Role : Entity
    {
        [Required, StringLength(10)]
        public string Name { get; set; }

        public List<User> Users { get; set; } = new List<User>();
    }
}
