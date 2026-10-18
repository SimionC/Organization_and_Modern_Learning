using App.Server.Models;
using App.Server.ORM;
using Microsoft.AspNetCore.Identity;

namespace App.Server.Services;

/// <summary>Creates the minimum fake records needed to explore a fresh local install.</summary>
public static class DemoDataSeeder
{
    public const string DemoEmail = "reviewer@example.com";
    public const string DemoPassword = "Reviewer123!";

    public static void Seed(AppDbContext dbContext)
    {
        EnsureLookupRows(dbContext);

        if (dbContext.Users.Any())
            return;

        var teacher = new User
        {
            Email = DemoEmail,
            Nume = "Alex",
            Prenume = "Reviewer",
            UserTypeId = UserRoles.Teacher,
            Password = string.Empty,
            MustChangePassword = false
        };
        teacher.Password = new PasswordHasher<User>().HashPassword(teacher, DemoPassword);

        dbContext.Users.Add(teacher);
        dbContext.Courses.Add(new Course
        {
            Title = "Introduction to Web Development",
            Description = "A sample course for exploring the course dashboard.",
            Teacher = teacher,
            JoinPassword = "123456",
            IsClosed = false
        });
        dbContext.SaveChanges();
    }

    private static void EnsureLookupRows(AppDbContext dbContext)
    {
        var expectedRoles = new[]
        {
            new UserType { Id = UserRoles.Student, Name = UserRoles.StudentName, Permissions = "" },
            new UserType { Id = UserRoles.Teacher, Name = UserRoles.TeacherName, Permissions = "" },
            new UserType { Id = UserRoles.Admin, Name = UserRoles.AdminName, Permissions = "" }
        };

        foreach (var role in expectedRoles)
        {
            if (!dbContext.UserTypes.Any(existing => existing.Id == role.Id))
                dbContext.UserTypes.Add(role);
        }

        if (!dbContext.VisibilityTypes.Any(existing => existing.Id == 1))
        {
            dbContext.VisibilityTypes.Add(new VisibilityType { Id = 1, Name = "private" });
        }

        dbContext.SaveChanges();
    }
}
