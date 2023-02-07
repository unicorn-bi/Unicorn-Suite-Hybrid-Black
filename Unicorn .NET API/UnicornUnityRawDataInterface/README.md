# Unicorn Unity Raw Data Interface

The Unicorn Unity Raw Data Interface implements a basic data acquisition loop in Unity. It allows to acquire raw EEG data in realtime.

Prerequisites:
- Microsoft Windows 10 Pro, 64-bit, English
- Unicorn Suite Hybrid Black 1.18.00
- Microsoft Visual Studio
-- Microsoft .NET framework 4.7.1
- Unity 2019.1.0f2

[screenshot]: https://github.com/unicorn-bi/Unicorn-Suite-Hybrid-Black/blob/master/Unicorn%20.NET%20API/UnicornUnityRawDataInterface/Images/1.png "Screenshot"

1. Create a new Unity Project
2. Create a "Plugins" folder in the "Assets" folder
3. Copy "Unicorn.dll" and "UnicornDotNet.dll" from "C:\Users\<username>\Documents\gtec\Unicorn Suite\Hybrid Black\Unicorn DotNet\Lib" to the "Plugins" folder
4. Copy "UnicornUnity.cs" to the Assets folder
5. Attach the script to a gameobject in Unity <br><br>
![alt text][screenshot]