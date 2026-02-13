using RestApiCase.Domain.Tasks.Entities;
using System;
using RestApiCase.Application.Tasks.DTOS.Requests;
using RestApiCase.Application.Tasks.DTOS.ResponseDTO;
using RestApiCase.Domain.Tasks.Enums;

namespace RestApiCase.Application.Tasks.Mappings
{
    public static class MappingExtensions
    {
        public static TaskItem ToTaskItem(this CreateTask request)
        {
            ArgumentNullException.ThrowIfNull(request);

            return new TaskItem(
                userId: request.UserId,
                title: request.Title,
                description: request.Description,
                summary: request.Summary,
                dueDate: request.DueDate
            );
        }

        public static TaskResponse ToTaskResponse(this TaskItem task)
        {
            ArgumentNullException.ThrowIfNull(task);
            return new TaskResponse(task);
        }

        public static void ApplyUpdate(this TaskItem task, UpdateTask request)
        {
            ArgumentNullException.ThrowIfNull(task);
            ArgumentNullException.ThrowIfNull(request);

            if (!string.IsNullOrEmpty(request.Title))
                task.UpdateTitle(request.Title);

            if (!string.IsNullOrEmpty(request.Description))
                task.UpdateDescription(request.Description);
            if (!string.IsNullOrEmpty(request.Summary))
                task.UpdateSummary(request.Summary);
            if (request.DueDate.HasValue)
                task.UpdateDueDate(request.DueDate.Value.ToUniversalTime());
            if (request.Status != task.Status && Enum.IsDefined(request.Status))
                task.UpdateStatus(request.Status);
        }
    }
}