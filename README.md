# BookTheShow 🎬🎭🎫

> A comprehensive movie & event ticket booking platform built for learning **System Design**, **OOAD**, and **Design Patterns** — inspired by **BookMyShow** and the **Ticketmaster** case study.

[![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-blue)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18.x-61DAFB)](https://reactjs.org/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)](https://www.microsoft.com/sql-server)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![YouTube](https://img.shields.io/badge/YouTube-Content-FF0000)](docs/CONTENT-CREATOR-ROADMAP.md)

---

## 🔥 Why This Project?

Remember when **Ticketmaster crashed** during Taylor Swift's Eras Tour presale?
- 14+ million users tried to access simultaneously
- System designed for 1.5 million couldn't handle the load
- Became a case study for designing **high-volume systems**

This project tackles those **exact challenges** while teaching you:
- How to handle **1M+ concurrent users**
- **Distributed locking** to prevent double bookings
- **Virtual waiting rooms** for fair ticket distribution
- **Real-time seat updates** across thousands of users

---

## 📋 Table of Contents

- [Project Overview](#-project-overview)
- [The Ticketmaster Problem](#-the-ticketmaster-problem)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Learning Roadmap](#-learning-roadmap)
- [Features & Scenarios](#-features--scenarios)
- [Design Patterns Used](#-design-patterns-used)
- [System Design Concepts](#-system-design-concepts)
- [Documentation](#-documentation)
- [Getting Started](#-getting-started)
- [Milestones](#-milestones)

---

## 🎯 Project Overview

**BookTheShow** is a ticket booking platform combining:

| Feature | Learning Focus |
|---------|---------------|
| **Movie Booking** (BookMyShow style) | CRUD, Search, Basic Architecture |
| **Event/Concert Booking** (Ticketmaster style) | High-volume, Concurrency, Scaling |

### What You'll Learn

| Category | Topics |
|----------|--------|
| **System Design** | Scalability, Caching, Message Queues, Load Balancing, Sharding |
| **Concurrency** | Distributed Locking, Two-Phase Commit, Optimistic/Pessimistic Locking |
| **Design Patterns** | Factory, Strategy, Observer, State, Decorator, Adapter, Command, Chain of Responsibility |
| **OOAD** | SOLID Principles, DDD, Clean Architecture, Aggregates |
| **Real-Time** | WebSockets, SignalR, Pub/Sub |

---

## 💥 The Ticketmaster Problem

### What Happened (Nov 2022)

```
┌─────────────────────────────────────────────────────────────┐
│                    TAYLOR SWIFT ERAS TOUR                   │
├─────────────────────────────────────────────────────────────┤
│  Expected Users:     1.5 million                            │
│  Actual Users:       14+ million                            │
│  Result:             SYSTEM CRASHED 💥                       │
│  Outcome:            Congressional hearings, lawsuits       │
└─────────────────────────────────────────────────────────────┘
```

### Technical Problems We'll Solve

| Problem | Our Solution |
|---------|-------------|
| **Thundering Herd** | Virtual Waiting Room with Redis Sorted Sets |
| **Double Booking** | Distributed Locking (Redlock algorithm) |
| **Database Bottleneck** | Read Replicas + Redis Caching |
| **No Auto-Scaling** | Container orchestration patterns |
| **Single Point of Failure** | Event-driven architecture |
| **Bot Traffic** | Rate limiting + Token bucket algorithm |

📖 **Deep Dive**: [High-Volume System Design](docs/architecture/high-volume-system-design.md)

---

## 🏗️ Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              CLIENT LAYER                                    │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐              │
│  │   React Web     │  │   Mobile App    │  │   Admin Portal  │              │
│  │   Application   │  │   (Future)      │  │   (React)       │              │
│  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘              │
└───────────┼─────────────────────┼─────────────────────┼─────────────────────┘
            │                     │                     │
            └─────────────────────┼─────────────────────┘
                                  │
┌─────────────────────────────────┼───────────────────────────────────────────┐
│                        WAITING ROOM LAYER (for high-demand events)           │
│  ┌──────────────────────────────┴──────────────────────────────────────┐    │
│  │              Virtual Waiting Room (Redis Sorted Set)                 │    │
│  │         • Queue Position • Fair Distribution • Bot Protection        │    │
│  └──────────────────────────────┬──────────────────────────────────────┘    │
└─────────────────────────────────┼───────────────────────────────────────────┘
                                  │
┌─────────────────────────────────┼───────────────────────────────────────────┐
│                         API GATEWAY LAYER                                    │
│  ┌──────────────────────────────┴──────────────────────────────────────┐    │
│  │                        API Gateway (YARP)                            │    │
│  │    • Rate Limiting • Authentication • Circuit Breaker • Routing      │    │
│  └──────────────────────────────┬──────────────────────────────────────┘    │
└─────────────────────────────────┼───────────────────────────────────────────┘
                                  │
┌─────────────────────────────────┼───────────────────────────────────────────┐
│                           SERVICE LAYER                                      │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │   Identity  │  │   Catalog   │  │   Booking   │  │   Payment   │         │
│  │   Service   │  │   Service   │  │   Service   │  │   Service   │         │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘         │
│         │                │                │                │                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │Notification │  │   Search    │  │  Inventory  │  │  Analytics  │         │
│  │   Service   │  │   Service   │  │   Service   │  │   Service   │         │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘         │
└─────────┼────────────────┼────────────────┼────────────────┼────────────────┘
          │                │                │                │
┌─────────┼────────────────┼────────────────┼────────────────┼────────────────┐
│         │           DATA & MESSAGING LAYER                 │                 │
│  ┌──────┴──────┐  ┌─────────────┐  ┌─────────────┐  ┌──────┴──────┐         │
│  │  SQL Server │  │    Redis    │  │ Elasticsearch│  │  RabbitMQ   │         │
│  │  (Primary)  │  │ (Cache+Lock)│  │  (Search)   │  │  (Events)   │         │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘         │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Evolutionary Architecture Phases

| Phase | Architecture Style | Learning Focus |
|-------|-------------------|----------------|
| **Phase 1** | Monolith | SOLID, Clean Architecture, Repository Pattern |
| **Phase 2** | Modular Monolith | Bounded Contexts, Domain Events |
| **Phase 3** | Microservices | Service Communication, Event-Driven Architecture |
| **Phase 4** | Distributed System | CQRS, Event Sourcing, Saga Pattern |

---

## 🛠️ Tech Stack

### Backend
| Technology | Purpose |
|------------|---------|
| **.NET 10** | Backend framework |
| **ASP.NET Core Web API** | RESTful APIs |
| **Entity Framework Core** | ORM |
| **MediatR** | CQRS & Mediator pattern |
| **FluentValidation** | Request validation |
| **AutoMapper** | Object mapping |
| **Serilog** | Structured logging |
| **xUnit + Moq** | Testing |

### Frontend
| Technology | Purpose |
|------------|---------|
| **React 18** | UI framework |
| **TypeScript** | Type safety |
| **Redux Toolkit** | State management |
| **React Router** | Navigation |
| **Axios** | HTTP client |
| **Tailwind CSS** | Styling |
| **Vite** | Build tool |
| **SignalR Client** | Real-time updates |

### Database & Infrastructure
| Technology | Purpose |
|------------|---------|
| **SQL Server 2022** | Primary database |
| **Redis** | Caching, Session, Distributed Locks |
| **Elasticsearch** | Full-text search |
| **RabbitMQ** | Message broker |
| **Docker** | Containerization |
| **SignalR** | Real-time communication |

---

## 📚 Learning Roadmap

### Phase 1: Foundation (Weeks 1-4)
**Goal**: Build a working monolith with clean architecture

**Learning Topics**:
- [ ] Clean Architecture / Onion Architecture
- [ ] Domain-Driven Design basics
- [ ] Repository & Unit of Work patterns
- [ ] Dependency Injection
- [ ] JWT Authentication

**📺 Videos**: Introduction, Project Setup, Clean Architecture

### Phase 2: Design Patterns (Weeks 5-8)
**Goal**: Implement core booking features with design patterns

**Learning Topics**:
- [ ] Factory Pattern (Ticket creation)
- [ ] Strategy Pattern (Dynamic pricing)
- [ ] Observer Pattern (Real-time seat updates)
- [ ] State Pattern (Booking lifecycle)
- [ ] Decorator Pattern (Price add-ons)

**📺 Videos**: One video per pattern with real implementation

### Phase 3: Advanced Patterns (Weeks 9-12)
**Goal**: Add booking flow with concurrency handling

**Learning Topics**:
- [ ] CQRS Pattern
- [ ] Mediator Pattern
- [ ] Chain of Responsibility (Validation)
- [ ] Command Pattern (Booking operations)
- [ ] Adapter Pattern (Payment gateways)

**📺 Videos**: Complex pattern implementations

### Phase 4: System Design (Weeks 13-16)
**Goal**: Handle high-volume scenarios

**Learning Topics**:
- [ ] Distributed Locking (Redis)
- [ ] Two-Phase Booking
- [ ] Caching strategies (Cache-Aside)
- [ ] Virtual Waiting Room
- [ ] Rate Limiting

**📺 Videos**: Ticketmaster case study solutions

### Phase 5: Real-Time & Scale (Weeks 17-20)
**Goal**: Real-time features and optimization

**Learning Topics**:
- [ ] SignalR for real-time updates
- [ ] Elasticsearch integration
- [ ] Event-driven architecture
- [ ] Database optimization
- [ ] Performance tuning

**📺 Videos**: Scaling and optimization techniques

---

## 🎬 Features & Scenarios

### User Flow

```
┌──────────────────────────────────────────────────────────────────────────┐
│                           BOOKING JOURNEY                                 │
├──────────────────────────────────────────────────────────────────────────┤
│                                                                           │
│  1. SEARCH          2. SELECT           3. SEATS           4. PAY        │
│  ┌─────────┐       ┌─────────┐         ┌─────────┐       ┌─────────┐    │
│  │ 🔍 Find │──────►│ 🎬 Pick │────────►│ 💺 Book │──────►│ 💳 Pay  │    │
│  │ Movie   │       │ Show    │         │ Seats   │       │         │    │
│  └─────────┘       └─────────┘         └─────────┘       └─────────┘    │
│       │                 │                   │                 │          │
│       ▼                 ▼                   ▼                 ▼          │
│  • By city         • Theater           • Real-time        • Multiple    │
│  • By ZIP          • Time slot           updates           gateways     │
│  • By genre        • Date              • Seat lock        • Confirm     │
│  • By name         • Format            • 10 min hold      • E-ticket    │
│                                                                           │
└──────────────────────────────────────────────────────────────────────────┘
```

### Scenarios by Complexity

| Scenario | Complexity | Design Patterns | System Design |
|----------|------------|-----------------|---------------|
| User Registration | Easy | Factory | Input Validation |
| Movie Search | Easy | Specification | Full-text Search |
| View Seat Layout | Medium | Composite | Matrix Representation |
| Select Seats | Medium | Observer | Real-time Updates |
| Hold Seats | Hard | State | Distributed Locking |
| Dynamic Pricing | Hard | Strategy + Decorator | Business Rules |
| Payment | Hard | Adapter + Command | Transaction Management |
| High-Volume Sale | Expert | Multiple | Virtual Queue, Sharding |

---

## 🎨 Design Patterns Used

### Creational Patterns
| Pattern | Use Case | Learning Value |
|---------|----------|----------------|
| **Factory Method** | Creating different ticket types | Object creation abstraction |
| **Builder** | Complex booking objects | Fluent interfaces |
| **Singleton** | Configuration manager | Thread-safe single instance |
| **Prototype** | Cloning seat layouts | Object copying |

### Structural Patterns
| Pattern | Use Case | Learning Value |
|---------|----------|----------------|
| **Adapter** | Payment gateway integration | Interface compatibility |
| **Composite** | Theater → Screen → Seat | Tree structures |
| **Decorator** | Price add-ons (3D, food) | Dynamic behavior |
| **Facade** | Booking API simplification | Subsystem abstraction |

### Behavioral Patterns
| Pattern | Use Case | Learning Value |
|---------|----------|----------------|
| **Strategy** | Dynamic pricing algorithms | Algorithm swapping |
| **Observer** | Real-time seat updates | Event notification |
| **State** | Booking lifecycle | State machine |
| **Command** | Booking operations | Operation encapsulation |
| **Chain of Responsibility** | Validation pipeline | Request handling |
| **Template Method** | Payment processing | Algorithm skeleton |

📖 **Deep Dive**: [Design Patterns Roadmap](docs/patterns/design-patterns-roadmap.md)

---

## 🏛️ System Design Concepts

### Concurrency Handling

```
┌──────────────────────────────────────────────────────────────┐
│                 TWO-PHASE BOOKING                             │
├──────────────────────────────────────────────────────────────┤
│                                                               │
│  PHASE 1: HOLD                    PHASE 2: CONFIRM           │
│  ┌─────────────────────┐         ┌─────────────────────┐    │
│  │                     │         │                     │    │
│  │  User selects       │         │  User pays          │    │
│  │  seats              │         │                     │    │
│  │        │            │         │        │            │    │
│  │        ▼            │         │        ▼            │    │
│  │  Redis SETNX        │────────►│  DB Transaction     │    │
│  │  (Distributed Lock) │         │  (Permanent)        │    │
│  │        │            │         │        │            │    │
│  │        ▼            │         │        ▼            │    │
│  │  10 min TTL         │         │  Seat = BOOKED      │    │
│  │  (Auto release)     │         │  Release Lock       │    │
│  │                     │         │                     │    │
│  └─────────────────────┘         └─────────────────────┘    │
│                                                               │
└──────────────────────────────────────────────────────────────┘
```

### Key System Design Topics

| Topic | Implementation | Document |
|-------|---------------|----------|
| **Distributed Locking** | Redis SETNX, Redlock | [High-Volume Design](docs/architecture/high-volume-system-design.md) |
| **Virtual Waiting Room** | Redis Sorted Set + WebSocket | [High-Volume Design](docs/architecture/high-volume-system-design.md) |
| **Caching Strategy** | Multi-layer (CDN, Redis, In-Memory) | [System Design](docs/architecture/system-design.md) |
| **Database Sharding** | City/Region based | [System Design](docs/architecture/system-design.md) |
| **Event-Driven Architecture** | RabbitMQ + Domain Events | [System Design](docs/architecture/system-design.md) |
| **Real-Time Updates** | SignalR + Redis Pub/Sub | [System Design](docs/architecture/system-design.md) |

---

## 📖 Documentation

| Document | Description |
|----------|-------------|
| [📐 System Design](docs/architecture/system-design.md) | Complete system architecture |
| [🚀 High-Volume Design](docs/architecture/high-volume-system-design.md) | Ticketmaster-scale solutions |
| [🎨 Design Patterns Roadmap](docs/patterns/design-patterns-roadmap.md) | Pattern-by-pattern learning |
| [📋 OOAD Principles](docs/architecture/ooad-principles.md) | SOLID, DDD, Clean Architecture |
| [🎯 GitHub Issues Setup](docs/project/GITHUB-ISSUES-SETUP.md) | Complete issue templates |
| [📺 Content Creator Roadmap](docs/CONTENT-CREATOR-ROADMAP.md) | YouTube/Instagram guide |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Development Plan (No Code Yet!)

This is a **documentation-first** project. Current phase:

1. ✅ Project vision and scope defined
2. ✅ Architecture documented
3. ✅ Design patterns mapped
4. ✅ System design documented
5. ✅ GitHub issues and milestones planned
6. ⏳ **Next**: Create solution structure (following the docs!)

---

## 🎯 Milestones

| Milestone | Focus | Duration |
|-----------|-------|----------|
| **M1** | Project Foundation | Week 1-2 |
| **M2** | Domain & Database | Week 3-4 |
| **M3** | Auth & Security | Week 5-6 |
| **M4** | Catalog Management | Week 7-8 |
| **M5** | Booking System | Week 9-12 |
| **M6** | Payments | Week 13-14 |
| **M7** | Frontend | Week 15-18 |
| **M8** | Advanced Features | Week 19-22 |

📖 **Detailed Plan**: [GitHub Issues Setup](docs/project/GITHUB-ISSUES-SETUP.md)

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📞 Contact

**Project Link**: [https://github.com/awalekeeran/BookTheShow](https://github.com/awalekeeran/BookTheShow)

---

<p align="center">
  <strong>Built for learning System Design, OOAD, and Design Patterns</strong><br>
  <em>Inspired by the Ticketmaster Taylor Swift Eras Tour incident</em> 🎤
</p>

---

## ⭐ Star History

If this project helps you learn, please give it a star! ⭐
- [ ] Service decomposition
- [ ] API Gateway pattern
- [ ] Saga Pattern (Distributed transactions)
- [ ] Event Sourcing
- [ ] Service discovery

---

## 🎬 Features & Scenarios

### Epic 1: User Management
| Feature | Description | Design Pattern | System Design Concept |
|---------|-------------|----------------|----------------------|
| User Registration | Sign up with email/phone | Factory | Input validation |
| Authentication | Login with JWT | Strategy | Token management |
| Profile Management | Update profile, preferences | Builder | Caching |
| Role-based Access | Admin, User, Theater Owner | - | Authorization |

### Epic 2: Catalog Management
| Feature | Description | Design Pattern | System Design Concept |
|---------|-------------|----------------|----------------------|
| Movie CRUD | Add/Edit/Delete movies | Repository | Database design |
| Theater Management | Manage theaters, screens | Composite | Hierarchical data |
| Show Scheduling | Create show timings | Builder | Time slot management |
| Seat Layout Design | Configure seat arrangements | Prototype | Matrix representation |

### Epic 3: Search & Discovery
| Feature | Description | Design Pattern | System Design Concept |
|---------|-------------|----------------|----------------------|
| Movie Search | Search by name, genre | Specification | Full-text search |
| Filter & Sort | Multiple filters | Strategy | Query optimization |
| Recommendations | Personalized suggestions | Strategy | ML integration |
| Nearby Theaters | Location-based search | - | Geospatial queries |

### Epic 4: Booking Flow
| Feature | Description | Design Pattern | System Design Concept |
|---------|-------------|----------------|----------------------|
| Seat Selection | Interactive seat map | Observer | Real-time updates |
| Seat Locking | Temporary hold | - | Distributed locking |
| Price Calculation | Dynamic pricing | Strategy + Decorator | Business rules |
| Booking Confirmation | Complete booking | State | State machine |

### Epic 5: Payment Processing
| Feature | Description | Design Pattern | System Design Concept |
|---------|-------------|----------------|----------------------|
| Payment Gateway | Multiple payment options | Adapter | Third-party integration |
| Transaction Management | Handle payments | Command | Transaction handling |
| Refund Processing | Handle cancellations | - | Eventual consistency |
| Invoice Generation | PDF tickets | Template | Document generation |

### Epic 6: Notifications
| Feature | Description | Design Pattern | System Design Concept |
|---------|-------------|----------------|----------------------|
| Email Notifications | Booking confirmations | Observer | Async processing |
| SMS Alerts | Reminders | Observer | Message queues |
| Push Notifications | Real-time updates | Observer | WebSockets |

---

## 🎨 Design Patterns Used

### Creational Patterns

| Pattern | Use Case in BookTheShow | Implementation |
|---------|------------------------|----------------|
| **Factory Method** | Creating different ticket types (Regular, Premium, VIP) | `TicketFactory` |
| **Abstract Factory** | Creating UI components for different themes | `ThemeFactory` |
| **Builder** | Constructing complex booking objects | `BookingBuilder` |
| **Prototype** | Cloning seat layouts for new screens | `SeatLayoutPrototype` |
| **Singleton** | Configuration manager, Logger | `ConfigurationManager` |

### Structural Patterns

| Pattern | Use Case in BookTheShow | Implementation |
|---------|------------------------|----------------|
| **Adapter** | Payment gateway integrations | `PaymentGatewayAdapter` |
| **Bridge** | Separating notification types from channels | `NotificationBridge` |
| **Composite** | Theater → Screen → Seat hierarchy | `TheaterComposite` |
| **Decorator** | Adding features to base ticket price | `PriceDecorator` |
| **Facade** | Simplified booking API | `BookingFacade` |
| **Proxy** | Lazy loading of movie details | `MovieProxy` |

### Behavioral Patterns

| Pattern | Use Case in BookTheShow | Implementation |
|---------|------------------------|----------------|
| **Chain of Responsibility** | Validation pipeline | `ValidationChain` |
| **Command** | Booking operations (book, cancel, modify) | `BookingCommand` |
| **Iterator** | Iterating through available seats | `SeatIterator` |
| **Mediator** | Communication between booking components | `MediatR` |
| **Observer** | Seat availability updates | `SeatObserver` |
| **State** | Booking lifecycle states | `BookingState` |
| **Strategy** | Pricing algorithms, search algorithms | `PricingStrategy` |
| **Template Method** | Payment processing workflow | `PaymentTemplate` |

---

## 🏛️ System Design Concepts

### 1. Database Design

```
┌─────────────────────────────────────────────────────────────────────┐
│                         DATABASE SCHEMA                              │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  ┌──────────────┐     ┌──────────────┐     ┌──────────────┐        │
│  │    Users     │     │   Movies     │     │   Theaters   │        │
│  ├──────────────┤     ├──────────────┤     ├──────────────┤        │
│  │ Id           │     │ Id           │     │ Id           │        │
│  │ Name         │     │ Title        │     │ Name         │        │
│  │ Email        │     │ Genre        │     │ Location     │        │
│  │ PasswordHash │     │ Duration     │     │ City         │        │
│  │ Phone        │     │ Rating       │     │ Address      │        │
│  │ Role         │     │ ReleaseDate  │     │ ContactNo    │        │
│  └──────────────┘     └──────────────┘     └──────┬───────┘        │
│         │                    │                    │                 │
│         │                    │                    │                 │
│         │             ┌──────┴───────┐     ┌──────┴───────┐        │
│         │             │    Shows     │     │   Screens    │        │
│         │             ├──────────────┤     ├──────────────┤        │
│         │             │ Id           │     │ Id           │        │
│         │             │ MovieId (FK) │     │ TheaterId(FK)│        │
│         │             │ ScreenId(FK) │     │ Name         │        │
│         │             │ StartTime    │     │ TotalSeats   │        │
│         │             │ EndTime      │     │ SeatLayout   │        │
│         │             │ Price        │     └──────────────┘        │
│         │             └──────┬───────┘                              │
│         │                    │                                      │
│         │             ┌──────┴───────┐     ┌──────────────┐        │
│         │             │    Seats     │     │  SeatTypes   │        │
│         │             ├──────────────┤     ├──────────────┤        │
│         │             │ Id           │     │ Id           │        │
│         │             │ ScreenId(FK) │     │ Name         │        │
│         │             │ SeatTypeId   │     │ PriceMultiplier│      │
│         │             │ RowNumber    │     └──────────────┘        │
│         │             │ SeatNumber   │                              │
│         │             └──────┬───────┘                              │
│         │                    │                                      │
│  ┌──────┴───────┐     ┌──────┴───────┐     ┌──────────────┐        │
│  │   Bookings   │────►│ BookedSeats  │     │   Payments   │        │
│  ├──────────────┤     ├──────────────┤     ├──────────────┤        │
│  │ Id           │     │ Id           │     │ Id           │        │
│  │ UserId (FK)  │     │ BookingId(FK)│     │ BookingId(FK)│        │
│  │ ShowId (FK)  │     │ SeatId (FK)  │     │ Amount       │        │
│  │ TotalAmount  │     │ Price        │     │ Status       │        │
│  │ Status       │     └──────────────┘     │ PaymentMethod│        │
│  │ BookingTime  │                          │ TransactionId│        │
│  └──────────────┘                          └──────────────┘        │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

### 2. Concurrency Handling (Seat Booking)

```
┌─────────────────────────────────────────────────────────────────────┐
│              SEAT BOOKING CONCURRENCY SOLUTION                       │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  User A ─────┐                                                       │
│              │    ┌─────────────┐    ┌─────────────┐                │
│              ├───►│   Redis     │───►│   Lock      │                │
│              │    │ Distributed │    │   Acquired  │                │
│  User B ─────┤    │    Lock     │    └──────┬──────┘                │
│              │    └─────────────┘           │                        │
│              │           │                  ▼                        │
│              │           │         ┌─────────────┐                  │
│              │           │         │  Database   │                  │
│              │           │         │  Transaction│                  │
│              │           │         └──────┬──────┘                  │
│              │           │                │                          │
│              │    ┌──────▼──────┐  ┌──────▼──────┐                  │
│              └───►│   Retry     │  │   Confirm   │                  │
│                   │   Queue     │  │   Booking   │                  │
│                   └─────────────┘  └─────────────┘                  │
│                                                                      │
│  Optimistic Locking: Version column in database                     │
│  Pessimistic Locking: SELECT FOR UPDATE                             │
│  Distributed Locking: Redis SETNX with TTL                          │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

### 3. Caching Strategy

```
┌─────────────────────────────────────────────────────────────────────┐
│                      CACHING ARCHITECTURE                            │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  ┌─────────────┐         ┌─────────────┐         ┌─────────────┐   │
│  │   Request   │────────►│   Redis     │────────►│  Database   │   │
│  │             │         │   Cache     │         │             │   │
│  └─────────────┘         └─────────────┘         └─────────────┘   │
│                                                                      │
│  Cache Policies:                                                    │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │ Data Type          │ Strategy      │ TTL      │ Invalidation│   │
│  ├─────────────────────────────────────────────────────────────┤   │
│  │ Movie List         │ Cache-Aside   │ 1 hour   │ On Update   │   │
│  │ Show Timings       │ Cache-Aside   │ 15 mins  │ On Update   │   │
│  │ Seat Availability  │ Write-Through │ 30 secs  │ Real-time   │   │
│  │ User Sessions      │ Cache-Aside   │ 24 hours │ On Logout   │   │
│  │ Search Results     │ Cache-Aside   │ 5 mins   │ TTL         │   │
│  └─────────────────────────────────────────────────────────────┘   │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

### 4. Event-Driven Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                   EVENT-DRIVEN BOOKING FLOW                          │
├─────────────────────────────────────────────────────────────────────┤
│                                                                      │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐      │
│  │  Booking │───►│ Payment  │───►│  Ticket  │───►│  Notify  │      │
│  │  Created │    │ Processed│    │ Generated│    │   User   │      │
│  └────┬─────┘    └────┬─────┘    └────┬─────┘    └────┬─────┘      │
│       │               │               │               │             │
│       ▼               ▼               ▼               ▼             │
│  ┌─────────────────────────────────────────────────────────────┐   │
│  │                      RabbitMQ / Azure Service Bus            │   │
│  │  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌─────────┐            │   │
│  │  │ booking │ │ payment │ │ ticket  │ │ notif   │            │   │
│  │  │ .events │ │ .events │ │ .events │ │ .events │            │   │
│  │  └─────────┘ └─────────┘ └─────────┘ └─────────┘            │   │
│  └─────────────────────────────────────────────────────────────┘   │
│       │               │               │               │             │
│       ▼               ▼               ▼               ▼             │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐      │
│  │ Booking  │    │ Payment  │    │  Email   │    │   SMS    │      │
│  │ Service  │    │ Service  │    │ Service  │    │ Service  │      │
│  └──────────┘    └──────────┘    └──────────┘    └──────────┘      │
│                                                                      │
└─────────────────────────────────────────────────────────────────────┘
```

---

## 📐 OOAD Principles

### SOLID Principles Application

| Principle | Application in BookTheShow |
|-----------|---------------------------|
| **S**ingle Responsibility | `BookingService` only handles booking logic, `PaymentService` only handles payments |
| **O**pen/Closed | Pricing strategies can be extended without modifying `PriceCalculator` |
| **L**iskov Substitution | All payment gateways (`RazorpayGateway`, `StripeGateway`) are interchangeable |
| **I**nterface Segregation | `IBookingReader` and `IBookingWriter` instead of single `IBookingService` |
| **D**ependency Inversion | Services depend on `IRepository<T>`, not concrete implementations |

### Domain Model (Core Entities)

```csharp
// Aggregate Roots
- User (Aggregate Root)
  └── UserProfile (Entity)
  └── BookingHistory (Value Object)

- Movie (Aggregate Root)
  └── MovieDetails (Entity)
  └── Cast (Value Object)
  └── Reviews (Entity)

- Theater (Aggregate Root)
  └── Screen (Entity)
      └── Seat (Entity)
  └── Location (Value Object)

- Show (Aggregate Root)
  └── ShowTiming (Value Object)
  └── Pricing (Value Object)

- Booking (Aggregate Root)
  └── BookedSeat (Entity)
  └── Payment (Entity)
  └── Ticket (Value Object)
```

---

## 📁 Project Structure

```
BookTheShow/
├── 📁 docs/
│   ├── 📁 architecture/
│   │   ├── system-design.md
│   │   ├── database-design.md
│   │   └── api-design.md
│   ├── 📁 diagrams/
│   │   ├── class-diagrams/
│   │   ├── sequence-diagrams/
│   │   └── use-case-diagrams/
│   └── 📁 patterns/
│       ├── creational-patterns.md
│       ├── structural-patterns.md
│       └── behavioral-patterns.md
│
├── 📁 src/
│   ├── 📁 Backend/
│   │   ├── 📁 BookTheShow.API/                 # Web API Layer
│   │   │   ├── Controllers/
│   │   │   ├── Middleware/
│   │   │   ├── Filters/
│   │   │   └── Program.cs
│   │   │
│   │   ├── 📁 BookTheShow.Application/         # Application Layer (CQRS)
│   │   │   ├── Commands/
│   │   │   ├── Queries/
│   │   │   ├── Handlers/
│   │   │   ├── DTOs/
│   │   │   ├── Validators/
│   │   │   └── Mappings/
│   │   │
│   │   ├── 📁 BookTheShow.Domain/              # Domain Layer
│   │   │   ├── Entities/
│   │   │   ├── ValueObjects/
│   │   │   ├── Aggregates/
│   │   │   ├── Events/
│   │   │   ├── Interfaces/
│   │   │   └── Exceptions/
│   │   │
│   │   ├── 📁 BookTheShow.Infrastructure/      # Infrastructure Layer
│   │   │   ├── Data/
│   │   │   │   ├── Context/
│   │   │   │   ├── Configurations/
│   │   │   │   ├── Repositories/
│   │   │   │   └── Migrations/
│   │   │   ├── Services/
│   │   │   ├── Caching/
│   │   │   └── Messaging/
│   │   │
│   │   └── 📁 BookTheShow.Shared/              # Shared Kernel
│   │       ├── Constants/
│   │       ├── Extensions/
│   │       └── Helpers/
│   │
│   └── 📁 Frontend/
│       └── 📁 book-the-show-web/               # React Application
│           ├── 📁 src/
│           │   ├── 📁 components/
│           │   ├── 📁 pages/
│           │   ├── 📁 features/
│           │   ├── 📁 hooks/
│           │   ├── 📁 services/
│           │   ├── 📁 store/
│           │   ├── 📁 types/
│           │   └── 📁 utils/
│           ├── package.json
│           └── vite.config.ts
│
├── 📁 tests/
│   ├── 📁 BookTheShow.UnitTests/
│   ├── 📁 BookTheShow.IntegrationTests/
│   └── 📁 BookTheShow.E2ETests/
│
├── 📁 scripts/
│   ├── setup.ps1
│   └── seed-data.sql
│
├── 📄 docker-compose.yml
├── 📄 .gitignore
├── 📄 README.md
└── 📄 BookTheShow.sln
```

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

## 🎯 Milestones

### Milestone 1: Project Foundation ✅
- [x] Initialize repository
- [ ] Setup solution structure
- [ ] Configure CI/CD pipeline
- [ ] Setup development environment

### Milestone 2: Core Domain
- [ ] Domain models & entities
- [ ] Database schema & migrations
- [ ] Repository implementations
- [ ] Unit tests for domain

### Milestone 3: Authentication & User Management
- [ ] JWT authentication
- [ ] User registration/login
- [ ] Role-based authorization
- [ ] Profile management

### Milestone 4: Catalog Features
- [ ] Movie management APIs
- [ ] Theater & screen management
- [ ] Show scheduling
- [ ] Search functionality

### Milestone 5: Booking System
- [ ] Seat selection
- [ ] Booking workflow
- [ ] Concurrency handling
- [ ] Price calculation

### Milestone 6: Payment Integration
- [ ] Payment gateway integration
- [ ] Transaction management
- [ ] Refund processing

### Milestone 7: Frontend Development
- [ ] React app setup
- [ ] Authentication UI
- [ ] Movie browsing
- [ ] Booking flow UI

### Milestone 8: Advanced Features
- [ ] Caching implementation
- [ ] Elasticsearch integration
- [ ] Notification system
- [ ] Analytics dashboard

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📞 Contact

**Project Link**: [https://github.com/awalekeeran/BookTheShow](https://github.com/awalekeeran/BookTheShow)

---

<p align="center">
  Made with ❤️ for learning System Design, OOAD, and Design Patterns
</p>
