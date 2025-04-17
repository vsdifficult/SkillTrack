using Microsoft.AspNetCore.Mvc; 
using SkillTrack.Core.Models; 
using SkillTrack.Core.Services; 

namespace SkillTrack.API.User
{ 
    [ApiController()] 
    [Route("api/users")] 
    public class UserController: ControllerBase
    { 
        private readonly IDatabaseService _database; 

        public UserController(IDatabaseService database) 
        { 
            _database = database; 
        } 

        [HttpPost("register")] 
        public async Task<IActionResult> CreateUser([FromBody] UserDTO new_user) 
        { 
            try 
            { 
                var client = await _database.CreateUser(new_user); 
                return client?  Ok() : StatusCode(501, new {error = "Some problem in server"});  
            } 
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message}); 
            }
        } 
        [HttpGet("get/{id}")] 
        public async Task<IActionResult> GetUser(int user_id)
        { 
            try 
            { 
                var client = await _database.GetUser(user_id); 
                return Ok(new {user = client}); 
            } 
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message); 
            }
        } 
        
    }
}