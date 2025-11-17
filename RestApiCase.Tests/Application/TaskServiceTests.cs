using FluentAssertions;
using Moq;
using RestApiCase.Application.Tasks.DTOS.Requests;
using RestApiCase.Application.Tasks.Services;
using RestApiCase.Domain.Tasks.Entities;
using RestApiCase.Domain.Tasks.Interfaces;
using Xunit;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _mockRepository;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _mockRepository = new Mock<ITaskRepository>();
        _service = new TaskService(_mockRepository.Object);
    }

    [Fact]
    public async Task ExecuteAsync_CreateRequest_Should_AddTask()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var request = new CreateTask(new Guid(), "Título", "Descrição teste para atividade", "", dueDate: DateTime.Now.AddDays(1));
        var expectedTask = new TaskItem(guid, "Título", "Descrição teste para atividade", "", dueDate: DateTime.Now.AddDays(1));
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
        var request = new UpdateTask(Guid.NewGuid(), null, "Descrição teste para atividade nova", null, null, RestApiCase.Domain.Tasks.Enums.TaskItemStatus.Completed);
        var existingTask = new TaskItem(new Guid(), "Original", "Descrição teste para atividade Original", "", DateTime.Now);
        _mockRepository.Setup(r => r.GetTaskByIdAsync(request.Id)).ReturnsAsync(existingTask);
        _mockRepository.Setup(r => r.UpdateTaskAsync(It.IsAny<TaskItem>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.ExecuteAsync(request);

        // Assert
        existingTask.Description.Should().Be("Descrição teste para atividade nova");  // Mudou
        existingTask.Title.Should().Be("Original");  // Não mudou (null)
    }
}