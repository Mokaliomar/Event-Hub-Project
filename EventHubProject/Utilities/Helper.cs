using System;
using EventHubProject.Models;
using Microsoft.EntityFrameworkCore;
namespace EventHubProject.Utilities;

public static class Helper
{
    public static void ViewAllEvents(MyDbContext context)
    {
        var Events = context.Events.Include(e => e.Organizer).ToList();
        foreach (var theEvent in Events)
        {
            System.Console.WriteLine();
            Console.WriteLine("ID: " + theEvent.Id);
            Console.WriteLine("Title: " + theEvent.Title);
            Console.WriteLine("Description: " + theEvent.DetailedDescription);
            Console.WriteLine("Start Date: " + theEvent.StartDate);
            Console.WriteLine("Organizer: " + theEvent.Organizer?.Name);
            Console.WriteLine("\n====================================");
            Thread.Sleep(1000);
        }
    }
    public static void ViewAllOrganizers(MyDbContext context)
    {
        var Organizers = context.Organizers.Include(o => o.Account).ToList();
        foreach (var organizer in Organizers)
        {
            Console.WriteLine();
            Console.WriteLine("Name: " + organizer.Name);
            Console.WriteLine("Email: " + organizer.Account.Email);
            Console.WriteLine("Phone Number: " + organizer.Account.PhoneNumber);
            Console.WriteLine("\n====================================");
            Thread.Sleep(1000);
        }
    }

    public static void AttendeeEvents(MyDbContext context, Attendee attendee)
    {
        // var attendeeEvents = context.EventRegistrations.Where(er => er.AttendeeId == attendee.AttendeeId).ToList();
        var attendeeEvents = (from er in context.EventRegistrations
                              join e in context.Events
                              on er.EventId equals e.Id
                              where er.AttendeeId == attendee.AttendeeId
                              select new
                              {
                                  e.Title,
                                  e.DetailedDescription,
                                  e.StartDate,
                                  er.ShortNote,
                                  er.RegistrationDate
                              }).ToList();
        foreach (var theEvent in attendeeEvents)
        {
            System.Console.WriteLine($"Title : {theEvent.Title}");
            System.Console.WriteLine($"Description : {theEvent.DetailedDescription}");
            System.Console.WriteLine($"Start Date : {theEvent.StartDate}");
            System.Console.WriteLine($"Short Note : {theEvent.ShortNote}");
            System.Console.WriteLine($"Registration Date : {theEvent.RegistrationDate}");
        }
    }

    public static void OrganizerProfile(MyDbContext context, Organizer organizer)
    {
        if (organizer.Account == null)
        {
            Console.WriteLine("[Error] Account data is missing for this organizer.");
            return;
        }

        // Ensure the profile page object exists so we don't hit nulls during display or update
        organizer.Account.ProfilePage ??= new ProfilePage();

        Console.Clear();
        System.Console.WriteLine("====================================");
        System.Console.WriteLine("    USER PROFILE   ");
        System.Console.WriteLine("====================================");

        System.Console.WriteLine($"Name : {organizer.Name}");
        System.Console.WriteLine($"Title : {organizer.Account.ProfilePage.Title ?? "Not Set"}");
        System.Console.WriteLine($"Email : {organizer.Account.Email}");
        System.Console.WriteLine($"Biography : {organizer.Account.ProfilePage.Biography ?? "No biography provided."}");
        System.Console.WriteLine($"Phone Number : {organizer.Account.PhoneNumber}");
        System.Console.WriteLine($"The Logo : {organizer.Account.ProfilePage.Logo ?? "No Logo"}");

        char yn = GetValidInput<char>(
            "Do you want to update the profile? (y/n): ",
            "Invalid input! Please enter 'y' or 'n'.",
            char.TryParse
        );
        if (yn == 'y')
        {
            UpdateProfilePage(context, organizer);
        }
    }

    public static void UpdateProfilePage(MyDbContext context, Organizer organizer)
    {
        System.Console.WriteLine("====================================");
        #region Update Profile
        string? newName = GetOptionalString(
            "Enter the new name (Enter to skip): ",
            "Invalid input! Please use alphabetic characters only.\n",
            s => s.All(c => char.IsLetter(c) || char.IsWhiteSpace(c))
        );

        string? newTitle = GetOptionalString(
            "Enter the new title (Enter to skip): ",
            "Invalid input! Please use alphabetic characters only.\n",
            s => s.All(c => char.IsLetter(c) || char.IsWhiteSpace(c))
        );

        string? newBiography = GetOptionalString(
            "Enter the new biography (Enter to skip): ",
            "Invalid input! Please use alphabetic characters only.\n",
            s => s.All(c => char.IsLetter(c) || char.IsWhiteSpace(c))
        );

        string? newLogo = GetOptionalString(
            "Enter the new logo URL (Enter to skip): ",
            "Invalid input! Please use URL format only.\n",
            s => Uri.TryCreate(s, UriKind.Absolute, out _)
        );
        #endregion
        // Safe assignments — no null-conditional on the left side
        if (newName != null) organizer.Name = newName;

        // We assume ProfilePage is initialized by the caller (OrganizerProfile)
        if (newTitle != null) organizer.Account.ProfilePage.Title = newTitle;
        if (newBiography != null) organizer.Account.ProfilePage.Biography = newBiography;
        if (newLogo != null) organizer.Account.ProfilePage.Logo = newLogo;

        context.SaveChanges();

        System.Console.WriteLine("[Success] Profile updated successfully!");
    }

    #region Validation Section
    public static string GetValidString(string prompt, string errorMessage, Func<string, bool> validationRule)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        // طول ما الإدخال فاضي أو مش بيحقق الشرط (validationRule)
        while (string.IsNullOrWhiteSpace(input) || !validationRule(input))
        {
            Console.WriteLine($"\n[Error] {errorMessage}");
            Console.Write(prompt);
            input = Console.ReadLine();
        }

        return input;
    }

    // Separate helper that allows empty input (returns null on skip)
    public static string? GetOptionalString(string prompt, string errorMessage, Func<string, bool> validationRule)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        // Empty = skip, return null
        if (string.IsNullOrWhiteSpace(input)) return null;

        // Non-empty must pass validation
        while (!validationRule(input))
        {
            Console.WriteLine($"\n[Error] {errorMessage}");
            Console.Write(prompt);
            input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) return null;
        }

        return input;
    }

    public delegate bool TryParseHandler<T>(string input, out T result);

    public static T GetValidInput<T>(string prompt, string errorMessage, TryParseHandler<T> parser)
    {
        Console.Write(prompt);

        T result;
        // اللوب هيفضل شغال طول ما الـ parser مش قادر يحول الـ string للنوع T
        while (!parser(Console.ReadLine() ?? "", out result))
        {
            Console.WriteLine($"\n[Error] {errorMessage}");
            Console.Write(prompt);
        }

        return result; // هترجعلك الداتا متحولة وجاهزة
    }
    #endregion

    #region Testing
    public static void AddingDummyAttendeeRecords()
    {
        using (var context = new MyDbContext())
        {
            var dummyAttendees = new List<Attendee>
            {
                new Attendee
                {
                    FirstName = "Omar",
                    LastName = "Morshed",
                    Email = "omar.morshed@example.com",
                    Address = new Address
                    {
                        Street = "15 Maadi St",
                        City = "Cairo",
                        Country = "Egypt",
                        PostalCode = "11431"
                    }
                },
                new Attendee
                {
                    FirstName = "Ahmed",
                    LastName = "Ali",
                    Email = "ahmed.ali@example.com",
                    Address = new Address
                    {
                        Street = "10 Tahrir Square",
                        City = "Cairo",
                        Country = "Egypt",
                        PostalCode = "11511"
                    }
                },
                new Attendee
                {
                    FirstName = "Sara",
                    LastName = "Mahmoud",
                    Email = "sara.m@example.com",
                    Address = new Address
                    {
                        Street = "55 Gleem",
                        City = "Alexandria",
                        Country = "Egypt",
                        PostalCode = "21511"
                    }
                },
                new Attendee
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Address = new Address
                    {
                        Street = "123 Tech Blvd",
                        City = "New York",
                        Country = "USA",
                        PostalCode = "10001"
                    }
                },
                new Attendee
                {
                    FirstName = "Laila",
                    LastName = "Hassan",
                    Email = null, // الـ Email مسموح يكون null في الـ Model بتاعك
                    Address = new Address
                    {
                        Street = "90 Abbas El Akkad",
                        City = "Cairo",
                        Country = "Egypt",
                        PostalCode = "11768"
                    }
                }
            };

            context.Attendees.AddRange(dummyAttendees);
            context.SaveChanges();

            Console.WriteLine("[System] Sample Attendees with their Addresses have been seeded successfully!");
        }
    }

    /* public static void MoveRecords()
    {
        using (var context = new MyDbContext())
        {
            var organizers = context.Organizers.ToList();

            foreach(var organizer in organizers)
            {
                var account = new Account
                {
                    Name = organizer.Name,
                    Email = organizer.Email,
                    PhoneNumber = organizer.PhoneNumber,
                    PasswordHash = organizer.PasswordHash,
                    Role = Enums.Role.Organizer,
                    OrganizerId = organizer.Id
                };
                context.Accounts.Add(account);
            }
            context.SaveChanges();
        }
    } */
    #endregion
}
