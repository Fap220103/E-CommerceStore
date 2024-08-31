# Cài Đặt Dự Án

## Cài Đặt

1. clone dự án về
2. Tạo file appsettings.json và sao chép file `appsettings.development.json` vào `appsettings.json`.
3. Chỉnh sửa các giá trị trong `appsettings.json` để phù hợp với môi trường của bạn.

## Biến Môi Trường

Thiết lập các biến môi trường sau:

- `ASPNETCORE_ENVIRONMENT=Development`
- `ConnectionStrings__DefaultConnection=Server=myServer;Database=myDb;User Id=myUser;Password=myPass;`

## Chạy Ứng Dụng

Sử dụng lệnh sau để chạy ứng dụng:

```bash
dotnet run
