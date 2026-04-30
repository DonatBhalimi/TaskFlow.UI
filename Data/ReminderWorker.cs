using TaskFlow.UI.Models;

namespace TaskFlow.UI.Data
{
    public class ReminderWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public ReminderWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<TaskRepository>();
                var reminderService = scope.ServiceProvider.GetRequiredService<TaskReminderService>();
                var notificationRepo = scope.ServiceProvider.GetRequiredService<NotificationRepository>();
                var userRepo = scope.ServiceProvider.GetRequiredService<UserRepository>();
                var tasks = await repo.GetAllAsync();
                var nowUtc = DateTime.UtcNow;

                var due = reminderService.GetTasksNeedingReminder(tasks, nowUtc);
                var approaching = tasks
                .Where(t =>
                {
                    var targetUserId = t.AssignedToUserId ?? t.CreatedByUserId;
                    var targetUser = userRepo.GetByIdAsync(targetUserId).Result;

                    if (targetUser == null)
                        return false;

                    return reminderService.ShouldTriggerDeadlineApproaching(
                        t,
                        nowUtc,
                        targetUser.DeadlineApproachingMinutesBefore);
                }).ToList();
                var missed = reminderService.GetTasksWithDeadlineMissed(tasks, nowUtc);
                var recurringCompleted = reminderService.GetCompletedRecurringTaskNeedingNotification(tasks);
                foreach (var task in due)
                {
                    Console.WriteLine($"Reminder : {task.Title}");

                    var targetUserId = task.AssignedToUserId ?? task.CreatedByUserId;
                    var targetUser = await userRepo.GetByIdAsync(targetUserId);

                    if (targetUser == null ||
                        !targetUser.InAppNotificationEnabled ||
                        !targetUser.TaskReminderNotificationEnabled)
                    {
                        task.LastReminderSentUtc = DateTime.UtcNow;
                        await repo.UpdateAsync(task);
                        continue;
                    }

                    var notification = new Notification
                    {
                        UserId = targetUserId,
                        TaskId = task.Id,
                        WorkspaceId = task.WorkspaceId,
                        Message = $"Reminder due: {task.Title}",
                        CreatedAtUtc = DateTime.UtcNow,
                        IsRead = false
                    };

                    await notificationRepo.CreateAsync(notification);

                    task.LastReminderSentUtc = DateTime.UtcNow;
                    await repo.UpdateAsync(task);
                }

                foreach (var task in approaching)
                {
                    Console.WriteLine($"Deadline approaching : {task.Title}");
                    var targetUserId = task.AssignedToUserId ?? task.CreatedByUserId;
                    var targetUser = await userRepo.GetByIdAsync(targetUserId);
                    if (targetUser == null ||
                        !targetUser.InAppNotificationEnabled ||
                        !targetUser.DeadlineApproachingNotificationsEnabled)
                    {
                        task.LastDeadlineAproachingNotifUtc = DateTime.UtcNow;
                        await repo.UpdateAsync(task);
                        continue;
                    }
                    var notification = new Notification
                    {
                        UserId= targetUserId,
                        TaskId = task.Id,
                        WorkspaceId = task.WorkspaceId,
                        Message = $"Deadline approaching: {task.Title}",
                        CreatedAtUtc = DateTime.UtcNow,
                        IsRead = false
                    };
                    await notificationRepo.CreateAsync(notification);
                    task.LastDeadlineAproachingNotifUtc = DateTime.UtcNow;
                    await repo.UpdateAsync(task);

                }
                foreach(var task in missed)
                {
                    Console.WriteLine($"Deadline missed : {task.Title}");
                    var targetUserId = task.AssignedToUserId ?? task.CreatedByUserId;
                    var targetUser = await userRepo.GetByIdAsync(targetUserId);

                    if (targetUser == null ||
                        !targetUser.InAppNotificationEnabled ||
                        !targetUser.DeadlineMissedNotificationEnabled)
                    {
                        task.LastDeadlineMissedNotifUtc = DateTime.UtcNow;
                        await repo.UpdateAsync(task);
                        continue;
                    }

                    var notification = new Notification
                    {
                        UserId = targetUserId,
                        TaskId = task.Id,
                        WorkspaceId = task.WorkspaceId,
                        Message = $"Deadline missed : {task.Title}",
                        CreatedAtUtc = DateTime.UtcNow,
                        IsRead = false
                    };
                    await notificationRepo.CreateAsync(notification);
                    task.LastDeadlineMissedNotifUtc = DateTime.UtcNow;
                    await repo.UpdateAsync(task);
                }
                foreach (var task in recurringCompleted) 
                {
                    Console.WriteLine($"Recurring task completed : {task.Title}");
                    var targetUserId = task.AssignedToUserId ?? task.CreatedByUserId;
                    var targetUser = await userRepo.GetByIdAsync(targetUserId);

                    if (targetUser == null ||
                        !targetUser.InAppNotificationEnabled ||
                        !targetUser.RecurringCompletionNotificationEnabled)
                    {
                        task.LastRecurringCompletionNotifUtc = DateTime.UtcNow;
                        await repo.UpdateAsync(task);
                        continue;
                    }
                    var notification = new Notification
                    {
                        UserId = targetUserId,
                        TaskId = task.Id,
                        WorkspaceId = task.WorkspaceId,
                        Message = $"Recurring task completed: {task.Title}",
                        CreatedAtUtc = DateTime.UtcNow,
                        IsRead = false
                    };
                    await notificationRepo.CreateAsync(notification);
                    task.LastRecurringCompletionNotifUtc = DateTime.UtcNow;
                    await repo.UpdateAsync(task);
                }


                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
