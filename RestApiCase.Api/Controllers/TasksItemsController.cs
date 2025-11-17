using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using RestApiCase.Application.Tasks.DTOS.Requests;
using RestApiCase.Application.Tasks.DTOS.ResponseDTO;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Interfaces;
using RestApiCase.Domain.User.Interface;
using RestApiCase.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestApiCase.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksItemsController(ITaskService<TaskResponse> taskService, IUserService userService) : ControllerBase
    {
        private readonly ITaskService<TaskResponse> _taskService = taskService;
        private readonly IUserService _userService = userService;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTask()
        {
            var userId = User?.FindFirst("jti")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            {
                return Unauthorized();
            }

            var tasks = await _taskService.GetAllTasksAsync(userId);

            if (tasks == null || !tasks.Any())
            {
                return NoContent();
            }
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TaskItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<TaskItem>> GetTask(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Invalid ID");
            }
            var tasks = await _taskService.GetTaskByIdAsync(id);
            if (tasks == null)
            {
                return NotFound("No task found");
            }
            return Ok(tasks);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TaskItem), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<TaskItem>> PostTasks([FromBody] CreateTask request)
        {
            var userId = User?.FindFirst("jti")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out Guid userGuid))
            {
                return Unauthorized();
            }
            request.UserId = userGuid;
            var task = await _taskService.ExecuteAsync(request);
            return CreatedAtAction(nameof(TaskItem), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> PutTasks(string id, [FromBody] UpdateTask request)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Id inválido");
            }
            request.Id = Guid.Parse(id);
            await _taskService.ExecuteAsync(request);
            return NoContent();
        }

        // DELETE: api/TaskItems/5
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> DeleteTasks(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Id inválido");
            }
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}