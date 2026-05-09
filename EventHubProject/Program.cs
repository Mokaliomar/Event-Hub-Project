using System;
using System.Linq;
using System.Threading.Tasks;
using EventHubProject.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHubProject
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (var context = new MyDbContext())
            {
                Console.Clear();
                Console.WriteLine("=========================================");
                Console.WriteLine("        Welcome to EventHub CLI!         ");
                Console.WriteLine("=========================================\n");

                // ==========================================
                // 1. مرحلة تسجيل الدخول (خارج اللوب)
                // ==========================================
                Console.Write("Please enter your name (Organizer Name): ");
                string? orgName = Console.ReadLine();

                while (string.IsNullOrWhiteSpace(orgName) || !orgName.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                {
                    Console.Write("Invalid input! Please use alphabetic characters only.\n");
                    Console.Write("Please enter your name again: ");
                    orgName = Console.ReadLine();
                }

                var organizer = context.Organizers.FirstOrDefault(o => o.Name == orgName);

                if (organizer == null)
                {
                    organizer = new Organizer { Name = orgName };
                    context.Organizers.Add(organizer);
                    context.SaveChanges();
                    Console.WriteLine($"\n[System] New Organizer '{orgName}' created successfully.");
                }
                else
                {
                    Console.WriteLine($"\n[System] Welcome back, {organizer.Name}!");
                }

                await Task.Delay(2500); // استراحة بسيطة عشان يلحق يقرا الرسالة

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
                            string eventTitle = Console.ReadLine();

                            Console.Write("Event Details: ");
                            string? eventDescription = Console.ReadLine();

                            Console.Write("Event Date (yyyy-mm-dd): ");
                            DateTime eventDate;
                            string dateInput = Console.ReadLine();
                            while (!DateTime.TryParse(dateInput, out eventDate))
                            {
                                Console.Write("Invalid format! Please enter date as (yyyy-mm-dd): ");
                                dateInput = Console.ReadLine();
                            }

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
                                    if(!string.IsNullOrWhiteSpace(NewDescription)) eventToEdit.DetailedDescription = NewDescription;

                                    Console.WriteLine($"New Date (Leave empty to keep '{eventToEdit.StartDate:d}') (yyyy-mm-dd): ");
                                    string? newDateInput = Console.ReadLine();
                                    if(DateTime.TryParse(newDateInput, out DateTime newDate)) eventToEdit.StartDate = newDate ;

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
    }
}