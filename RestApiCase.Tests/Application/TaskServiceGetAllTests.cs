using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RestApiCase.Application.Tasks.Services;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Enums;
using RestApiCase.Domain.Tasks.Interfaces;
using System.Security.Claims;
using Xunit;

namespace RestApiCase.Tests.Application
{
    public class TaskServiceGetAllTests
    {
        private readonly Mock<ITaskRepository> _mockRepository;
        private readonly Mock<IHttpContextAccessor> _mockAccessor;
        private readonly TaskService _service;
        private readonly Guid _userId;

        public TaskServiceGetAllTests()
        {
            _userId = Guid.NewGuid();
            _mockAccessor = new Mock<IHttpContextAccessor>();
            var mockContext = new Mock<HttpContext>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("jti", _userId.ToString())
            }, "TestAuth"));
            mockContext.Setup(c => c.User).Returns(user);
            _mockAccessor.Setup(a => a.HttpContext).Returns(mockContext.Object);
            _mockRepository = new Mock<ITaskRepository>();
            _service = new TaskService(_mockRepository.Object, _mockAccessor.Object);
        }

        [Fact]
        public async Task GetAllTasksAsync_NoFilter_Should_ReturnAllTasks()
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem(_userId, "Task 1", "Descrição teste para atividade 1", "Summary", DateTime.UtcNow.AddDays(1)),
                new TaskItem(_userId, "Task 2", "Descrição teste para atividade 2", "Summary", DateTime.UtcNow.AddDays(2))
            };
            _mockRepository.Setup(r => r.GetAllTasksAsync(_userId, false, null)).ReturnsAsync(tasks);

            var result = await _service.GetAllTasksAsync(_userId);

            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllTasksAsync_WithStatusFilter_Should_PassStatusToRepo()
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem(_userId, "Task 1", "Descrição teste para atividade 1", "Summary", DateTime.UtcNow.AddDays(1))
            };
            _mockRepository.Setup(r => r.GetAllTasksAsync(_userId, false, TaskItemStatus.Pending)).ReturnsAsync(tasks);

            var result = await _service.GetAllTasksAsync(_userId, TaskItemStatus.Pending);

            result.Should().HaveCount(1);
            _mockRepository.Verify(r => r.GetAllTasksAsync(_userId, false, TaskItemStatus.Pending), Times.Once);
        }

        [Fact]
        public async Task GetAllTasksAsync_EmptyResult_Should_ReturnEmpty()
        {
            _mockRepository.Setup(r => r.GetAllTasksAsync(_userId, false, null)).ReturnsAsync(new List<TaskItem>());

            var result = await _service.GetAllTasksAsync(_userId);

            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetTaskByIdAsync_ExistingTask_Should_ReturnResponse()
        {
            var taskId = Guid.NewGuid();
            var task = new TaskItem(_userId, "Task 1", "Descrição teste para atividade 1", "Summary", DateTime.UtcNow.AddDays(1));
            _mockRepository.Setup(r => r.GetTaskByIdAsync(taskId, false, _userId)).ReturnsAsync(task);

            var result = await _service.GetTaskByIdAsync(taskId, _userId);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Task 1");
        }

        [Fact]
        public async Task GetTaskByIdAsync_NonExisting_Should_ReturnNull()
        {
            _mockRepository.Setup(r => r.GetTaskByIdAsync(It.IsAny<Guid>(), false, _userId)).ReturnsAsync((TaskItem?)null);

            var result = await _service.GetTaskByIdAsync(Guid.NewGuid(), _userId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteTaskAsync_Should_CallRepository()
        {
            var taskId = Guid.NewGuid();

            await _service.DeleteTaskAsync(taskId);

            _mockRepository.Verify(r => r.DeleteTaskAsync(taskId), Times.Once);
        }
    }
}
