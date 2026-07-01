# 📚 Library Management System

A desktop application built as part of an internship evaluation project, demonstrating Object-Oriented Programming principles using C#, WPF, Entity Framework Core, and the MVVM architectural pattern.

---

## 🎯 Project Overview

This Library Management System allows library staff to manage books, members, and borrowing transactions through a clean and intuitive desktop interface.

---

## ✨ Features

- Book Management — Add, update, delete, and search books by title, author, ISBN, or genre
- Member Management — Register and manage library members with contact information
- Borrowing System — Borrow and return books with automatic availability tracking
- Business Rules — Prevents borrowing unavailable books and deleting members with active loans
- Real-time Search — Instant filtering as you type
- Overdue Tracking — Highlights overdue transactions automatically

---

## 🏗️ Architecture

This project follows the MVVM (Model-View-ViewModel)** pattern with a layered architecture:

├── Models/          → Data definitions (Book, Member, BorrowTransaction)
├── Data/            → Entity Framework DbContext and database configuration
├── Repositories/    → Data access layer with LINQ queries
├── Services/        → Business logic and validation rules
├── ViewModels/      → Connects UI to data (INotifyPropertyChanged, ICommand)
├── Views/           → WPF XAML user interface screens
└── Helpers/         → RelayCommand implementations for data binding

---

## 🎓 OOP Concepts Demonstrated

| Concept | Implementation |
|---|---|
| **Abstraction** | `BaseEntity` abstract class with abstract `GetDisplayName()` method |
| **Inheritance** | `Book`, `Member`, `BorrowTransaction` all inherit from `BaseEntity` |
| **Encapsulation** | Private backing fields with controlled public properties in all models |
| **Polymorphism** | `IBookService`, `IMemberService`, `ITransactionService` interfaces |
| **SOLID Principles** | Single Responsibility, Dependency Inversion via DI container |

---

## 🗄️ Database Design

Three tables in SQL Server managed by Entity Framework Core:

| Table | Columns |
|---|---|
| `Books` | Id, Title, Author, ISBN, Genre, IsAvailable, CreatedAt, UpdatedAt |
| `Members` | Id, Name, ContactInfo, MembershipDate, CreatedAt, UpdatedAt |
| `BorrowTransactions` | Id, BookId, MemberId, BorrowDate, DueDate, ReturnDate |

---

## 🛠️ Tech Stack

- Language: C# (.NET 8)
- UI Framework: WPF (Windows Presentation Foundation)
- Database:SQL Server / LocalDB
- ORM: Entity Framework Core 8
- Architecture:MVVM Pattern
- Logging:Serilog
- Dependency Injection:Microsoft.Extensions.DependencyInjection

---


## 👩‍💻 Developer

Built by **Tilu** as part of an internship training evaluation.

- Concepts covered: OOP, SOLID, MVVM, Entity Framework, LINQ, Dependency Injection, Error Handling, Logging