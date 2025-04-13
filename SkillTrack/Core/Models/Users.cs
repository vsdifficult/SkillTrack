using Supabase;
using Supabase.Postgrest.Attributes;

namespace SkillTrack.Core.Models
{
    public class User: Supabase.Postgrest.Models.BaseModel
    { 
        [PrimaryKey("Id")] 
        public int Id {get; set; } 
        [Column("Username")] 
        public string Username {get; set; } 
        [Column("Password")] 
        public string Password {get; set; } 
        [Column("Tasks")] 
        public List<int> Tasks {get; set; } = new List<int>(); 
    } 
    public class UserDTO
    { 
        public string Username {get; set; } 
        public string Password {get; set; } 
    }
}