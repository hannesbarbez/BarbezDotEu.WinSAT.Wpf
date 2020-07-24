# Experience Index System Assessment Tool (WPF)
![Experience Index System Assessment Tool (WPF) screenshot - finished](https://barbez.eu/wp-content/uploads/2020/07/image-1.png)

The Experience Index System Assessment Tool is a replacement for the graphical user interface (GUI) of the Windows System Assessment Tool (WinSAT) originally introduced in Windows Vista, but removed from Windows 8.1 and later versions.

This app initiates the original WinSAT test, which assesses performance characteristics and capabilities of your hardware, reporting them in the form of a Windows Experience Index score. The WEI includes five sub scores measuring respectively processor, memory, desktop graphics, 3D graphics, and primary disk. The base score is equal to the lowest of the sub scores and is not an average of the sub scores.

*An improved and more extensive UWP version is available from the Microsoft Store at https://www.microsoft.com/p/experience-index-system-assessment-tool/9mt9h8ptp897*

# Usage
Start the Experience Index System Assessment Tool. For optimal and accurate results, leave it running uninterrupted by not using any other programs until all scores are calculated and appear on screen. The app shows a "working" background animation while all necessary System Assessment tests are being run. This can take several minutes. When finished, the Windows Experience Index will be shown. 

![Experience Index System Assessment Tool (WPF) screenshot - in progress](https://barbez.eu/wp-content/uploads/2020/07/image.png)

# Features
- Substitutes the graphical user interface (GUI) of the Windows System Assessment Tool (WinSAT) on Windows 8 and newer;
- Includes five sub scores measuring respectively processor, memory, desktop graphics, 3D graphics, and primary disk.

# Author
Hannes Barbez (www.barbez.eu), between 2017-2020.

# Technicalities
C#, WPF, PowerShell, XAML, multi-threading, WinSAT
