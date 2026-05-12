using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using EventHubProject.Models;
using EventHubProject.Models.Enums;
using EventHubProject.Utilities;
using Microsoft.EntityFrameworkCore;

namespace EventHubProject
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Intro();
            // Helper.AddingDummyAttendeeRecords();
            bool running = true;
            while (running)
            {
                Console.WriteLine("Are you a participant or an organizer (P/O)?");
                char choice = char.ToLower(Convert.ToChar(Console.ReadLine() ?? ""));
                while (char.IsLetter(choice) == false)
                {
                    System.Console.WriteLine("Invalid input! Please enter 'P' for participant or 'O' for organizer.");
                    choice = char.ToLower(Convert.ToChar(Console.ReadLine() ?? ""));
                }
                switch (char.ToLower(choice))
                {
                    case 'p':
                        AttendeeSection();
                        running = false;
                        break;
                    case 'o':
                        OrganizerSection();
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid input! Please enter 'P' for participant or 'O' for organizer.");
                        break;
                }
            }
        }
        static void Intro()
        {
            Console.Clear();
            Console.WriteLine("=========================================");
            Console.WriteLine("        Welcome to EventHub CLI!         ");
            Console.WriteLine("=========================================\n");
        }
        static void AttendeeSection()
        {
            using (var context = new MyDbContext())
            {
                System.Console.WriteLine("Welcome to the Attendee Section!");
                System.Console.Write("Please enter your name: ");
                string? name = Console.ReadLine();

                // Changing the attendee detection logic
                var attendee = context.Attendees.FirstOrDefault(a => a.FirstName + ' ' + a.LastName == name) ?? context.Attendees.FirstOrDefault(a => a.FirstName == name) ?? context.Attendees.FirstOrDefault(a => a.LastName == name);
                if (attendee == null)
                {
                    System.Console.WriteLine("[System] Sorry, you were not found in our database.");
                    System.Console.WriteLine("You will need to register first.");
                    Thread.Sleep(2500); 
                    AttendeeRegistration(ref attendee);
                }
                Thread.Sleep(2500);
                Console.Clear();
                int choice = 0;
                while (choice != 5)
                {
                    Console.Clear();
                    Console.WriteLine($"Welcome {attendee.FirstName} {attendee.LastName}!");
                    Console.WriteLine("================================");
                    Console.WriteLine("1. View all Events");
                    Console.WriteLine("2. View all Organizers");
                    Console.WriteLine("3. Exit");

                    bool validChoice = false;
                    while (!validChoice)
                    {
                        choice = GetValidInput<int>(
                            "Enter your choice: ",
                            "Invalid input! Please enter a number between 1 and 5.",
                            int.TryParse
                        );
                        switch (choice)
                        {
                            case 1:
                                Helper.ViewAllEvents(context);
                                validChoice = true;
                                break;
                            case 2:
                                Helper.ViewAllOrganizers(context);
                                validChoice = true;
                                break;
                            case 3:
                                validChoice = true;
                                System.Console.WriteLine("Goodbye!");
                                return;
                            default:
                                System.Console.WriteLine("Invalid input! Please enter a number between 1 and 5.");
                                break;
                        }
                    }
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }

        }
        static void OrganizerSection()
        {
            using (var context = new MyDbContext())
            {
                // ==========================================
                // 1. مرحلة تسجيل الدخول (خارج اللوب)
                // ==========================================
                string? orgName = GetValidString(
                    "Enter your name (Organizer Name): ",
                    "Invalid input! Please use alphabetic characters only.\n",
                    s => s.All(c => char.IsLetter(c) || char.IsWhiteSpace(c))
                );
                /* Old Validation
                Console.Write("Please enter your name (Organizer Name): ");
                string? orgName = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(orgName) || !orgName.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    Console.Write("Invalid input! Please use alphabetic characters only.\n");
                    Console.Write("Please enter your name again: ");
                    orgName = Console.ReadLine();
                } */

                var organizer = context.Organizers.FirstOrDefault(o => o.Name == orgName);

                if (organizer == null)
                {
                    // ==========================================
                    // Register Flow (مستخدم جديد)
                    // ==========================================
                    Console.WriteLine($"\n[System] User '{orgName}' not found. Let's create a new account.");

                    Console.Write("Enter a new password: ");
                    string password = Console.ReadLine() ?? "";

                    // تشفير الباسورد باستخدام BCrypt
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                    organizer = new Organizer
                    {
                        Name = orgName,
                        PasswordHash = hashedPassword // بنحفظ النسخة المتشفرة بس
                    };

                    context.Organizers.Add(organizer);
                    context.SaveChanges();
                    Console.WriteLine("\n[System] Account created successfully! You are now logged in.");
                    /* organizer = new Organizer { Name = orgName };
                    context.Organizers.Add(organizer);
                    context.SaveChanges();
                    Console.WriteLine($"\n[System] New Organizer '{orgName}' created successfully."); */
                }
                else
                {
                    // ==========================================
                    // Login Flow (مستخدم موجود بالفعل)
                    // ==========================================
                    Console.WriteLine($"\n[System] Account found for '{organizer.Name}'.");

                    // شيك لو الـ PasswordHash فاضي (مستخدم قديم)
                    if (string.IsNullOrEmpty(organizer.PasswordHash))
                    {
                        Console.WriteLine($"\n[System] Welcome {organizer.Name}! Since this is an old account, you need to set a password.");
                        Console.Write("Enter your new password: ");
                        string newPassword = Console.ReadLine() ?? "";

                        // تشفير وحفظ الباسورد
                        organizer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        context.SaveChanges();

                        Console.WriteLine("[Success] Password has been set! You can now use it to login next time.");
                    }
                    else
                    {
                        Console.Write("Please enter your password: ");
                        string password = Console.ReadLine() ?? "";

                        // التحقق من الباسورد: بندي للمكتبة الباسورد العادي والهاش اللي في الداتا بيز وهي بتقارنهم
                        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, organizer.PasswordHash);

                        int attempts = 3;
                        while (!isPasswordValid && attempts > 0)
                        {
                            Console.WriteLine("\n[Error] Incorrect password! Access Denied.");
                            attempts--;

                            Console.Write("Please enter your password: ");
                            password = Console.ReadLine() ?? "";

                            isPasswordValid = BCrypt.Net.BCrypt.Verify(password, organizer.PasswordHash);
                        }

                        if (attempts == 0)
                        {
                            System.Console.WriteLine("Sorry, You tried to many Times to enter the Password!");
                            return;
                        }

                        Console.WriteLine("\n[System] Login successful! Welcome back.");
                        Task.Delay(1500);
                    }
                }

                Task.Delay(2500); // استراحة بسيطة عشان يلحق يقرا الرسالة

                // ==========================================
                // 2. مرحلة القائمة الرئيسية (جوه اللوب)
                // ==========================================
                bool isRunning = true;
                while (isRunning)
                {
                    Console.Clear();
                    Console.WriteLine($"=== Main Menu | Organizer: {organizer.Name} ===");
                    Console.WriteLine("1. Add New Event");
                    Console.WriteLine("2. View My Events");
                    Console.WriteLine("3. Edit an Event");
                    Console.WriteLine("4. Delete an Event");
                    Console.WriteLine("5. Exit");
                    Console.WriteLine("=========================================");
                    Console.Write("Choose an option (1-5): ");

                    string? choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            // === إضافة حدث ===
                            Console.WriteLine("\n--- Add Event ---");
                            Console.Write("Event Title: ");
                            string eventTitle = Console.ReadLine() ?? "No Title";

                            Console.Write("Event Details: ");
                            string? eventDescription = Console.ReadLine();


                            DateTime eventDate = GetValidInput<DateTime>(
                                "Event Date (yyyy-mm-dd): ",
                                "Invalid format! Please enter date as (yyyy-mm-dd): ",
                                DateTime.TryParse
                            );
                            /* 
                            Console.Write("Event Date (yyyy-mm-dd): ");
                            string dateInput = Console.ReadLine();
                            while (!DateTime.TryParse(dateInput, out eventDate))
                            {
                                Console.Write("Invalid format! Please enter date as (yyyy-mm-dd): ");
                                dateInput = Console.ReadLine();
                            } */

                            var newEvent = new Event
                            {
                                Title = eventTitle,
                                DetailedDescription = eventDescription,
                                StartDate = eventDate,
                                OrganizerId = organizer.Id
                            };

                            context.Events.Add(newEvent);
                            context.SaveChanges();
                            Console.WriteLine($"[Success] Event '{newEvent.Title}' added successfully!");
                            break;

                        case "2":
                            // === عرض الأحداث ===
                            Console.WriteLine("\n--- My Events ---");
                            var myEvents = context.Events.Where(e => e.OrganizerId == organizer.Id).ToList();
                            if (myEvents.Count == 0)
                            {
                                Console.WriteLine("You have no events yet.");
                            }
                            else
                            {
                                foreach (var ev in myEvents)
                                {
                                    // بنطبع الـ ID عشان هنحتاجه في التعديل والمسح
                                    Console.WriteLine($"ID: {ev.Id} | \nTitle: {ev.Title} \nDescription: {ev.DetailedDescription} \nDate: {ev.StartDate:d}");
                                    Console.WriteLine("-----------------------------------------------");
                                }
                            }
                            break;

                        case "3":
                            // === تعديل حدث ===
                            Console.WriteLine("\n--- Edit Event ---");
                            Console.Write("Enter the ID of the Event you want to edit: ");
                            if (int.TryParse(Console.ReadLine(), out int editId))
                            {
                                // بنجيب الـ Event من الداتا بيز ونتأكد إنه بتاع نفس المنظم
                                var eventToEdit = context.Events.FirstOrDefault(e => e.Id == editId && e.OrganizerId == organizer.Id);

                                if (eventToEdit != null)
                                {
                                    Console.Write($"New Title (Leave empty to keep '{eventToEdit.Title}'): ");
                                    string? newTitle = Console.ReadLine();
                                    if (!string.IsNullOrWhiteSpace(newTitle)) eventToEdit.Title = newTitle;

                                    Console.Write($"New Description (Leave empty to keep '{eventToEdit.DetailedDescription}'): ");
                                    string? NewDescription = Console.ReadLine();
                                    if (!string.IsNullOrWhiteSpace(NewDescription)) eventToEdit.DetailedDescription = NewDescription;

                                    Console.WriteLine($"New Date (Leave empty to keep '{eventToEdit.StartDate:d}') (yyyy-mm-dd): ");
                                    string? newDateInput = Console.ReadLine();
                                    if (DateTime.TryParse(newDateInput, out DateTime newDate)) eventToEdit.StartDate = newDate;

                                    context.SaveChanges();
                                    Console.WriteLine("[Success] Event updated successfully!");
                                }
                                else
                                {
                                    Console.WriteLine("[Error] Event not found or you don't have permission to edit it.");
                                }
                            }
                            break;

                        case "4":
                            // === مسح حدث ===
                            Console.WriteLine("\n--- Delete Event ---");
                            Console.Write("Enter the ID of the Event you want to delete: ");
                            if (int.TryParse(Console.ReadLine(), out int deleteId))
                            {
                                var eventToDelete = context.Events.FirstOrDefault(e => e.Id == deleteId && e.OrganizerId == organizer.Id);

                                if (eventToDelete != null)
                                {
                                    context.Events.Remove(eventToDelete);
                                    context.SaveChanges();
                                    Console.WriteLine("[Success] Event deleted successfully!");
                                }
                                else
                                {
                                    Console.WriteLine("[Error] Event not found or you don't have permission to delete it.");
                                }
                            }
                            break;

                        case "5":
                            // === خروج ===
                            isRunning = false;
                            Console.WriteLine("Goodbye!");
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please select 1-5.");
                            break;
                    }

                    if (isRunning)
                    {
                        Console.WriteLine("\nPress any key to return to the main menu...");
                        Console.ReadKey();
                    }
                }
            }
        }

        static void AttendeeRegistration(ref Attendee? attendee)
        {
            using (var context = new MyDbContext())
            {
                Console.Clear();
                Console.WriteLine("Welcome to the Attendee Registration Section!");
                Console.WriteLine("=============================================");
                Console.WriteLine("Please enter your information:");

                Console.Write("First Name: ");
                string firstName = Console.ReadLine() ?? "Unknown";

                Console.Write("Last Name: ");
                string lastName = Console.ReadLine() ?? "Unknown";

                string email;
                while (true)
                {
                    Console.Write("Email: ");
                    email = Console.ReadLine() ?? "Unknown";

                    // Efficiently check if the email already exists in the database
                    bool emailExists = context.Attendees.Any(a => a.Email == email && a.Email != "Unknown");

                    if (emailExists)
                    {
                        Console.WriteLine("[Error] This email is already registered. Please enter a different email.");
                    }
                    else
                    {
                        break; // Exit the loop if the email is unique
                    }
                }

                attendee = new Attendee { FirstName = firstName, LastName = lastName, Email = email };

                Console.Write("Do you have a specific address (y/n): ");
                char yn = Convert.ToChar(Console.ReadLine() ?? "n");
                if (yn == 'y')
                {
                    Console.WriteLine("Street: ");
                    string street = Console.ReadLine() ?? "Unknown";

                    Console.WriteLine("City: ");
                    string city = Console.ReadLine() ?? "Unknown";

                    Console.WriteLine("Country: ");
                    string country = Console.ReadLine() ?? "Unknown";

                    Console.WriteLine("Postal Code: ");
                    string postalCode = Console.ReadLine() ?? "Unknown";

                    attendee.Address = new Address { Street = street, City = city, Country = country, PostalCode = postalCode };
                }

                #region Badge Settings
                Tier tier = Tier.Standard;
                if (email.EndsWith("@eventhub.com"))
                {
                    tier = Tier.VIP;
                }
                attendee.Badge = new Badge
                {
                    DateIssued = DateTime.Now,
                    Tier = tier
                };
                #endregion

                context.Attendees.Add(attendee);
                context.SaveChanges();
                Console.WriteLine("[System] Registration successful!");
            }
        }

        #region Validation Section
        static string GetValidString(string prompt, string errorMessage, Func<string, bool> validationRule)
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
        delegate bool TryParseHandler<T>(string input, out T result);

        static T GetValidInput<T>(string prompt, string errorMessage, TryParseHandler<T> parser)
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
    }
}