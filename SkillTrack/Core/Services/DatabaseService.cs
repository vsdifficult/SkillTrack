using SkillTrack.Core.Models;
using Supabase; 

namespace SkillTrack.Core.Services 
{ 
    public class DatabaseService: IDatabaseService
    { 
        public async Task<Supabase.Client> SupabaseClient()
        {
            try 
            { 
                var url = ""; 
                var key = ""; 
                var options = new SupabaseOptions
                { 
                    AutoConnectRealtime = true
                }; 
                var client = new Supabase.Client(url, key, options); 
                return client; 
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error: {ex.Message}"); 
                throw; 
            }
        } 
        public async Task<string> CreateUser(UserDTO _user)
        { 
            try 
            { 
                var client = await SupabaseClient(); 
                var user = new User
                {
                    Username = _user.Username,
                    Password = _user.Password,
                    Tasks = new List<int>()
                }; 
                try 
                { 
                    await client.From<User>().Insert(user); 
                    return "Ok"; 
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine($"Error: {ex.Message}"); 
                    throw; 
                }
            } 
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error: {ex.Message}"); 
                throw; 
            }
        } 
        public async Task<string> GetUser(int user_id)
        { 
            try 
            { 
                var client = await SupabaseClient(); 
                var query = client.From<User>().Filter("Id", Supabase.Postgrest.Constants.Operator.Equals, user_id); 
                return "Ok"; 
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error: {ex.Message}"); 
                throw; 
            }
        } 
        public async Task<string> AddTask(UserTaskDTO userTaskDTO)
        { 
            try 
            { 
                var client = await SupabaseClient(); 
                var task = new UserTask
                {
                    User_Id = userTaskDTO.User_Id,
                    Desc = userTaskDTO.Desc,
                    Task_Name = userTaskDTO.Task_Name
                };  
                await client.From<UserTask>().Insert(task); 
                return "Ok"; 
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error: {ex.Message}"); 
                throw; 
            }
        } 
        public async Task<string> DeleteTask(int user_id)
        { 
            try 
            {   
                var client = await SupabaseClient();  
                try 
                { 
                    await client.From<UserTask>().Where(x => x.User_Id == user_id).Delete();
                    return "Ok"; 
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine($"Error: {ex.Message}"); 
                    throw; 
                }
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Error: {ex.Message}"); 
                throw; 
            }
        }
    }
}