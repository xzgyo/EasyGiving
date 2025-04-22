# EasyGiving project (using C# language)
## Description
This app can generate "give" command for Minecraft easily. (command format: JE 1.20.1)  

## These functions will be added in the nearly future
1. Effective enchantment: It is impossible for a tool to accept all enchantments. Sometimes enchantment errors will cause bugs (for example, a crossbow cannot be fired when all enchantments are 255)  
2. Higher/Lower minecraft versions support: Iwill write different versions' give command into a `.db` file, and youcan generate command for 1.21.x even latest snapshot!  

## How to use (double click exe)
Download latest release, unzip, you will find exe file is at `publish\win-x64\EasyGiving.exe`.  

## Libraries used
1. Microsoft.Windows.SDK.BuildTools (10.0.26100.1742)  
2. Microsoft.WindowsAppSDK (1.7.250401001)  
3. System.Data.SQLite (1.0.119)  
4. Microsoft.NETCore.App (8.0.15)  
5. Microsoft.Windows.SDK.NET.Ref.Windows (10.0.19041.57)  

## Customize enchantments and tools
I'm using sqlite to save informations  
> `Tools.db`: Tool excel  
> `Enchantments.db`: Enchantments excel  