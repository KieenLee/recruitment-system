# Hệ Thống Tự Động Hóa Tuyển Dụng Bằng AI

## Tổng Quan
Dự án "Hệ Thống Tự Động Hóa Tuyển Dụng Bằng AI" được xây dựng với kiến trúc Polyglot Persistence, sử dụng SQL Server cho dữ liệu có cấu trúc và MongoDB cho dữ liệu linh hoạt của ứng viên. Mục tiêu của hệ thống là cung cấp một nền tảng hiệu quả cho việc quản lý tuyển dụng, từ việc đăng tin tuyển dụng đến việc phân tích hồ sơ ứng viên bằng AI.

## Kiến Trúc Dự Án
- **Backend:** Được phát triển bằng C# với .NET 8, sử dụng Minimal APIs để kết nối với cả SQL Server và MongoDB.
- **Frontend:** Sử dụng React, khởi tạo bằng Vite, để tạo giao diện người dùng tương tác với backend thông qua API.

## Cấu Trúc Thư Mục
```
RecruitmentAiSystem
├── Backend
│   ├── Data
│   ├── Models
│   ├── DTOs
│   ├── Services
│   ├── Program.cs
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Backend.csproj
│   └── Dockerfile
├── frontend
│   ├── public
│   ├── src
│   ├── package.json
│   ├── vite.config.js
│   └── Dockerfile
├── docker-compose.yml
└── README.md
```

## Hướng Dẫn Cài Đặt

### Bước 1: Cài Đặt Backend
1. Di chuyển vào thư mục `Backend`.
2. Chạy lệnh sau để cài đặt các gói NuGet cần thiết:
   ```
   dotnet add package Microsoft.EntityFrameworkCore.SqlServer
   dotnet add package Microsoft.EntityFrameworkCore.Tools
   dotnet add package MongoDB.Driver
   dotnet add package Swashbuckle.AspNetCore
   ```
3. Cấu hình chuỗi kết nối trong `appsettings.json`.

### Bước 2: Cài Đặt Frontend
1. Di chuyển vào thư mục `frontend`.
2. Chạy lệnh sau để cài đặt các phụ thuộc:
   ```
   npm install
   ```

### Bước 3: Chạy Ứng Dụng
1. Để chạy backend, sử dụng lệnh:
   ```
   dotnet run
   ```
2. Để chạy frontend, sử dụng lệnh:
   ```
   npm run dev
   ```

### Bước 4: Triển Khai với Docker
1. Sử dụng `docker-compose` để triển khai toàn bộ ứng dụng:
   ```
   docker-compose up --build
   ```

## Liên Hệ
Nếu bạn có bất kỳ câu hỏi nào về dự án, vui lòng liên hệ với nhóm phát triển qua email: support@recruitmentaisystem.com.