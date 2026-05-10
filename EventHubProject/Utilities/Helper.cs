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
}
