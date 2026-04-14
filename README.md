# WhatsApp Notification System


A distributed, event-driven WhatsApp notification system built with C# and ASP.NET Core. The system processes banking transactions and delivers real-time WhatsApp alerts to users via Twilio.

---

## Architecture Overview

```
NotificationDashboard (Razor Pages UI)
        ↓ HTTP POST
NotificationApi (ASP.NET Core Web API)
        ↓ Publish event
Kafka (Message Broker - Docker)
        ↓ Consume event
NotificationConsumer (Worker Service)
        ↓ Save + Send
SQL Server (Docker) + Twilio WhatsApp API
        ↓
User's WhatsApp
```

---

## Projects

| Project | Type | Description |
|---|---|---|
| `Shared` | Class Library | Models, DbContext, Events, Enums |
| `NotificationApi` | ASP.NET Core Web API | REST endpoints, Kafka producer |
| `NotificationConsumer` | Worker Service | Kafka consumer, Twilio sender |
| `CronJob` | Worker Service | Quartz.NET retry scheduler |
| `NotificationDashboard` | Razor Pages | Monitoring UI |

---

## Tech Stack

- **Language**: C# / .NET 10
- **Framework**: ASP.NET Core
- **Message Broker**: Apache Kafka (Docker)
- **Database**: SQL Server (Docker)
- **ORM**: Entity Framework Core
- **WhatsApp**: Twilio API
- **Scheduler**: Quartz.NET
- **UI**: Razor Pages
- **API Docs**: Swagger UI

---

## Database Tables

| Table | Description |
|---|---|
| `tbl_Notification_Setup` | Message templates per transaction type |
| `tbl_Notifications` | Log of all sent/pending/failed notifications |
| `tbl_Accounts` | Mock bank accounts |
| `tbl_Transactions` | Mock bank transactions |

---

## API Endpoints

### Accounts
- `POST /api/Accounts` — Create mock account
- `GET /api/Accounts` — Get all accounts
- `GET /api/Accounts/{id}` — Get account by ID

### Transactions
- `POST /api/BankTransactions/deposit` — Deposit funds
- `POST /api/BankTransactions/withdraw` — Withdraw funds
- `POST /api/BankTransactions/transfer` — Transfer between accounts

### Notifications
- `POST /api/Notification/trigger` — Manually trigger a notification

---

## Prerequisites

- .NET 10 SDK
- Docker Desktop
- SQL Server Management Studio (SSMS)
- Twilio account (sandbox)
- Visual Studio 2022

---

## Getting Started

### 1. Start Docker containers

```bash
docker-compose up -d
```

This starts:
- Kafka + Zookeeper on port `9092`
- SQL Server on port `1433`

### 2. Create Kafka topic

```bash
docker exec -it kafka kafka-topics --create \
  --topic bank-notifications \
  --bootstrap-server localhost:9092 \
  --partitions 3 \
  --replication-factor 1
```

### 3. Set up the database

Connect to SQL Server via SSMS (`localhost,1433`, user: `sa`) and run the setup scripts to create tables and seed notification templates.

### 4. Configure credentials

Update `appsettings.json` in `NotificationConsumer` and `CronJob`:

```json
{
  "Twilio": {
    "AccountSid": "YOUR_ACCOUNT_SID",
    "AuthToken": "YOUR_AUTH_TOKEN",
    "WhatsAppFrom": "whatsapp:+14155238886"
  }
}
```

### 5. Join Twilio Sandbox

Send the sandbox join message from your WhatsApp to `+14155238886` before testing.

### 6. Run all projects

Set multiple startup projects in Visual Studio:
- `NotificationApi`
- `NotificationConsumer`
- `CronJob`
- `NotificationDashboard`

Press **F5**.

---

## URLs

| Service | URL |
|---|---|
| Dashboard | `https://localhost:7100` |
| Swagger UI | `https://localhost:7271/swagger` |
| Kafka | `localhost:9092` |
| SQL Server | `localhost,1433` |

---

## Notification Flow

1. User performs a transaction in the dashboard
2. `NotificationApi` saves the transaction and publishes a `TransactionEvent` to Kafka
3. `NotificationConsumer` picks up the event, fetches the message template from `tbl_Notification_Setup`, formats the message, saves to `tbl_Notifications` as Pending, and calls Twilio
4. Twilio delivers the WhatsApp message to the user
5. Notification status is updated to Sent or Failed
6. `CronJob` runs every 30 seconds to retry any Pending or Failed notifications

---

## WhatsApp Message Format

```
🏦 Bank Alert
✅ Credit: ₦50,000.00
Account: ACC-1234567
Desc: Salary Payment
Balance: ₦120,000.00
Ref: TXN-20260411193352-5829
Date: 11 Apr 2026, 07:33 PM
```

---

## Project Structure

```
WhatsAppNotificationSystem/
├── docker-compose.yml
├── Shared/
│   ├── Data/AppDbContext.cs
│   ├── Events/TransactionEvent.cs
│   ├── Models/
│   │   ├── Account.cs
│   │   ├── Transaction.cs
│   │   ├── Notification.cs
│   │   └── NotificationSetup.cs
│   └── Enums/
│       ├── NotificationType.cs
│       └── NotificationStatus.cs
├── NotificationApi/
│   ├── Controllers/
│   ├── Services/
│   └── DTOs/
├── NotificationConsumer/
│   ├── Workers/KafkaConsumerWorker.cs
│   └── Services/
├── CronJob/
│   ├── Jobs/RetryFailedNotificationsJob.cs
│   └── Services/
└── NotificationDashboard/
    ├── Pages/
    │   ├── Index.cshtml
    │   ├── Accounts.cshtml
    │   ├── Transactions.cshtml
    │   ├── Notifications.cshtml
    │   └── Templates.cshtml
    └── wwwroot/css/site.css
```

---

## License

Built for internal banking notification demonstration purposes.
=======
An enterprise-grade, event-driven notification system that processes banking transactions and sends real-time WhatsApp alerts using Twilio.

## 🚀 Features
- Real-time transaction notifications
- Event-driven architecture using Kafka
- Retry mechanism with Quartz.NET
- Dashboard for monitoring and testing
- WhatsApp messaging via Twilio

## 🏗️ Architecture
- NotificationApi → Produces events to Kafka
- NotificationConsumer → Consumes and sends messages
- CronJob → Retries failed notifications
- Dashboard → UI for monitoring

## 🛠️ Tech Stack
- C# / .NET
- ASP.NET Core Web API
- Apache Kafka
- SQL Server
- Entity Framework Core
- Twilio API
- Quartz.NET
- Docker

## 📦 Projects
- Shared
- NotificationApi
- NotificationConsumer
- CronJob
- NotificationDashboard

## ⚙️ Setup
1. Clone the repo
2. Run:
   ```bash
   docker-compose up -d
   ```
3. Create Kafka topic:
   ```bash
   docker exec -it kafka kafka-topics --create \
   --topic bank-notifications \
   --bootstrap-server localhost:9092 \
   --partitions 3 \
   --replication-factor 1
   ```

4. Configure Twilio credentials in `appsettings.json`

5. Run all projects in Visual Studio

## 📊 Endpoints
- `/api/Accounts`
- `/api/BankTransactions`
- `/api/Notification`

## 📌 Dashboard
- https://localhost:7100

## 📌 Swagger
- https://localhost:7271/swagger

---

