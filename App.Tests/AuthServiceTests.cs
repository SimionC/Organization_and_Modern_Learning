using App.Server.Models;
using App.Server.ORM;
using App.Server.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace App.Tests;

public sealed class AuthServiceTests
{
    [Fact]
    public void LoginTrimsEmailAndComparesItWithoutCaseSensitivity()
    {
        using var dbContext = CreateDatabase();
        var authService = new AuthService(dbContext);
        Assert.NotNull(authService.Login(new LoginModel
        {
            Email = "STUDENT@example.test",
            Password = "OriginalPassword123!"
        }));
        Assert.Null(authService.Login(new LoginModel
        {
            Email = "student@example.test",
            Password = "incorrect"
        }));
    }

    [Fact]
    public void ChangePasswordRejectsShortPasswordsAndAcceptsValidPassword()
    {
        using var dbContext = CreateDatabase();
        var authService = new AuthService(dbContext);
        const int userId = 7;

        Assert.False(authService.ChangePassword(userId, "short"));
        Assert.True(authService.ChangePassword(userId, "LongEnough123!"));
        Assert.False(dbContext.Users.Single().MustChangePassword);
        Assert.NotNull(authService.Login(new LoginModel
        {
            Email = "student@example.test",
            Password = "LongEnough123!"
        }));
    }

    [Fact]
    public void DemoSeederCreatesALoggableTeacherAndCourseOnlyOnce()
    {
        using var dbContext = CreateEmptyDatabase();

        DemoDataSeeder.Seed(dbContext);
        DemoDataSeeder.Seed(dbContext);

        var demoUser = new AuthService(dbContext).Login(new LoginModel
        {
            Email = DemoDataSeeder.DemoEmail,
            Password = DemoDataSeeder.DemoPassword
        });

        Assert.NotNull(demoUser);
        Assert.Equal(UserRoles.Teacher, demoUser.UserTypeId);
        Assert.Single(dbContext.Courses);
    }

    private static AppDbContext CreateDatabase()
    {
        var dbContext = CreateEmptyDatabase();
        var user = new User
        {
            Id = 7,
            Email = "student@example.test",
            Nume = "Student",
            Prenume = "Example",
            UserTypeId = UserRoles.Student,
            Password = string.Empty,
            MustChangePassword = true
        };
        user.Password = new PasswordHasher<User>().HashPassword(user, "OriginalPassword123!");
        dbContext.Users.Add(user);
        dbContext.SaveChanges();
        return dbContext;
    }

    private static AppDbContext CreateEmptyDatabase()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
