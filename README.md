# BookTheShow 🎬🎭
> A comprehensive movie/event ticket booking platform.

---

## 🎯 Project Overview

**BookTheShow** is a ticket booking platform similar to BookMyShow, here we understand and implement:

- **System Design**: Scalability, high availability, caching, message queues, microservices
- **OOAD**: Class diagrams, sequence diagrams, use case diagrams, domain modeling
- **Design Patterns**: Creational, Structural, and Behavioral patterns in practical scenarios

### Business Domain

Users can:
- Browse movies, events, and shows
- View theaters, screens, and seat layouts
- Book tickets with seat selection
- Make payments and receive confirmations
- View booking history and manage profiles

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [SQL Server 2022](https://www.microsoft.com/sql-server) or [Docker](https://www.docker.com/)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Quick Start

```bash
# Clone the repository
git clone https://github.com/awalekeeran/BookTheShow.git
cd BookTheShow

# Start infrastructure (SQL Server, Redis)
docker-compose up -d

# Backend setup
cd src/Backend/BookTheShow.API
dotnet restore
dotnet ef database update
dotnet run

# Frontend setup (new terminal)
cd src/Frontend/book-the-show-web
npm install
npm run dev
```

---
