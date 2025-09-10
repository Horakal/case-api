using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using RestApiCase.Api.Requests;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Interfaces;
using RestApiCase.Domain.User.Interface;
using RestApiCase.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestApiCase.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksItemsController(ITaskService taskService, IUserService userService) : ControllerBase
    {
        private readonly ITaskService _taskService = taskService;
        private readonly IUserService _userService = userService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetItems()
        {
            var userId = User?.FindFirst("jti")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            {
                return Unauthorized();
            }
            try
            {
                var tasks = await _taskService.GetAllTasksAsync(userId);
                if (tasks == null || !tasks.Any())
                {
                    return NoContent();
                }
                return Ok(tasks);
            }
            catch (Exception)
            {
                return StatusCode(500, "Erro interno");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(string id)
        {
            try
            {
                var tasks = await _taskService.GetTaskByIdAsync(id);
                if (tasks == null)
                {
                    return NotFound("Nenhuma tarefa encontrada");
                }
                return Ok(tasks);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // POST: api/TaskItem
        [HttpPost]
        public async Task<ActionResult<TaskItem>> PostTasks([FromBody] CreateTask request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User?.FindFirst("jti")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            {
                return Unauthorized();
            }
            try
            {
                var task = await _taskService.CreateTaskAsync(userGuid, request.Title, request.Description, request.Summary, request.DueDate);
                return CreatedAtAction(nameof(TaskItem), new { id = task.Id }, task);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // PUT: api/TaskItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTasks(string id, [FromBody] UpdateTask request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _taskService.UpdateTaskAsync(id, request.Title, request.Description, request.Status, request.DueDate);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // DELETE: api/TaskItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTasks(string id)
        {
            try
            {
                await _taskService.DeleteTaskAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}