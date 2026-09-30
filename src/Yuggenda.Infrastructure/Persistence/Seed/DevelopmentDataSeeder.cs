using Microsoft.EntityFrameworkCore;
using Yuggenda.Domain.Entities;
using Yuggenda.Domain.Enums;
using Yuggenda.Infrastructure.Persistence.Context;

namespace Yuggenda.Infrastructure.Persistence.Seed;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(YuggendaDbContext context)
    {
        if (await context.Users.AnyAsync(user =>
            user.Email == "owner@yuggenda.dev"))
        {
            return;
        }

        var owner = new User(
            "owner@yuggenda.dev",
            SeedPasswordHash,
            "Oliver",
            "Owner");

        var admin = new User(
            "admin@yuggenda.dev",
            SeedPasswordHash,
            "Alice",
            "Admin");

        var staff1 = new User(
            "staff1@yuggenda.dev",
            SeedPasswordHash,
            "Steve",
            "Staff");

        var staff2 = new User(
            "staff2@yuggenda.dev",
            SeedPasswordHash,
            "Sarah",
            "Staff");

        var customerUser = new User(
            "customer@yuggenda.dev",
            SeedPasswordHash,
            "Carlos",
            "Customer");

        var secondOwner = new User(
            "owner2@yuggenda.dev",
            SeedPasswordHash,
            "Olivia",
            "Owner");

        var secondAdmin = new User(
            "admin2@yuggenda.dev",
            SeedPasswordHash,
            "Andrew",
            "Admin");

        var secondStaff = new User(
            "staff3@yuggenda.dev",
            SeedPasswordHash,
            "Sophie",
            "Staff");

        context.Users.AddRange(
            owner,
            admin,
            staff1,
            staff2,
            customerUser,
            secondOwner,
            secondAdmin,
            secondStaff);

        // BUSINESS
        var barberShop = new Business(
            "Yuggenda Barber Shop",
            "America/Argentina/Buenos_Aires",
            "Barbershop for haircuts and grooming services.",
            "contact@barbershop.yuggenda.dev",
            "+54 11 5555-1000");

        var fitness = new Business(
            "Yuggenda Fitness",
            "America/Argentina/Buenos_Aires",
            "Personal training and fitness services.",
            "contact@fitness.yuggenda.dev",
            "+54 11 5555-2000");

        context.Businesses.AddRange(barberShop, fitness);

        // BUSINESS MEMBER
        var barberOwner = new BusinessMember(
            barberShop.Id,
            owner.Id,
            BusinessRole.Owner);

        var barberAdmin = new BusinessMember(
            barberShop.Id,
            admin.Id,
            BusinessRole.Admin);

        var barberStaff1 = new BusinessMember(
            barberShop.Id,
            staff1.Id,
            BusinessRole.Staff);

        var barberStaff2 = new BusinessMember(
            barberShop.Id,
            staff2.Id,
            BusinessRole.Staff);

        var fitnessOwner = new BusinessMember(
            fitness.Id,
            secondOwner.Id,
            BusinessRole.Owner);

        var fitnessAdmin = new BusinessMember(
            fitness.Id,
            secondAdmin.Id,
            BusinessRole.Admin);

        var fitnessStaff = new BusinessMember(
            fitness.Id,
            secondStaff.Id,
            BusinessRole.Staff);

        var fitnessStaff1 = new BusinessMember(
            fitness.Id,
            staff1.Id,
            BusinessRole.Staff);

        context.BusinessMembers.AddRange(
            barberOwner,
            barberAdmin,
            barberStaff1,
            barberStaff2,
            fitnessOwner,
            fitnessAdmin,
            fitnessStaff,
            fitnessStaff1);


        // SERVICES
        var barberHaircut = new Service(
            barberShop.Id,
            "Classic Haircut",
            30,
            8000m,
            "Classic men's haircut.");

        var beardTrim = new Service(
            barberShop.Id,
            "Beard Trim",
            20,
            5000m,
            "Beard trimming and shaping.");

        var haircutAndBeard = new Service(
            barberShop.Id,
            "Haircut & Beard",
            50,
            12000m,
            "Classic haircut combined with beard trimming.");

        var personalTraining = new Service(
            fitness.Id,
            "Personal Training",
            60,
            15000m,
            "One-on-one personal training session.");

        var fitnessAssessment = new Service(
            fitness.Id,
            "Fitness Assessment",
            45,
            10000m,
            "Initial fitness and physical condition assessment.");

        var nutritionConsultation = new Service(
            fitness.Id,
            "Nutrition Consultation",
            30,
            9000m,
            "Individual nutrition consultation.");

        context.Services.AddRange(
            barberHaircut,
            beardTrim,
            haircutAndBeard,
            personalTraining,
            fitnessAssessment,
            nutritionConsultation);

        barberHaircut.Members.Add(barberStaff1);
        barberHaircut.Members.Add(barberStaff2);

        beardTrim.Members.Add(barberStaff1);
        beardTrim.Members.Add(barberStaff2);

        haircutAndBeard.Members.Add(barberStaff1);

        personalTraining.Members.Add(fitnessStaff);

        fitnessAssessment.Members.Add(fitnessStaff);

        nutritionConsultation.Members.Add(fitnessStaff);

        // AVAILABILITY

        // Steve: monday to friday, 09:00 - 17:00
        var barberStaff1Monday = new Availability(
            barberStaff1.Id,
            AvailabilityType.Weekly,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            true,
            DayOfWeek.Monday);

        var barberStaff1Tuesday = new Availability(
            barberStaff1.Id,
            AvailabilityType.Weekly,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            true,
            DayOfWeek.Tuesday);

        var barberStaff1Wednesday = new Availability(
            barberStaff1.Id,
            AvailabilityType.Weekly,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            true,
            DayOfWeek.Wednesday);

        var barberStaff1Thursday = new Availability(
            barberStaff1.Id,
            AvailabilityType.Weekly,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            true,
            DayOfWeek.Thursday);

        var barberStaff1Friday = new Availability(
            barberStaff1.Id,
            AvailabilityType.Weekly,
            new TimeOnly(9, 0),
            new TimeOnly(17, 0),
            true,
            DayOfWeek.Friday);

        // Sarah tuesday to saturday, 12:00 - 20:00
        var barberStaff2Tuesday = new Availability(
            barberStaff2.Id,
            AvailabilityType.Weekly,
            new TimeOnly(12, 0),
            new TimeOnly(20, 0),
            true,
            DayOfWeek.Tuesday);

        var barberStaff2Wednesday = new Availability(
            barberStaff2.Id,
            AvailabilityType.Weekly,
            new TimeOnly(12, 0),
            new TimeOnly(20, 0),
            true,
            DayOfWeek.Wednesday);

        var barberStaff2Thursday = new Availability(
            barberStaff2.Id,
            AvailabilityType.Weekly,
            new TimeOnly(12, 0),
            new TimeOnly(20, 0),
            true,
            DayOfWeek.Thursday);

        var barberStaff2Friday = new Availability(
            barberStaff2.Id,
            AvailabilityType.Weekly,
            new TimeOnly(12, 0),
            new TimeOnly(20, 0),
            true,
            DayOfWeek.Friday);

        var barberStaff2Saturday = new Availability(
            barberStaff2.Id,
            AvailabilityType.Weekly,
            new TimeOnly(10, 0),
            new TimeOnly(16, 0),
            true,
            DayOfWeek.Saturday);

        // Sophie monday to friday, 08:00 - 16:00
        var fitnessStaffMonday = new Availability(
            fitnessStaff.Id,
            AvailabilityType.Weekly,
            new TimeOnly(8, 0),
            new TimeOnly(16, 0),
            true,
            DayOfWeek.Monday);

        var fitnessStaffTuesday = new Availability(
            fitnessStaff.Id,
            AvailabilityType.Weekly,
            new TimeOnly(8, 0),
            new TimeOnly(16, 0),
            true,
            DayOfWeek.Tuesday);

        var fitnessStaffWednesday = new Availability(
            fitnessStaff.Id,
            AvailabilityType.Weekly,
            new TimeOnly(8, 0),
            new TimeOnly(16, 0),
            true,
            DayOfWeek.Wednesday);

        var fitnessStaffThursday = new Availability(
            fitnessStaff.Id,
            AvailabilityType.Weekly,
            new TimeOnly(8, 0),
            new TimeOnly(16, 0),
            true,
            DayOfWeek.Thursday);

        var fitnessStaffFriday = new Availability(
            fitnessStaff.Id,
            AvailabilityType.Weekly,
            new TimeOnly(8, 0),
            new TimeOnly(16, 0),
            true,
            DayOfWeek.Friday);

        var exceptionDate = DateOnly.FromDateTime(DateTime.UtcNow);

        while (exceptionDate.DayOfWeek != DayOfWeek.Wednesday)
        {
            exceptionDate = exceptionDate.AddDays(1);
        }

        // Steve date exception
        var barberStaff1Exception = new Availability(
            barberStaff1.Id,
            AvailabilityType.Exception,
            new TimeOnly(0, 0),
            new TimeOnly(23, 59),
            false,
            specificDate: exceptionDate);

        // Sarah date exception (more hours)
        var barberStaff2Exception = new Availability(
            barberStaff2.Id,
            AvailabilityType.Exception,
            new TimeOnly(9, 0),
            new TimeOnly(18, 0),
            true,
            specificDate: exceptionDate);

        context.Availabilities.AddRange(
            barberStaff1Monday,
            barberStaff1Tuesday,
            barberStaff1Wednesday,
            barberStaff1Thursday,
            barberStaff1Friday,

            barberStaff2Tuesday,
            barberStaff2Wednesday,
            barberStaff2Thursday,
            barberStaff2Friday,
            barberStaff2Saturday,

            fitnessStaffMonday,
            fitnessStaffTuesday,
            fitnessStaffWednesday,
            fitnessStaffThursday,
            fitnessStaffFriday,

            barberStaff1Exception,
            barberStaff2Exception);

        // CUSTOMER

        var registeredCustomer = new Customer(
            barberShop.Id,
            "Carlos",
            "Customer",
            "customer@yuggenda.dev",
            "+54 11 5555-3000",
            customerUser.Id);

        var registeredFitnessCustomer = new Customer(
            fitness.Id,
            "Daniel",
            "Fitness",
            "daniel@yuggenda.dev",
            "+54 11 5555-4000",
            customerUser.Id);

        var guestBarberCustomer = new Customer(
            barberShop.Id,
            "Laura",
            "Guest",
            "laura.guest@example.com",
            "+54 11 5555-5000");

        var guestFitnessCustomer = new Customer(
            fitness.Id,
            "Martin",
            "Guest",
            "martin.guest@example.com",
            "+54 11 5555-6000");

        context.Customers.AddRange(
            registeredCustomer,
            registeredFitnessCustomer,
            guestBarberCustomer,
            guestFitnessCustomer);

        // APPOINTMENTS

        var nextMonday = DateTime.UtcNow.Date;

        while (nextMonday.DayOfWeek != DayOfWeek.Monday)
        {
            nextMonday = nextMonday.AddDays(1);
        }

        var nextTuesday = nextMonday.AddDays(1);

        var barberAppointment1 = new Appointment(
            barberShop.Id,
            registeredCustomer.Id,
            barberHaircut.Id,
            barberStaff1.Id,
            nextMonday.AddHours(12),
            nextMonday.AddHours(12).AddMinutes(30),
            "Regular haircut.");

        var barberAppointment2 = new Appointment(
            barberShop.Id,
            guestBarberCustomer.Id,
            beardTrim.Id,
            barberStaff2.Id,
            nextTuesday.AddHours(14),
            nextTuesday.AddHours(14).AddMinutes(20),
            "Guest customer appointment.");

        var fitnessAppointment = new Appointment(
            fitness.Id,
            registeredFitnessCustomer.Id,
            personalTraining.Id,
            fitnessStaff.Id,
            nextMonday.AddHours(10),
            nextMonday.AddHours(11),
            "First personal training session.");

        var guestFitnessAppointment = new Appointment(
            fitness.Id,
            guestFitnessCustomer.Id,
            fitnessAssessment.Id,
            fitnessStaff.Id,
            nextMonday.AddHours(12),
            nextMonday.AddHours(12).AddMinutes(45));

        context.Appointments.AddRange(
            barberAppointment1,
            barberAppointment2,
            fitnessAppointment,
            guestFitnessAppointment);

        await context.SaveChangesAsync();
    }

    private const string SeedPasswordHash =
        "$2a$11$ZHzkEV4L/ZP4IkKr.zwLAecgiJQQJLWYSlkc6m54fR4e7LytrN9Vq";
}