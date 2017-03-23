# KerbClientProject
C# Kerberos client to retrieve token from KDC and push it to WSO2 IS kerberos grant

Initially, Kerberos realm should be setup using Winddows Domain Controller as KDC and Kerberos Client Windows instances.

1. This script should be placed in a Windows instance which is logged in as a Kerberos client to the created realm.
2. Another client should be registered in KDC with a Service Principle Name (SPN)
3. WSO2 IS/APIM should be hosted inside the network so that the .net client can be rechable.




