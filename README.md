# 📚 Library Management System

A complete C# WPF desktop application built with Entity Framework Core, SQL Server, and the MVVM pattern.
Demonstrates all four OOP pillars and SOLID principles.

---

## 🗂️ Project Structure

```
LibraryManagementSystem/
├── Models/                     ← Data entities (OOP: Inheritance, Encapsulation)
│   ├── BaseEntity.cs           ← Abstract base class (Abstraction)
│   ├── Book.cs
│   ├── Member.cs
│   └── BorrowTransaction.cs
├── Data/
│   └── LibraryDbContext.cs     ← Entity Framework DbContext
├── Repositories/               ← Data access layer (OOP: Polymorphism via Interfaces)
│   ├── IRepository.cs          ← Generic CRUD interface
│   ├── IBookRepository.cs
│   ├── IMemberRepository.cs
│   ├── ITransactionRepository.cs
│   ├── BookRepository.cs
│   ├── MemberRepository.cs
│   └── TransactionRepository.cs
├── Services/                   ← Business logic layer (SOLID: Single Responsibility)
│   ├── IServices.cs            ← Service interfaces
│   ├── BookService.cs
│   └── Services.cs             ← MemberService + TransactionService
├── ViewModels/                 ← MVVM ViewModels (INotifyPropertyChanged + Commands)
│   ├── ViewModelBase.cs
│   ├── BookViewModel.cs
│   ├── MemberViewModel.cs
│   ├── TransactionViewModel.cs
│   └── MainViewModel.cs
├── Views/                      ← WPF XAML UI
│   ├── Styles.xaml
│   ├── MainWindow.xaml / .cs
│   ├── BookView.xaml / .cs
│   ├── MemberView.xaml / .cs
│   └── TransactionView.xaml / .cs
├── Helpers/
│   └── RelayCommand.cs         ← ICommand implementations for MVVM
├── Migrations/                 ← EF Core database migrations
├── App.xaml / App.xaml.cs      ← DI container setup + startup
└── LibraryManagementSystem.csproj
```

---

## ✅ Prerequisites

| Tool | Download |
|------|----------|
| Visual Studio 2022 (Community or higher) | https://visualstudio.microsoft.com/ |
| .NET 8 SDK | https://dotnet.microsoft.com/download |
| SQL Server (any edition) or SQL Server LocalDB | Included with Visual Studio |
| EF Core CLI tools | `dotnet tool install --global dotnet-ef` |

---

## 🚀 Quick Start (Developer Setup)

### Step 1 — Clone / Open the project
```bash
# If using git
git clone <your-repo-url>
cd LibraryManagementSystem
```
Or open `LibraryManagementSystem.csproj` directly in Visual Studio.

### Step 2 — Restore NuGet packages
```bash
dotnet restore
```
Or in Visual Studio: right-click the solution → **Restore NuGet Packages**.

### Step 3 — Set the connection string
Open `App.xaml.cs` and find this line:
```csharp
"Server=(localdb)\\mssqllocaldb;Database=LibraryManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
```
- **LocalDB (default, easiest):** Works out of the box if Visual Studio is installed.
- **SQL Server Express:** Change to `Server=.\\SQLEXPRESS;Database=LibraryManagementDB;Trusted_Connection=True;`
- **Full SQL Server:** Change to `Server=YOUR_SERVER;Database=LibraryManagementDB;Trusted_Connection=True;`

### Step 4 — Run the database migration
```bash
dotnet ef database update
```
This creates the database and seeds it with 3 sample books and 2 members automatically.

### Step 5 — Run the application
```bash
dotnet run
```
Or press **F5** in Visual Studio.

---

## 🛠️ How to Build It Yourself (Step-by-Step)

If you want to rebuild this from scratch, follow these steps exactly.

---

### Phase 1: Project Setup

1. Open Visual Studio → **Create a new project** → search for **WPF Application** → select it (C#, not VB).
2. Name it `LibraryManagementSystem`, target **.NET 8**.
3. Install NuGet packages (Tools → NuGet Package Manager → Package Manager Console):
```powershell
Install-Package Microsoft.EntityFrameworkCore.SqlServer
Install-Package Microsoft.EntityFrameworkCore.Tools
Install-Package Microsoft.EntityFrameworkCore.Design
Install-Package Serilog
Install-Package Serilog.Sinks.File
Install-Package Serilog.Sinks.Console
Install-Package Microsoft.Extensions.DependencyInjection
```
4. Install the EF Core CLI tool globally:
```bash
dotnet tool install --global dotnet-ef
```

---

### Phase 2: Create the Models (OOP — Inheritance + Encapsulation + Abstraction)

**OOP Concept Applied:** Create a `BaseEntity` abstract class first so all models inherit common fields (ID, CreatedAt, UpdatedAt). This is **Inheritance + Abstraction**.

1. Create a `Models/` folder.
2. Create `BaseEntity.cs` — make it `abstract` with an abstract method `GetDisplayName()`.
3. Create `Book.cs` — inherit from `BaseEntity`. Use **private backing fields** for Title/Author/ISBN to enforce validation (this is **Encapsulation**). Override `GetDisplayName()` (this is **Polymorphism**).
4. Create `Member.cs` — same pattern.
5. Create `BorrowTransaction.cs` — include `BookId` and `MemberId` as foreign keys, and nullable `ReturnDate`.

**Key OOP points to understand:**
- `abstract class` = cannot be instantiated directly; forces subclasses to implement `GetDisplayName()`
- `private string _title; public string Title { get => _title; set => _title = value?.Trim(); }` = Encapsulation (controlled access)
- Both `Book` and `Member` override `GetDisplayName()` differently = Polymorphism

---

### Phase 3: Create the DbContext (Entity Framework)

1. Create a `Data/` folder.
2. Create `LibraryDbContext.cs` — inherit from `DbContext`.
3. Add three `DbSet<T>` properties for Books, Members, BorrowTransactions.
4. Override `OnModelCreating()` to:
   - Set table names
   - Define primary keys
   - Add unique index on ISBN
   - Configure relationships (HasOne / WithMany / HasForeignKey)
   - Seed sample data with `HasData()`

**Common mistake:** Forgetting `OnDelete(DeleteBehavior.Restrict)` on foreign keys causes cascade delete errors.

---

### Phase 4: Create Interfaces (OOP — Polymorphism + SOLID)

**OOP Concept Applied:** Define interfaces so you can swap implementations (e.g., replace SQL Server with SQLite for testing). This is **Polymorphism via Interface** and **Dependency Inversion (SOLID-D)**.

1. Create `Repositories/IRepository.cs` — generic interface with `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `ExistsAsync`.
2. Create `IBookRepository.cs`, `IMemberRepository.cs`, `ITransactionRepository.cs` — each extends `IRepository<T>` with type-specific methods.
3. Create `Services/IServices.cs` — same pattern for the service layer.

**Why this matters:** When writing tests, you mock these interfaces. When changing databases, you only change the concrete repository class, not the services or ViewModels.

---

### Phase 5: Implement Repositories (Data Access with LINQ)

1. Create `BookRepository.cs` — implement `IBookRepository`.
2. Each method uses `await _context.Books.Where(...).ToListAsync()` — this is **LINQ**.
3. Wrap all DB calls in `try/catch` and log with `Serilog`.
4. Repeat for `MemberRepository.cs` and `TransactionRepository.cs`.

**Key LINQ patterns to know:**
```csharp
// Filter
.Where(b => b.IsAvailable)
// Search (case-insensitive)
.Where(b => b.Title.ToLower().Contains(term.ToLower()))
// Include related data (JOIN equivalent)
.Include(t => t.Book).ThenInclude(...)
// Ordering
.OrderBy(b => b.Title)
// Async execution
.ToListAsync()
```

---

### Phase 6: Implement Services (Business Logic — SOLID)

1. Create `BookService.cs` — inject `IBookRepository` via constructor (Dependency Injection).
2. Add business rules here, NOT in the repository:
   - "Cannot add a book with a duplicate ISBN"
   - "Cannot delete a book that is currently borrowed"
3. Create `Services.cs` with `MemberService` and `TransactionService`.
4. `TransactionService.BorrowBookAsync()` must:
   - Validate the book exists and `IsAvailable == true`
   - Create the transaction
   - Set `book.IsAvailable = false` and save

**SOLID applied:**
- **S** (Single Responsibility): BookService only handles book business logic
- **D** (Dependency Inversion): Depends on `IBookRepository` interface, not `BookRepository` directly

---

### Phase 7: Create ViewModels (MVVM Pattern)

**MVVM means:** The View (XAML) knows nothing about data. The ViewModel holds all state and commands. The View just binds to the ViewModel.

1. Create `ViewModelBase.cs`:
   - Implement `INotifyPropertyChanged`
   - Create a `SetProperty<T>()` helper that calls `OnPropertyChanged()` automatically

2. Create `BookViewModel.cs`:
   - Add `ObservableCollection<Book> Books` — auto-notifies the DataGrid when items change
   - Add form field properties (`FormTitle`, `FormAuthor`, etc.)
   - Add ICommand properties: `AddCommand`, `UpdateCommand`, `DeleteCommand`, `LoadCommand`
   - Wire commands to async methods using `AsyncRelayCommand`

3. Create `RelayCommand.cs` and `AsyncRelayCommand.cs` — these wrap `Action` into `ICommand` for XAML binding.

**Why ObservableCollection:** Unlike `List<T>`, `ObservableCollection<T>` fires events when items are added/removed, so the DataGrid updates automatically.

---

### Phase 8: Create the Views (WPF XAML)

1. Modify `App.xaml` to reference a `Styles.xaml` resource dictionary.
2. Create `Styles.xaml` — define reusable styles for Button, TextBox, DataGrid, Label.
3. Create `MainWindow.xaml` — use a `TabControl` with three tabs.
4. Create `BookView.xaml` as a `UserControl`:
   - Left side: `TextBox` for search (bound to `SearchTerm`), `DataGrid` bound to `Books`
   - Right side: form fields bound to `FormTitle`, `FormAuthor`, etc., and buttons bound to commands

**Key XAML binding patterns:**
```xml
<!-- Two-way binding (UI ↔ ViewModel) -->
<TextBox Text="{Binding FormTitle, UpdateSourceTrigger=PropertyChanged}"/>

<!-- Command binding -->
<Button Command="{Binding AddCommand}"/>

<!-- Collection binding -->
<DataGrid ItemsSource="{Binding Books}" SelectedItem="{Binding SelectedBook}"/>

<!-- Conditional style using DataTrigger -->
<DataTrigger Binding="{Binding IsAvailable}" Value="True">
    <Setter Property="Text" Value="✅ Yes"/>
</DataTrigger>
```

---

### Phase 9: Wire up Dependency Injection

In `App.xaml.cs`, override `OnStartup()`:
```csharp
var services = new ServiceCollection();
services.AddDbContext<LibraryDbContext>(options => options.UseSqlServer(connectionString));
services.AddScoped<IBookRepository, BookRepository>();    // Interface → Implementation
services.AddScoped<IBookService, BookService>();
services.AddTransient<BookViewModel>();
services.AddTransient<MainWindow>();
var provider = services.BuildServiceProvider();
```
Then resolve `MainWindow` from the container — it gets all dependencies injected automatically.

---

### Phase 10: Create and Run Migrations

```bash
# Create the migration (generates the SQL schema from your models)
dotnet ef migrations add InitialCreate

# Apply to database (creates tables)
dotnet ef database update
```

If you change a model later:
```bash
dotnet ef migrations add AddNewColumn
dotnet ef database update
```

---

## 🎓 OOP Principles — Where to Find Them in the Code

| Principle | File | How it's applied |
|-----------|------|-----------------|
| **Abstraction** | `Models/BaseEntity.cs` | Abstract class with abstract `GetDisplayName()` |
| **Inheritance** | `Models/Book.cs`, `Member.cs`, etc. | All models inherit `BaseEntity` |
| **Encapsulation** | `Models/Book.cs` | Private `_title` field, validated in public `Title` setter |
| **Polymorphism** | `Repositories/IRepository.cs` + all implementations | Same interface, multiple implementations |
| **SOLID-S** | `Services/BookService.cs` | Only handles book logic |
| **SOLID-O** | `Repositories/IRepository.cs` | Add new entity types without modifying existing code |
| **SOLID-I** | `Repositories/IBookRepository.cs` | Separate interfaces per concern |
| **SOLID-D** | `App.xaml.cs` (DI container) | Classes depend on abstractions, not concretions |
| **MVVM** | `ViewModels/` | ViewModels expose data and commands; Views only bind |
| **LINQ** | All Repositories | `Where`, `Include`, `OrderBy`, `ToListAsync` |

---

## 📋 Evaluation Checklist

- [x] CRUD for Books (Add, Read, Update, Delete)
- [x] CRUD for Members
- [x] Borrow / Return transactions
- [x] Book availability constraint enforced
- [x] OOP: Abstraction (BaseEntity), Inheritance (all models), Encapsulation (private fields), Polymorphism (interfaces)
- [x] SOLID principles applied
- [x] MVVM pattern
- [x] Data binding
- [x] Entity Framework with SQL Server
- [x] LINQ queries
- [x] Error handling (try/catch + MessageBox)
- [x] Logging (Serilog → `logs/` folder)
- [x] Dependency Injection

---

## 🔧 Troubleshooting

| Problem | Fix |
|---------|-----|
| `A network-related error occurred` | SQL Server is not running. Start SQL Server service or use LocalDB. |
| `Cannot open database` | Run `dotnet ef database update` |
| `No executable found matching command "dotnet-ef"` | Run `dotnet tool install --global dotnet-ef` |
| Build errors about missing namespaces | Run `dotnet restore` |
| WPF XAML designer not showing | Rebuild the project (Ctrl+Shift+B) |
