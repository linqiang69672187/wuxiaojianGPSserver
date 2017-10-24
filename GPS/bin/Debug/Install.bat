%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe GPS.exe
Net Start GPSserver
sc config GPSserver start=auto