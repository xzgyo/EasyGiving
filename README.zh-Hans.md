# EasyGiving (由C#语言编写)
## 软件效果
该软件可以以生成我的世界give指令 (暂时只能生成Java版1.20.1的)  

## 计划添加以下功能
1. 附有效的魔: 给一个工具啥魔都附容易出问题，有些魔咒互相排斥，还会导致工具无法正常使用 (比如给弩啥都附上255他就没法发射了（？原因有待考证）)  
2. 支持更多版本: 将give的语法存在数据库里，要生成别的版本就只要加载别的数据库文件（？功能疑似无法实现？写死？用花括号然后正则？换成脚本语言生成？）  

## 使用方法
下载最新的release，解压，打开`publish\win-x64\EasyGiving.exe`  

## 使用的运行库
1. Microsoft.Windows.SDK.BuildTools (10.0.26100.1742)  
2. Microsoft.WindowsAppSDK (1.7.250401001)  
3. System.Data.SQLite (1.0.119)  
4. Microsoft.NETCore.App (8.0.15)  
5. Microsoft.Windows.SDK.NET.Ref.Windows (10.0.19041.57)  

## 自定义工具和附魔（？可以做多语言适配？文言文版适配？）
sqlite数据库存储  
> `Tools.db`: 工具列表  
> `Enchantments.db`: 附魔表  