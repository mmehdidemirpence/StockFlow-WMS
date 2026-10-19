# 📦 StockFlow WMS - Warehouse Management System

**StockFlow WMS** is a modern Warehouse Management System built with .NET 8 and Alpine.js. It offers robust inventory management, seamless data filtering, automated audit logging, and dynamic UI components while ensuring persistent transaction history even after product deletions.

---

## ✨ Features

* **Product & Inventory Management:** Full CRUD operations for products, companies, and product types with real-time stock tracking.
* **Audit Logging & History Preservation:**
  * Automated background logging for every creation, update, and deletion with timestamps.
  * Preserves historical stock logs (`Stock Out (Deletion)`) and original product names even if the product is deleted from the database (`SetNull` delete behavior).
* **Advanced Search & Filtering:**
  * Instant product search with debounced typing.
  * Filter inventory by Company and Product Type.
  * Real-time search/filter inputs inside Company and Product Type management modals.
* **Smart Autocomplete Selectors:** Modern text-based searchable dropdowns replacing rigid native HTML select inputs.
* **Date-Based Log Cleanup:** Ability to purge historical stock logs older than a specified date.
* **Server-Side Pagination:** Efficient pagination support for both main product tables and audit log history.
* **Excel / CSV Export:** One-click export of filtered inventory list into UTF-8 compatible CSV/Excel files.
* **Internationalization (i18n):** Date pickers and calendar interfaces standardized to `en-US`.

---

## 🛠️ Tech Stack

### Backend
* **.NET 8.0 / ASP.NET Core Web API**
* **Entity Framework Core** (EF Core ORM)
* **SQLite** Database

### Frontend
* **HTML5 & Tailwind CSS** (Responsive UI Design)
* **Alpine.js** (Reactive client-side state management)
* **FontAwesome** (Icon set)

---

## 🚀 Getting Started

Follow these steps to set up and run the project locally:

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation & Execution

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/YOUR_USERNAME/StockFlow-WMS.git](https://github.com/mmehdidemirpence/StockFlow-WMS.git)
   cd StockFlow-WMS