using LibraryApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Data;

public static class DemoDataSeeder
{
    private const string EmployeePassword = "Employee@123456";
    private const string DemoPenaltyReason = "Retard de restitution - jeu de données de démonstration";

    private static readonly string[] CategoryNames =
    [
        "Informatique", "Comptabilité", "Gestion", "Finance", "Marketing",
        "Littérature", "Développement personnel", "Sciences"
    ];

    private static readonly AuthorSeed[] AuthorSeeds =
    [
        new("Robert C.", "Martin"), new("Martin", "Fowler"),
        new("Andrew", "Hunt"), new("David", "Thomas"),
        new("Cal", "Newport"), new("Stephen R.", "Covey"),
        new("Peter F.", "Drucker"), new("Yuval Noah", "Harari"),
        new("Mary", "Shelley"), new("Jules", "Verne")
    ];

    private static readonly BookSeed[] BookSeeds =
    [
        new(1, "Clean Code", 0, "Informatique", 2008, 5),
        new(2, "The Clean Coder", 0, "Informatique", 2011, 4),
        new(3, "Refactoring", 1, "Informatique", 2018, 4),
        new(4, "Patterns of Enterprise Application Architecture", 1, "Informatique", 2002, 3),
        new(5, "The Pragmatic Programmer", 2, "Informatique", 2019, 5),
        new(6, "Programming Ruby", 2, "Informatique", 2004, 3),
        new(7, "The Software Craftsman", 3, "Informatique", 2014, 4),
        new(8, "Your Code as a Crime Scene", 3, "Informatique", 2015, 3),
        new(9, "Deep Work", 4, "Développement personnel", 2016, 5),
        new(10, "Digital Minimalism", 4, "Développement personnel", 2019, 4),
        new(11, "The 7 Habits of Highly Effective People", 5, "Gestion", 1989, 5),
        new(12, "First Things First", 5, "Gestion", 1994, 3),
        new(13, "The Effective Executive", 6, "Gestion", 1967, 4),
        new(14, "Innovation and Entrepreneurship", 6, "Finance", 1985, 3),
        new(15, "Sapiens", 7, "Sciences", 2011, 5),
        new(16, "Homo Deus", 7, "Sciences", 2015, 4),
        new(17, "Frankenstein", 8, "Littérature", 1818, 4),
        new(18, "The Last Man", 8, "Littérature", 1826, 3),
        new(19, "Twenty Thousand Leagues Under the Seas", 9, "Littérature", 1870, 5),
        new(20, "Around the World in Eighty Days", 9, "Littérature", 1872, 4)
    ];

    private static readonly StudentSeed[] StudentSeeds =
    [
        new("student01@demo.library.test", "Amira Ben Salem"),
        new("student02@demo.library.test", "Youssef Trabelsi"),
        new("student03@demo.library.test", "Mariam Gharbi"),
        new("student04@demo.library.test", "Sami Ben Amor"),
        new("student05@demo.library.test", "Ines Jaziri"),
        new("student06@demo.library.test", "Omar Khelifi"),
        new("student07@demo.library.test", "Nour Bouazizi"),
        new("student08@demo.library.test", "Fares Mansour"),
        new("student09@demo.library.test", "Rania Chaabane"),
        new("student10@demo.library.test", "Ahmed Mejri"),
        new("student11@demo.library.test", "Lina Saidi"),
        new("student12@demo.library.test", "Malek Dridi"),
        new("student13@demo.library.test", "Yasmine Hadded"),
        new("student14@demo.library.test", "Sofiene Ayari"),
        new("student15@demo.library.test", "Dorra Ben Yedder")
    ];

    private static readonly EmployeeSeed[] EmployeeSeeds =
    [
        new("demo.employee1@library.test", "Sarra Mansouri", "Responsable des prêts", ["Books.View", "Students.View", "Loans.View", "Loans.Create", "Loans.Return"]),
        new("demo.employee2@library.test", "Nabil Kacem", "Gestionnaire du catalogue", ["Books.View", "Books.Create", "Books.Edit", "Books.Delete", "Authors.View", "Authors.Create", "Authors.Edit", "Authors.Delete", "Categories.View", "Categories.Create", "Categories.Edit", "Categories.Delete"]),
        new("demo.employee3@library.test", "Leila Hamdi", "Responsable des rapports", ["Books.View", "Students.View", "Students.Create", "Students.Edit", "Students.Delete", "Employees.View", "Loans.View", "Reports.View"])
    ];

    private static readonly PlanSeed[] PlanSeeds =
    [
        new("Étudiant mensuel", "Accès à la bibliothèque pour un mois.", 30, 12m, 2),
        new("Étudiant semestriel", "Accès à la bibliothèque pour un semestre.", 180, 55m, 5),
        new("Étudiant annuel", "Accès à la bibliothèque pour une année.", 365, 95m, 8)
    ];

    private static readonly SubscriptionSeed[] SubscriptionSeeds =
    [
        new(1, "Étudiant annuel", -100, true, "Paid", "Carte bancaire"),
        new(2, "Étudiant semestriel", -210, false, "Paid", "Virement"),
        new(3, "Étudiant mensuel", -8, true, "Paid", "Espèces"),
        new(4, "Étudiant annuel", -40, true, "Paid", "Carte bancaire"),
        new(5, "Étudiant mensuel", -90, false, "Paid", "Espèces"),
        new(6, "Étudiant semestriel", 3, false, "Pending", "Virement"),
        new(7, "Étudiant annuel", -25, true, "Paid", "Carte bancaire"),
        new(8, "Étudiant mensuel", -50, false, "Failed", "Carte bancaire"),
        new(9, "Étudiant semestriel", -40, true, "Paid", "Virement"),
        new(10, "Étudiant annuel", -300, true, "Paid", "Carte bancaire"),
        new(11, "Étudiant mensuel", 5, false, "Pending", "Espèces"),
        new(12, "Étudiant semestriel", -190, false, "Paid", "Virement")
    ];

    private static readonly LoanSeed[] LoanSeeds =
    [
        new(1, 1, 350, 12, null, false),
        new(2, 2, 300, 20, 8m, true),
        new(3, 3, 260, 10, null, false),
        new(4, 4, 220, 18, 4m, true),
        new(5, 5, 180, 17, 3m, false),
        new(6, 6, 150, 12, null, false),
        new(7, 7, 120, 13, null, false),
        new(8, 8, 90, 9, null, false),
        new(9, 9, 60, 13, null, false),
        new(10, 10, 40, 10, null, false),
        new(11, 11, 30, 17, 2m, true),
        new(12, 12, 22, null, 5.5m, false),
        new(13, 13, 18, null, 2m, false),
        new(14, 14, 8, null, null, false),
        new(15, 15, 3, null, null, false)
    ];

    public static async Task<DemoDataSummary> SeedAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await using var transaction = await db.Database.BeginTransactionAsync();

        var admin = await userManager.FindByEmailAsync("admin@library.com");
        if (admin is null || !await userManager.IsInRoleAsync(admin, "Admin"))
            throw new InvalidOperationException("Le compte Admin attendu par AdminSeeder est absent ou n'a pas le rôle Admin.");

        await EnsureRoleAsync(roleManager, "Employee");

        var categories = await SeedCategoriesAsync(db);
        var authors = await SeedAuthorsAsync(db);
        var books = await SeedBooksAsync(db, categories, authors);
        var students = await SeedStudentsAsync(db);
        var employees = await SeedEmployeesAsync(db, userManager);
        await SeedPermissionsAsync(db, employees);
        var plans = await SeedPlansAsync(db);
        var subscriptions = await SeedSubscriptionsAsync(db, students, plans);
        await SeedPaymentsAsync(db, subscriptions);
        var loans = await SeedLoansAsync(db, books, students);
        await SeedPenaltiesAsync(db, loans);

        await db.SaveChangesAsync();
        await transaction.CommitAsync();

        var demoIsbns = BookSeeds.Select(seed => CreateIsbn13(seed.Number)).ToArray();
        var demoStudentEmails = StudentSeeds.Select(seed => seed.Email).ToArray();
        var demoEmployeeEmails = EmployeeSeeds.Select(seed => seed.Email).ToArray();
        var demoPlanNames = PlanSeeds.Select(seed => seed.Name).ToArray();
        var demoStudentIds = await db.Students
            .Where(student => demoStudentEmails.Contains(student.Email))
            .Select(student => student.Id)
            .ToArrayAsync();
        var demoPlanIds = await db.SubscriptionPlans
            .Where(plan => demoPlanNames.Contains(plan.Name))
            .Select(plan => plan.Id)
            .ToArrayAsync();
        var demoSubscriptionIds = await db.Subscriptions
            .Where(subscription => demoStudentIds.Contains(subscription.StudentId) && demoPlanIds.Contains(subscription.SubscriptionPlanId))
            .Select(subscription => subscription.Id)
            .ToArrayAsync();
        var demoBookIds = await db.Books
            .Where(book => demoIsbns.Contains(book.ISBN))
            .Select(book => book.Id)
            .ToArrayAsync();
        var existingAuthors = await db.Authors.AsNoTracking().ToListAsync();
        var demoAuthorCount = existingAuthors.Count(author =>
            AuthorSeeds.Any(seed => seed.FirstName == author.FirstName && seed.LastName == author.LastName));
        var demoLoans = await db.Loans
            .Where(loan => demoStudentIds.Contains(loan.StudentId) && demoBookIds.Contains(loan.BookId))
            .ToListAsync();
        var demoSubscriptions = await db.Subscriptions
            .Where(subscription => demoSubscriptionIds.Contains(subscription.Id))
            .ToListAsync();
        var demoPenalties = await db.Penalties
            .Where(penalty => penalty.Reason == DemoPenaltyReason)
            .ToListAsync();
        var employeeUserIds = employees.Values.Select(user => user.Id).ToArray();
        var now = DateTime.UtcNow;

        return new DemoDataSummary(
            await db.Categories.CountAsync(category => CategoryNames.Contains(category.Name)),
            demoAuthorCount,
            await db.Books.CountAsync(book => demoIsbns.Contains(book.ISBN)),
            await db.Students.CountAsync(student => demoStudentEmails.Contains(student.Email)),
            await db.Employees.CountAsync(employee => demoEmployeeEmails.Contains(employee.Email)),
            await db.Users.CountAsync(user => demoEmployeeEmails.Contains(user.Email!)),
            demoSubscriptionIds.Length,
            await db.SubscriptionPayments.CountAsync(payment => demoSubscriptionIds.Contains(payment.SubscriptionId)),
            await db.Loans.CountAsync(loan => demoStudentIds.Contains(loan.StudentId) && demoBookIds.Contains(loan.BookId)),
            demoPenalties.Count,
            demoLoans.Count(loan => loan.ReturnDate is null),
            demoLoans.Count(loan => loan.ReturnDate is null && loan.DueDate < now),
            demoLoans.Count(loan => loan.ReturnDate is not null),
            demoSubscriptions.Count(subscription => subscription.IsActive && subscription.StartDate <= now && subscription.EndDate >= now),
            demoPenalties.Count(penalty => penalty.IsPaid && penalty.PaidAt is not null),
            demoPenalties.Count(penalty => !penalty.IsPaid && penalty.PaidAt is null),
            await db.UserPermissions.CountAsync(assignment => employeeUserIds.Contains(assignment.UserId)));
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName)) return;

        var result = await roleManager.CreateAsync(new IdentityRole(roleName));
        EnsureIdentitySuccess(result, $"création du rôle {roleName}");
    }

    private static async Task<Dictionary<string, Category>> SeedCategoriesAsync(ApplicationDbContext db)
    {
        var result = new Dictionary<string, Category>();
        foreach (var name in CategoryNames)
        {
            var category = await db.Categories
                .OrderBy(item => item.Id)
                .FirstOrDefaultAsync(item => item.Name == name);
            if (category is null)
            {
                category = new Category { Name = name };
                db.Categories.Add(category);
            }
            result[name] = category;
        }
        await db.SaveChangesAsync();
        return result;
    }

    private static async Task<Dictionary<string, Author>> SeedAuthorsAsync(ApplicationDbContext db)
    {
        var result = new Dictionary<string, Author>();
        foreach (var seed in AuthorSeeds)
        {
            var author = await db.Authors
                .OrderBy(item => item.Id)
                .FirstOrDefaultAsync(item => item.FirstName == seed.FirstName && item.LastName == seed.LastName);
            if (author is null)
            {
                author = new Author { FirstName = seed.FirstName, LastName = seed.LastName };
                db.Authors.Add(author);
            }
            result[AuthorKey(seed.FirstName, seed.LastName)] = author;
        }
        await db.SaveChangesAsync();
        return result;
    }

    private static async Task<Dictionary<int, Book>> SeedBooksAsync(
        ApplicationDbContext db,
        IReadOnlyDictionary<string, Category> categories,
        IReadOnlyDictionary<string, Author> authors)
    {
        var result = new Dictionary<int, Book>();
        foreach (var seed in BookSeeds)
        {
            var isbn = CreateIsbn13(seed.Number);
            var book = await db.Books
                .OrderBy(item => item.Id)
                .FirstOrDefaultAsync(item => item.ISBN == isbn);
            if (book is null)
            {
                var author = AuthorSeeds[seed.AuthorIndex];
                book = new Book
                {
                    Title = seed.Title,
                    ISBN = isbn,
                    Price = 25m + seed.Number * 2m,
                    Description = $"Ouvrage de {seed.CategoryName.ToLowerInvariant()} publié en {seed.PublicationYear}, signé {author.FirstName} {author.LastName}.",
                    PublicationYear = seed.PublicationYear,
                    AvailableCopies = seed.AvailableCopies,
                    CategoryId = categories[seed.CategoryName].Id,
                    AuthorId = authors[AuthorKey(author.FirstName, author.LastName)].Id
                };
                db.Books.Add(book);
            }
            else if (book.Price is null)
            {
                var author = AuthorSeeds[seed.AuthorIndex];
                book.Price = 25m + seed.Number * 2m;
                book.Description = $"Ouvrage de {seed.CategoryName.ToLowerInvariant()} publié en {seed.PublicationYear}, signé {author.FirstName} {author.LastName}.";
            }
            result[seed.Number] = book;
        }
        await db.SaveChangesAsync();
        return result;
    }

    private static async Task<Dictionary<string, Student>> SeedStudentsAsync(ApplicationDbContext db)
    {
        var result = new Dictionary<string, Student>(StringComparer.OrdinalIgnoreCase);
        foreach (var seed in StudentSeeds)
        {
            var student = await db.Students
                .OrderBy(item => item.Id)
                .FirstOrDefaultAsync(item => item.Email == seed.Email);
            if (student is null)
            {
                student = new Student { FullName = seed.FullName, Email = seed.Email };
                db.Students.Add(student);
            }
            result[seed.Email] = student;
        }
        await db.SaveChangesAsync();
        return result;
    }

    private static async Task<Dictionary<string, ApplicationUser>> SeedEmployeesAsync(
        ApplicationDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        var result = new Dictionary<string, ApplicationUser>(StringComparer.OrdinalIgnoreCase);
        foreach (var seed in EmployeeSeeds)
        {
            var employee = await db.Employees
                .OrderBy(item => item.Id)
                .FirstOrDefaultAsync(item => item.Email == seed.Email);
            if (employee is null)
            {
                employee = new Employee
                {
                    FullName = seed.FullName,
                    Email = seed.Email,
                    JobTitle = seed.JobTitle,
                    IsActive = true
                };
                db.Employees.Add(employee);
                await db.SaveChangesAsync();
            }

            var user = string.IsNullOrWhiteSpace(employee.ApplicationUserId)
                ? await userManager.FindByEmailAsync(employee.Email)
                : await userManager.FindByIdAsync(employee.ApplicationUserId);

            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = employee.Email,
                    Email = employee.Email,
                    FullName = employee.FullName,
                    EmailConfirmed = true
                };
                var createResult = await userManager.CreateAsync(user, EmployeePassword);
                EnsureIdentitySuccess(createResult, $"création du compte {employee.Email}");
            }

            var linkedToAnotherEmployee = await db.Employees
                .AnyAsync(item => item.Id != employee.Id && item.ApplicationUserId == user.Id);
            if (linkedToAnotherEmployee)
                throw new InvalidOperationException($"Le compte {employee.Email} est déjà lié à une autre fiche employé.");

            if (!await userManager.IsInRoleAsync(user, "Employee"))
            {
                var roleResult = await userManager.AddToRoleAsync(user, "Employee");
                EnsureIdentitySuccess(roleResult, $"attribution du rôle Employee à {employee.Email}");
            }

            if (!string.Equals(employee.ApplicationUserId, user.Id, StringComparison.Ordinal))
            {
                employee.ApplicationUserId = user.Id;
                await db.SaveChangesAsync();
            }

            result[seed.Email] = user;
        }
        return result;
    }

    private static async Task SeedPermissionsAsync(
        ApplicationDbContext db,
        IReadOnlyDictionary<string, ApplicationUser> employees)
    {
        var requiredNames = EmployeeSeeds.SelectMany(seed => seed.Permissions).Distinct().ToArray();
        var permissions = await db.Permissions
            .Where(permission => requiredNames.Contains(permission.Name))
            .ToDictionaryAsync(permission => permission.Name);

        var missing = requiredNames.Where(name => !permissions.ContainsKey(name)).ToArray();
        if (missing.Length > 0)
            throw new InvalidOperationException($"Permissions non initialisées : {string.Join(", ", missing)}.");

        foreach (var seed in EmployeeSeeds)
        {
            var user = employees[seed.Email];
            foreach (var name in seed.Permissions)
            {
                var permission = permissions[name];
                var exists = await db.UserPermissions.AnyAsync(assignment =>
                    assignment.UserId == user.Id && assignment.PermissionId == permission.Id);
                if (!exists)
                    db.UserPermissions.Add(new UserPermission { UserId = user.Id, PermissionId = permission.Id });
            }
        }
        await db.SaveChangesAsync();
    }

    private static async Task<Dictionary<string, SubscriptionPlan>> SeedPlansAsync(ApplicationDbContext db)
    {
        var result = new Dictionary<string, SubscriptionPlan>(StringComparer.OrdinalIgnoreCase);
        foreach (var seed in PlanSeeds)
        {
            var plan = await db.SubscriptionPlans
                .FirstOrDefaultAsync(item => item.Name == seed.Name);
            if (plan is null)
            {
                plan = new SubscriptionPlan
                {
                    Name = seed.Name,
                    Description = seed.Description,
                    DurationDays = seed.DurationDays,
                    Price = seed.Price,
                    MaxLoans = seed.MaxLoans
                };
                db.SubscriptionPlans.Add(plan);
            }
            result[seed.Name] = plan;
        }
        await db.SaveChangesAsync();
        return result;
    }

    private static async Task<Dictionary<int, Subscription>> SeedSubscriptionsAsync(
        ApplicationDbContext db,
        IReadOnlyDictionary<string, Student> students,
        IReadOnlyDictionary<string, SubscriptionPlan> plans)
    {
        var today = DateTime.UtcNow.Date;
        var result = new Dictionary<int, Subscription>();
        foreach (var seed in SubscriptionSeeds)
        {
            var studentSeed = StudentSeeds[seed.StudentIndex - 1];
            var student = students[studentSeed.Email];
            var plan = plans[seed.PlanName];
            var subscription = await db.Subscriptions
                .OrderBy(item => item.Id)
                .FirstOrDefaultAsync(item => item.StudentId == student.Id && item.SubscriptionPlanId == plan.Id);
            if (subscription is null)
            {
                var startDate = today.AddDays(seed.StartOffsetDays);
                subscription = new Subscription
                {
                    StudentId = student.Id,
                    SubscriptionPlanId = plan.Id,
                    StartDate = startDate,
                    EndDate = startDate.AddDays(plan.DurationDays),
                    IsActive = seed.IsActive
                };
                db.Subscriptions.Add(subscription);
            }
            result[seed.StudentIndex] = subscription;
        }
        await db.SaveChangesAsync();
        return result;
    }

    private static async Task SeedPaymentsAsync(
        ApplicationDbContext db,
        IReadOnlyDictionary<int, Subscription> subscriptions)
    {
        for (var index = 0; index < SubscriptionSeeds.Length; index++)
        {
            var seed = SubscriptionSeeds[index];
            var subscription = subscriptions[seed.StudentIndex];
            var plan = await db.SubscriptionPlans.FindAsync(subscription.SubscriptionPlanId)
                ?? throw new InvalidOperationException("Formule associée à un abonnement introuvable.");
            var paymentDate = subscription.StartDate.AddDays(-1);
            var exists = await db.SubscriptionPayments.AnyAsync(payment =>
                payment.SubscriptionId == subscription.Id &&
                payment.PaymentDate == paymentDate &&
                payment.PaymentMethod == seed.PaymentMethod);
            if (!exists)
            {
                db.SubscriptionPayments.Add(new SubscriptionPayment
                {
                    SubscriptionId = subscription.Id,
                    Amount = plan.Price,
                    PaymentDate = paymentDate,
                    PaymentMethod = seed.PaymentMethod,
                    Status = seed.PaymentStatus
                });
            }
        }
        await db.SaveChangesAsync();
    }

    private static async Task<Dictionary<int, Loan>> SeedLoansAsync(
        ApplicationDbContext db,
        IReadOnlyDictionary<int, Book> books,
        IReadOnlyDictionary<string, Student> students)
    {
        var today = DateTime.UtcNow.Date;
        var result = new Dictionary<int, Loan>();
        foreach (var seed in LoanSeeds)
        {
            var studentSeed = StudentSeeds[seed.StudentIndex - 1];
            var student = students[studentSeed.Email];
            var book = books[seed.BookNumber];
            var loan = await db.Loans
                .OrderBy(item => item.Id)
                .FirstOrDefaultAsync(item => item.StudentId == student.Id && item.BookId == book.Id);
            if (loan is null)
            {
                if (seed.ReturnDaysAfterBorrow is null && book.AvailableCopies <= 0)
                    continue;

                var borrowDate = today.AddDays(-seed.BorrowDaysAgo).AddHours(10);
                loan = new Loan
                {
                    StudentId = student.Id,
                    BookId = book.Id,
                    BorrowDate = borrowDate,
                    DueDate = borrowDate.AddDays(14),
                    ReturnDate = seed.ReturnDaysAfterBorrow is int daysAfterBorrow
                        ? borrowDate.AddDays(daysAfterBorrow)
                        : null
                };
                db.Loans.Add(loan);
                if (loan.ReturnDate is null)
                    book.AvailableCopies--;
            }
            result[seed.Number] = loan;
        }
        await db.SaveChangesAsync();
        return result;
    }

    private static async Task SeedPenaltiesAsync(ApplicationDbContext db, IReadOnlyDictionary<int, Loan> loans)
    {
        foreach (var seed in LoanSeeds.Where(seed => seed.PenaltyAmount.HasValue))
        {
            if (!loans.TryGetValue(seed.Number, out var loan)) continue;
            var late = loan.ReturnDate is DateTime returnDate
                ? returnDate > loan.DueDate
                : loan.DueDate < DateTime.UtcNow;
            if (!late) continue;

            var exists = await db.Penalties.AnyAsync(penalty =>
                penalty.LoanId == loan.Id && penalty.Reason == DemoPenaltyReason);
            if (exists) continue;

            var isPaid = seed.PenaltyPaid && loan.ReturnDate.HasValue;
            db.Penalties.Add(new Penalty
            {
                LoanId = loan.Id,
                Amount = seed.PenaltyAmount!.Value,
                Reason = DemoPenaltyReason,
                IssuedAt = loan.DueDate.AddDays(1),
                IsPaid = isPaid,
                PaidAt = isPaid ? loan.ReturnDate : null
            });
        }
        await db.SaveChangesAsync();
    }

    private static string CreateIsbn13(int sequence)
    {
        var firstTwelveDigits = $"978000000{sequence:000}";
        var weightedSum = firstTwelveDigits
            .Select((digit, index) => (digit - '0') * (index % 2 == 0 ? 1 : 3))
            .Sum();
        var checkDigit = (10 - weightedSum % 10) % 10;
        return firstTwelveDigits + checkDigit;
    }

    private static string AuthorKey(string firstName, string lastName) => $"{firstName}|{lastName}";

    private static void EnsureIdentitySuccess(IdentityResult result, string operation)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"Échec lors de {operation} : {string.Join(" | ", result.Errors.Select(error => error.Description))}");
    }

    private sealed record AuthorSeed(string FirstName, string LastName);
    private sealed record BookSeed(int Number, string Title, int AuthorIndex, string CategoryName, int PublicationYear, int AvailableCopies);
    private sealed record StudentSeed(string Email, string FullName);
    private sealed record EmployeeSeed(string Email, string FullName, string JobTitle, string[] Permissions);
    private sealed record PlanSeed(string Name, string Description, int DurationDays, decimal Price, int MaxLoans);
    private sealed record SubscriptionSeed(int StudentIndex, string PlanName, int StartOffsetDays, bool IsActive, string PaymentStatus, string PaymentMethod);
    private sealed record LoanSeed(int Number, int BookNumber, int BorrowDaysAgo, int? ReturnDaysAfterBorrow, decimal? PenaltyAmount, bool PenaltyPaid)
    {
        public int StudentIndex => Number;
    }
}

public sealed record DemoDataSummary(
    int Categories,
    int Authors,
    int Books,
    int Students,
    int Employees,
    int EmployeeAccounts,
    int Subscriptions,
    int SubscriptionPayments,
    int Loans,
    int DemoPenalties,
    int OpenLoans,
    int OverdueLoans,
    int ReturnedLoans,
    int ActiveSubscriptions,
    int PaidPenalties,
    int UnpaidPenalties,
    int EmployeePermissions);
