using Microsoft.CodeAnalysis.CSharp.Syntax;
using TaskFlow.UI.Models;

namespace TaskFlow.UI.Data
{
    public class TaskReminderService
    {
        public bool ShouldTriggerReminder(TaskItem task, DateTime nowUtc)
        {
            if (task.IsCompleted) return false;
            if (!task.DueDate.HasValue) return false;
            if (task.ReminderMinutesBefore == null || task.ReminderMinutesBefore.Count == 0) return false;
            if (task.LastReminderSentUtc.HasValue &&
                task.LastReminderSentUtc.Value.Date == nowUtc.Date)
            {
                return false;
            }
            var dueUtc = task.DueDate.Value;

            foreach (var minutesBefore in task.ReminderMinutesBefore)
            {
                var reminderTime = dueUtc.AddMinutes(-minutesBefore);

                if (nowUtc >= reminderTime && nowUtc < dueUtc)
                    return true;
            }

            return false;
        }

        public List<TaskItem> GetTasksNeedingReminder(IEnumerable<TaskItem> tasks, DateTime nowUtc)
        {
            return tasks
                .Where(t => ShouldTriggerReminder(t, nowUtc))
                .ToList();
        }

        public bool ShouldTriggerDeadlineApproaching(TaskItem task, DateTime nowUtc,int minutesBefore)
        {
            if (task.IsCompleted) return false;
            if (!task.DueDate.HasValue) return false;
            if (task.LastDeadlineAproachingNotifUtc.HasValue) return false;

            var dueUtc = task.DueDate.Value;

            if (dueUtc <= nowUtc) return false;

            var timeUntilDue = dueUtc - nowUtc;

            return timeUntilDue <= TimeSpan.FromMinutes(minutesBefore);
        }

        public List<TaskItem> GetTasksWithDeadlineApproaching(IEnumerable<TaskItem> tasks,DateTime nowUtc,int minutesBefore)
        {
            return tasks
                .Where(t => ShouldTriggerDeadlineApproaching(t, nowUtc, minutesBefore))
                .ToList();
        }

        public bool ShouldTriggerDeadlineMissed(TaskItem task, DateTime nowUtc)
        {
            if (task.IsCompleted) return false;
            if (!task.DueDate.HasValue) return false;
            if (task.LastDeadlineMissedNotifUtc.HasValue) return false;
            return task.DueDate.Value < nowUtc;
        }
        public List<TaskItem> GetTasksWithDeadlineMissed(IEnumerable<TaskItem> tasks,DateTime nowUtc)
        {
            return tasks.Where(t => ShouldTriggerDeadlineMissed(t, nowUtc))
                .ToList();
        }

        public bool ShouldTriggerRecurringCompletion(TaskItem task)
        {
            if (!task.IsCompleted) return false;
            if (!task.IsRecurring) return false;
            if (task.LastRecurringCompletionNotifUtc.HasValue) return false;
            return true;
        }

        public List<TaskItem> GetCompletedRecurringTaskNeedingNotification(IEnumerable<TaskItem> tasks)
        {
            return tasks
                .Where(t => ShouldTriggerRecurringCompletion(t))
                .ToList();
        }
    }
}