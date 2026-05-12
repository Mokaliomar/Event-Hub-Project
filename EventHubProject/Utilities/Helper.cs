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
        var Organizers = context.Organizers.ToList();
        foreach (var organizer in Organizers)
        {
            System.Console.WriteLine();
            Console.WriteLine("Name: " + organizer.Name);
            Console.WriteLine("Email: " + organizer.Email);
            Console.WriteLine("Phone Number: " + organizer.PhoneNumber);
            Console.WriteLine("\n====================================");
            Thread.Sleep(1000);
        }
    }
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
}
