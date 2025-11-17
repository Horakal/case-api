using FluentAssertions;
using Microsoft.VisualBasic;
using RestApiCase.Domain.Tasks.Entities;
using Xunit;

public class TaskItemTests
{
    [Fact]
    public void Create_ValidInput_Should_SetPropertiesCorrectly()
    {
        // Arrange
        var title = "Nova Tarefa";
        var description = "Descrição teste para atividade";
        var dueDate = DateTime.Now.AddDays(1);
        var summary = "Resumo da tarefa";

        // Act
        var task = new TaskItem(new Guid(), title, description, summary, dueDate: dueDate);

        // Assert
        task.Title.Should().Be(title);
        task.Description.Should().Be(description);
        task.DueDate.Should().Be(dueDate.ToUniversalTime());
    }

    [Fact]
    public void UpdatePartial_NullTitle_Should_KeepOriginalTitle()
    {
        // Arrange
        var task = new TaskItem(new Guid(), "Original", "Descrição teste para atividade", null, null);
        var updateTitle = "";  // Simula update parcial

        // Act
        task.UpdateTitle(updateTitle);

        // Assert
        task.Title.Should().Be("Original");  // Mantém original
    }

    [Theory]
    [InlineData("00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    public void Create_InvalidTitle_Should_ThrowException(string invalidTitle)
    {
        // Act
        Action act = () => new TaskItem(new Guid(), invalidTitle, "Descrição teste para atividade", "Summ", dueDate: DateTime.Now.AddDays(1));
        // Assert
        act.Should().Throw<DomainValidationException>().WithMessage("Falhas de validação de domínio detectadas.");
    }

    [Theory]
    [InlineData(null)]  // Teste de invariante
    [InlineData("")]
    [InlineData(@"00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
                00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
                00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
                00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
                00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000
                00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    public void Create_InvalidDesc_Should_ThrowException(string invalidDesc)
    {
        // Act
        Action act = () => new TaskItem(new Guid(), "Tittle", invalidDesc, "Summ", dueDate: DateTime.Now.AddDays(1));
        // Assert
        act.Should().Throw<DomainValidationException>().WithMessage("Falhas de validação de domínio detectadas.");
    }

    [Theory]
    [InlineData("00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000" +
                "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    public void Create_InvalidSummary_Should_ThrowException(string invalidSummary)
    {
        // Act
        Action act = () => new TaskItem(new Guid(), "AAAA", "Descrição teste para atividade", invalidSummary, dueDate: DateTime.Now.AddDays(1));
        // Assert
        act.Should().Throw<DomainValidationException>().WithMessage("Falhas de validação de domínio detectadas.");
    }

    [Fact]
    public void Create_InvalidDate_Should_ThrowException()
    {
        // Try to parse the date string, if possible, otherwise use a default invalid date
        DateTime invalidDate = DateTime.Now.AddDays(-1);

        // Act
        Action act = () => new TaskItem(new Guid(), "AAA", "Descrição teste para atividade", "Summ", dueDate: invalidDate);
        // Assert
        act.Should().Throw<DomainValidationException>().WithMessage("Falhas de validação de domínio detectadas.");
    }
}