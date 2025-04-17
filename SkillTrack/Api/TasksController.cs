using Microsoft.AspNetCore.Mvc; 
using SkillTrack.Core.Models; 
using SkillTrack.Core.Services; 

namespace SkillTrack.API
{ 
    [ApiController] 
    [Route("api/tasks")]
    public class TaskController: ControllerBase
    { 
        private readonly IDatabaseService _database; 
        public TaskController(IDatabaseService database)
        { 
            _database = database; 
        } 
        
        [HttpPost("add-task")]
        public async Task<IActionResult> AddTask([FromBody] UserTaskDTO userTask) 
        { 
            try 
            { 
                var client = await _database.AddTask(userTask); 
                return client? Ok() : StatusCode(501, "Some problem in server"); 
            } 
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message); 
            }
        } 
        [Http]
    }
}