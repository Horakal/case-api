using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestApiCase.Api.Controllers;  // Ajuste o namespace
using RestApiCase.Application.Tasks.DTOS;  // Para CreateTask
using RestApiCase.Application.Tasks.DTOS.Requests;
using RestApiCase.Application.Tasks.DTOS.ResponseDTO;
using RestApiCase.Application.Tasks.Services;  // Para ITaskService
using RestApiCase.Domain.Tasks.Entities;  // Para TaskItem
using RestApiCase.Domain.Tasks.Interfaces;
using RestApiCase.Domain.User.Interface;
using System.Security.Claims;
using System.Security.Principal;  // Para ClaimsIdentity
using Xunit;

namespace RestApiCase.Tests.Api.Controllers
{
    public class TaskControllerTests
    {
        private readonly Mock<ITaskService<TaskResponse>> _mockTaskService;
        private readonly Mock<IUserService> _mockUserService;

        public TaskControllerTests()
        {
            _mockTaskService = new Mock<ITaskService<TaskResponse>>();
            _mockUserService = new Mock<IUserService>();
        }

        [Fact]
        public async Task PostTasks_ValidUserIdFromClaim_Should_CreateTaskAndReturnCreated()
        {
            // Arrange
            var userGuid = Guid.NewGuid();
            var userIdClaimValue = userGuid.ToString();  // Simula o valor do claim "jti"

            // Cria um ClaimsPrincipal fake com o claim "jti"
            var claims = new List<Claim>
            {
                new Claim("jti", userIdClaimValue)  // Claim "jti" com o GUID como string
            };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var request = new CreateTask
            {
                Title = "Tarefa de Teste",
                Description = "Descrição para tarefa teste",
                DueDate = DateTime.Now.AddDays(1)
                // UserId será setado no método
            };

            var taskItem = new TaskItem(userGuid, request.Title, request.Description, request.Summary, request.DueDate);
            // Create a TaskResponse instance to return from the mock
            var expectedTaskResponse = new TaskResponse(taskItem);

            _mockTaskService.Setup(s => s.ExecuteAsync(It.IsAny<CreateTask>()))
                            .ReturnsAsync(expectedTaskResponse);

            // Cria o controlador e seta o User manualmente
            var controller = new TasksItemsController(_mockTaskService.Object, _mockUserService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal  // Atribui o ClaimsPrincipal fake
                }
            };

            // Act
            var actionResult = await controller.PostTasks(request);
            var result = actionResult.Result as CreatedAtActionResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(201);  // Created
            result.Value.Should().Be(expectedTaskResponse);  // Retorna o task criado

            // Verifica se o serviço foi chamado com UserId correto
            _mockTaskService.Verify(s => s.ExecuteAsync(It.Is<CreateTask>(r =>
                r.UserId == userGuid && r.Title == "Tarefa de Teste")), Times.Once);
        }

        [Fact]
        public async Task PostTasks_InvalidUserIdFromClaim_Should_ReturnUnauthorized()
        {
            // Arrange
            // ClaimsPrincipal sem claim "jti" (ou vazio)
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());  // User vazio

            var request = new CreateTask { Title = "Tarefa" };

            var controller = new TasksItemsController(_mockTaskService.Object, _mockUserService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var actionResult = await controller.PostTasks(request);
            var result = actionResult.Result as UnauthorizedResult;
            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(401);  // Unauthorized

            // Serviço NÃO deve ser chamado
            _mockTaskService.Verify(s => s.ExecuteAsync(It.IsAny<CreateTask>()), Times.Never);
        }

        [Fact]
        public async Task PostTasks_MalformedUserId_Should_ReturnUnauthorized()
        {
            // Arrange
            // Claim "jti" com valor inválido (não GUID)
            var claims = new List<Claim> { new Claim("jti", "invalid-guid") };
            var claimsIdentity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var request = new CreateTask { Title = "Tarefa" };

            var controller = new TasksItemsController(_mockTaskService.Object, _mockUserService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var actionResult = await controller.PostTasks(request);
            var result = actionResult.Result as UnauthorizedResult;
            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(401);

            _mockTaskService.Verify(s => s.ExecuteAsync(It.IsAny<CreateTask>()), Times.Never);
        }
    }
}