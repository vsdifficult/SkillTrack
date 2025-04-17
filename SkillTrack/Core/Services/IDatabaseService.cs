using Supabase; 
using SkillTrack.Core.Models; 

namespace SkillTrack.Core.Services 
{ 
    public interface IDatabaseService 
    { 
        Task<Supabase.Client> SupabaseClient();  
        Task<bool> CreateUser(UserDTO _user); 
        Task<User?> GetUser(int user_id);
        Task<bool> AddTask(UserTaskDTO userTaskDTO); 
        Task<string> DeleteTask(int user_id); 
    }
}