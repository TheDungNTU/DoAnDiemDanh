## Cài đặt

- Chạy file SQL trong thư mục Content/sql
- Đổi chuỗi Connect String trong file Web.config

```
<connectionStrings>
    <add name="ASPNETConnectionString" connectionString="data source=DESKTOP-37IIIG3;initial catalog=FACE_RECOGNITION;Integrated Security=True" providerName="System.Data.SqlClient" />
    <add name="FACE_RECOGNITIONEntities" connectionString="metadata=res://*/Models.FaceRecognition.csdl|res://*/Models.FaceRecognition.ssdl|res://*/Models.FaceRecognition.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=DESKTOP-37IIIG3;initial catalog=FACE_RECOGNITION;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
  </connectionStrings>
```
## Sử dụng
```bash
Tài khoản admin: thedung6332@gmail.com
Password: admin
```

Cấu trúc file excel để import sinh viên
```
STT - Họ và tên - Tên Khoa - Tên Lớp
```
Lưu ý: Tên Khoa và Tên Lớp đã tồn tại trong cơ sở dữ liệu Khoa và Lớp


