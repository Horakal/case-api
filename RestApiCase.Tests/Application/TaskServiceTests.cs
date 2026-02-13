using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using RestApiCase.Application.Tasks.DTOS.Requests;
using RestApiCase.Application.Tasks.Services;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Interfaces;
using System.Security.Claims;
using Xunit;

namespace RestApiCase.Tests.Application
{
    public class TaskServiceTests
    {
        private readonly Mock<ITaskRepository> _mockRepository;
        private readonly Mock<IHttpContextAccessor> _mockAccessor;
        private readonly Mock<HttpContext> _mockContext;
        private readonly Mock<HttpContext> _mockHttp;
        private readonly TaskService _service;
        private readonly ClaimsPrincipal _mockUser;

        public TaskServiceTests()
        {
            _mockAccessor = new Mock<IHttpContextAccessor>();
            _mockContext = new Mock<HttpContext>();
            var userId = Guid.NewGuid().ToString();  // GUID fixo para testes consistentes (ou randomize se preferir)
            _mockUser = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim("jti", userId)  // Claim do JWT
        }, "TestAuth"));
            _mockContext.Setup(c => c.User).Returns(_mockUser);
            _mockAccessor.Setup(a => a.HttpContext).Returns(_mockContext.Object);
            _mockRepository = new Mock<ITaskRepository>();
            _mockHttp = new Mock<HttpContext>();
            _service = new TaskService(_mockRepository.Object, _mockAccessor.Object);
        }

        [Fact]
        public async Task ExecuteAsync_CreateRequest_Should_AddTask()
        {
            // Arrange
            var guid = _mockUser.FindFirst("jti")!.Value;
            _ = Guid.TryParse(guid, out Guid userGuid);
            var request = new CreateTask(userGuid, "Título", "Descrição teste para atividade", "", dueDate: DateTime.Now.AddDays(1));
            var expectedTask = new TaskItem(userGuid, "Título", "Descrição teste para atividade", "", dueDate: DateTime.Now.AddDays(1));
            _mockRepository.Setup(r => r.CreateTaskAsync(It.IsAny<TaskItem>())).ReturnsAsync(expectedTask);

            // Act
            var result = await _service.ExecuteAsync(request);

            // Assert
            _mockRepository.Verify(r => r.CreateTaskAsync(It.Is<TaskItem>(t => t.Title == "Título")), Times.Once);
            result.Title.Should().Be("Título");
        }

        [Fact]
        public async Task ExecuteAsync_UpdateRequest_WithNullFields_Should_NotChangeThem()
        {
            // Arrange
            var guid = _mockUser.FindFirst("jti")!.Value;
            _ = Guid.TryParse(guid, out Guid userGuid);
            var request = new UpdateTask(Guid.NewGuid(), null, "Descrição teste para atividade nova", null, null, RestApiCase.Domain.Tasks.Enums.TaskItemStatus.Completed, userGuid);
            var existingTask = new TaskItem(new Guid(), "Original", "Descrição teste para atividade Original", "", DateTime.Now);
            _mockRepository.Setup(r => r.GetTaskByIdAsync(request.Id, false, userGuid)).ReturnsAsync(existingTask);
            _mockRepository.Setup(r => r.UpdateTaskAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.ExecuteAsync(request);

            // Assert
            existingTask.Description.Should().Be("Descrição teste para atividade nova");  // Mudou
            existingTask.Title.Should().Be("Original");  // Não mudou (null)
        }
    }
}