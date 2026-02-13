using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestApiCase.Api.Controllers;
using RestApiCase.Application.Tasks.DTOS.ResponseDTO;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Enums;
using RestApiCase.Domain.Tasks.Interfaces;
using RestApiCase.Domain.User.Interface;
using System.Security.Claims;
using Xunit;

namespace RestApiCase.Tests.Api
{
    public class TasksItemsControllerGetTests
    {
        private readonly Mock<ITaskService<TaskResponse>> _mockTaskService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly TasksItemsController _controller;
        private readonly Guid _userGuid;

        public TasksItemsControllerGetTests()
        {
            _userGuid = Guid.NewGuid();
            _mockTaskService = new Mock<ITaskService<TaskResponse>>();
            _mockUserService = new Mock<IUserService>();
            _controller = new TasksItemsController(_mockTaskService.Object, _mockUserService.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim("jti", _userGuid.ToString())
                    }, "TestAuth"))
                }
            };
        }

        [Fact]
        public async Task GetTasks_NoFilter_Should_ReturnOk()
        {
            var task = new TaskItem(_userGuid, "Task 1", "Descrição teste para atividade", "Summary", DateTime.UtcNow.AddDays(1));
            var responses = new List<TaskResponse> { new TaskResponse(task) };
            _mockTaskService.Setup(s => s.GetAllTasksAsync(_userGuid, null)).ReturnsAsync(responses);

            var result = await _controller.GetTasks(null);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetTasks_WithStatusFilter_Should_ReturnOk()
        {
            var task = new TaskItem(_userGuid, "Task 1", "Descrição teste para atividade", "Summary", DateTime.UtcNow.AddDays(1));
            var responses = new List<TaskResponse> { new TaskResponse(task) };
            _mockTaskService.Setup(s => s.GetAllTasksAsync(_userGuid, TaskItemStatus.Pending)).ReturnsAsync(responses);

            var result = await _controller.GetTasks(TaskItemStatus.Pending);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTasks_EmptyResult_Should_ReturnNoContent()
        {
            _mockTaskService.Setup(s => s.GetAllTasksAsync(_userGuid, null)).ReturnsAsync(new List<TaskResponse>());

            var result = await _controller.GetTasks(null);

            result.Result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetTasks_NoUser_Should_ReturnUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await _controller.GetTasks(null);

            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task GetTask_ById_ExistingTask_Should_ReturnOk()
        {
            var taskId = Guid.NewGuid();
            var task = new TaskItem(_userGuid, "Task 1", "Descrição teste para atividade", "Summary", DateTime.UtcNow.AddDays(1));
            var response = new TaskResponse(task);
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(taskId, _userGuid)).ReturnsAsync(response);

            var result = await _controller.GetTask(taskId);

            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
        }

        [Fact]
        public async Task GetTask_ById_NotFound_Should_ReturnNotFound()
        {
            _mockTaskService.Setup(s => s.GetTaskByIdAsync(It.IsAny<Guid>(), _userGuid)).ReturnsAsync((TaskResponse?)null);

            var result = await _controller.GetTask(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetTask_ById_NoUser_Should_ReturnUnauthorized()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity()) }
            };

            var result = await _controller.GetTask(Guid.NewGuid());

            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task DeleteTasks_Should_ReturnNoContent()
        {
            var taskId = Guid.NewGuid();

            var result = await _controller.DeleteTasks(taskId);

            result.Should().BeOfType<NoContentResult>();
            _mockTaskService.Verify(s => s.DeleteTaskAsync(taskId), Times.Once);
        }
    }
}
