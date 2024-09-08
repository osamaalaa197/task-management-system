using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagement.Core.Consts
{
    public static class EmailTemplates
    {
        public static readonly string EmailConfirmationTemplate = @"
        <p>Hi {0},</p>
        <p>Please confirm your email address by clicking the link below:</p>
        <p><a href='{1}' style='font-size:18px;'>Confirm Email</a></p>
        <p>If you did not request this email, please ignore it.</p>
        <p>Best regards,<br/>Your Application Team</p>";
        public static string GetTaskAssignmentBody(string fullName, string taskTitle, string description, DateTime dueDate)
        {
            return $@"
            <p>Hi {fullName},</p>
            <p>You have been assigned a new task. Here are the details:</p>
            <ul>
                <li><strong>Task Title:</strong> {taskTitle}</li>
                <li><strong>Description:</strong> {description}</li>
                <li><strong>Due Date:</strong> {dueDate.ToString("MMMM dd, yyyy")}</li>
            </ul>
            <p>Please make sure to complete the task by the due date.</p>
            <p>Best regards,<br/>Task Management Team</p>";
        }
    }
}
