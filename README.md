# 📱 WhatsApp Notification System

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-12.0-239120?style=flat&logo=csharp&logoColor=white)
![Kafka](https://img.shields.io/badge/Apache%20Kafka-231F20?style=flat&logo=apachekafka&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)
![Twilio](https://img.shields.io/badge/Twilio-F22F46?style=flat&logo=twilio&logoColor=white)
![Quartz.NET](https://img.shields.io/badge/Quartz.NET-Scheduler-orange?style=flat)
![License](https://img.shields.io/badge/License-MIT-green?style=flat)

A distributed, event-driven WhatsApp notification system built with **C# and ASP.NET Core**. The system processes banking transactions and delivers real-time WhatsApp alerts to users via Twilio.

---

## ✨ Features

- 🔔 Real-time WhatsApp alerts for banking transactions (deposits, withdrawals, transfers)
- 📨 Event-driven architecture using Apache Kafka
- 🔁 Automatic retry mechanism with Quartz.NET (every 30 seconds)
- 📊 Monitoring dashboard to track notification statuses
- 🐳 Fully containerized with Docker (Kafka + SQL Server)

---

## 🏗️ Architecture


![WhatsApp Notification Architecture](https://github.com/Anonymous23r56/WhatsAppNotificationSystem/blob/375f1dfcb46518d19d1fa1d07b7226000aa6b40b/whatsapp_notification_architecture.svg)

### Notification Flow

1. User performs a transaction via the dashboard
2. `NotificationApi` saves the transaction and publishes a `TransactionEvent` to Kafka
3. `NotificationConsumer` picks up the event, fetches the message template, saves the notification as **Pending**, and calls Twilio
4. Twilio delivers the WhatsApp message to the user
5. Notification status is updated to **Sent** or **Failed**
6. `CronJob` runs every **30 seconds** to retry any Pending or Failed notifications

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Language | C# / .NET 10 |
| Framework | ASP.NET Core |
| Message Broker | Apache Kafka (Docker) |
| Database | SQL Server (Docker) |
| ORM | Entity Framework Core |
| WhatsApp Messaging | Twilio API |
| Scheduler | Quartz.NET |
| UI | Razor Pages |
| API Docs | Swagger UI |
 

---

## 📦 Projects

| Project | Type | Description |
|---|---|---|
| `Shared` | Class Library | Models, DbContext, Events, Enums |
| `NotificationApi` | ASP.NET Core Web API | REST endpoints, Kafka producer |
| `NotificationConsumer` | Worker Service | Kafka consumer, Twilio sender |
| `CronJob` | Worker Service | Quartz.NET retry scheduler |
| `NotificationDashboard` | Razor Pages | Monitoring UI |

---

## 🗄️ Database Tables

| Table | Description |
|---|---|
| `tbl_Notification_Setup` | Message templates per transaction type |
| `tbl_Notifications` | Log of all sent/pending/failed notifications |
| `tbl_Accounts` | Mock bank accounts |
| `tbl_Transactions` | Mock bank transactions |

---

## ⚙️ Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [Twilio Account (Sandbox)](https://www.twilio.com/try-twilio)
- Visual Studio 2022

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/Anonymous23r56/WhatsAppNotificationSystem.git
cd WhatsAppNotificationSystem
```

### 2. Start Docker containers

```bash
docker-compose up -d
```

This starts:
- Kafka + Zookeeper on port `9092`
- SQL Server on port `1433`

### 3. Create Kafka topic

```bash
docker exec -it kafka kafka-topics --create \
  --topic bank-notifications \
  --bootstrap-server localhost:9092 \
  --partitions 3 \
  --replication-factor 1
```

### 4. Set up the database

Connect to SQL Server via SSMS using:
- **Host**: `localhost,1433`
- **Username**: `sa`

Run the setup scripts to create tables and seed notification templates.

### 5. Configure credentials

Update `appsettings.json` in both `NotificationConsumer` and `CronJob`:

```json
{
  "Twilio": {
    "AccountSid": "YOUR_ACCOUNT_SID",
    "AuthToken": "YOUR_AUTH_TOKEN",
    "WhatsAppFrom": "whatsapp:+14155238886"
  }
}
```

> ⚠️ **Never commit real credentials to version control.** Use environment variables or a secrets manager in production.

### 6. Join Twilio Sandbox

Send the sandbox join message from your WhatsApp to **+1 415 523 8886** before testing.

### 7. Run all projects

In Visual Studio 2022, set multiple startup projects:
- `NotificationApi`
- `NotificationConsumer`
- `CronJob`
- `NotificationDashboard`

Then press **F5**.

---

## 🌐 Service URLs

| Service | URL |
|---|---|
| Dashboard | `https://localhost:7100` |
| Swagger UI | `https://localhost:7271/swagger` |
| Kafka | `localhost:9092` |
| SQL Server | `localhost,1433` |

---

## 📡 API Endpoints

### Accounts
| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/Accounts` | Create a mock account |
| `GET` | `/api/Accounts` | Get all accounts |
| `GET` | `/api/Accounts/{id}` | Get account by ID |

### Transactions
| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/BankTransactions/deposit` | Deposit funds |
| `POST` | `/api/BankTransactions/withdraw` | Withdraw funds |
| `POST` | `/api/BankTransactions/transfer` | Transfer between accounts |

### Notifications
| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/Notification/trigger` | Manually trigger a notification |

---

## 💬 WhatsApp Message Format

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

## 📁 Project Structure

```
WhatsAppNotificationSystem/
├── docker-compose.yml
│
├── CronJob/                                  # Quartz.NET retry scheduler
│   ├── Jobs/
│   │   └── RetryFailedNotificationsJob.cs
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   └── ITwilioWhatsAppService.cs
│   │   └── TwilioWhatsAppService.cs
│   └── appsettings.json
│   ├── Program.cs
│
├── NotificationApi/                          # REST API + Kafka producer
│   ├── Controllers/
│   │   ├── AccountsController.cs
│   │   ├── BankTransactionsController.cs
│   │   └── NotificationController.cs
│   ├── DTOs/
│   │   ├── BankTransactionDto.cs
│   │   ├── CreateAccountDto.cs
│   │   └── TransactionRequestDto.cs
│   ├── Services/
│   │   ├── Interfaces/
│   │   │   ├── IAccountService.cs
│   │   │   ├── IBankTransactionService.cs
│   │   │   └── IKafkaProducerService.cs
│   │   ├── AccountService.cs
│   │   ├── BankTransactionService.cs
│   │   └── KafkaProducerService.cs
│   ├── Validators/
│   │   ├── BankTransactionValidator.cs
│   │   ├── CreateAccountValidator.cs
│   │   └── TransactionEventValidator.cs
│   └── appsettings.json
│   ├── Program.cs
│
├── NotificationConsumer/                     # Kafka consumer + Twilio sender
│   ├── Services/
│   │   ├── Interfaces/
│   │   ├── NotificationLogService.cs
│   │   ├── NotificationSetupService.cs
│   │   └── TwilioWhatsAppService.cs
│   ├── Workers/
│   │   └── KafkaConsumerWorker.cs
│   └── appsettings.json
│   ├── Program.cs
│
├── NotificationDashboard/                    # Razor Pages monitoring UI
│   ├── Pages/
│   │   ├── Index.cshtml
│   │   ├── Accounts.cshtml
│   │   ├── Transactions.cshtml
│   │   ├── Notifications.cshtml
│   │   └── Templates.cshtml
│   ├── wwwroot/
│   └── appsettings.json
│   ├── Program.cs
│
└── Shared/                                   # Shared models, events, enums
    ├── Data/
    │   └── AppDbContext.cs
    ├── Events/
    │   └── TransactionEvent.cs
    ├── Models/
    │   ├── Account.cs
    │   ├── Transaction.cs
    │   ├── Notification.cs
    │   └── NotificationSetup.cs
    └── Enums/
        ├── NotificationType.cs
        └── NotificationStatus.cs
```

---

## 🔒 Security Notes

- Store Twilio credentials using **environment variables** or **ASP.NET Core User Secrets** — never hardcode them
- This project uses the **Twilio Sandbox** for development; switch to a production number before going live
- Restrict SQL Server access in production environments

---
## 👤 Author

**Samuel Olokor**

---

## 🔗 Connect With Me  
- LinkedIn: https://www.linkedin.com/in/samuel-olokor-a073bb326/ 
- Email: samuelolokor228@gmail.com

*Built for internal banking notification demonstration purposes.*
