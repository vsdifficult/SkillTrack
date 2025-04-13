
using Supabase.Postgrest.Attributes;

namespace SkillTrack.Core.Models 
{ 
    public class UserTask: Supabase.Postgrest.Models.BaseModel
    { 
        [PrimaryKey("Id")]
        public int Id {get; set; } 
        [Column("Task_Name")] 
        public string Task_Name {get; set; }
        [Column("Desc")] 
        public string Desc {get; set; } 
        [Column("User_Id")] 
        public int User_Id {get; set; }
    } 
    public class UserTaskDTO
    { 
        public string Task_Name {get; set; }
        public string Desc {get; set; } 
        public int User_Id {get; set; }
    }
}