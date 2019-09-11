md c:\SeleniumDrivers
copy ..\Libs\Selenium\*.exe c:\SeleniumDrivers\*.*
copy ..\Libs\*.bat c:\SeleniumDrivers\*.*
setx PATH "c:\SeleniumDrivers\;%path%;"
pause
