@echo off
chcp 65001 > nul
echo ==============================================
echo Dang kiem tra thay doi de push len GitHub...
echo ==============================================

git add .
set /p msg="Nhap noi dung commit (Enter de mac dinh 'Cap nhat du an'): "
if "%msg%"=="" set msg=Cap nhat du an

git commit -m "%msg%"
git push origin main

echo ==============================================
echo Hoan tat! Nhan phim bat ky de thoat.
echo ==============================================
pause > nul
