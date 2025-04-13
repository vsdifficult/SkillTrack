using Supabase; 
using SkillTrack.Core.Models; 

namespace SkillTrack.Core.Services 
{ 
    public interface IDatabaseService 
    { 
        Task<Supabase.Client> SupabaseClient();  
        Task<string> CreateUser(UserDTO _user); 
        Task<string> GetUser(int user_id); 
        Task<string> AddTask(UserTaskDTO userTaskDTO); 
        Task<string> DeleteTask(int user_id); 
    }
}