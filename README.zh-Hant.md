# EasyGiving (由C#語言編寫)
## 軟體效果
該軟體可以以生成我的世界give指令 (暫時只能生成Java版1.20.1的)  

## 計劃添加以下功能
1. 附有效的魔: 給一個工具啥魔都附容易出問題，有些魔咒互相排斥，還會導致工具無法正常使用 (比如給弩啥都附上255他就沒法發射了（？原因有待考證）)  
2. 支援更多版本: 將give的語法存在資料庫里，要生成別的版本就只要載入別的資料庫檔案（？功能疑似無法實現？寫死？用花括弧然後正則？換成腳本語言生成？）  

## 使用方法
下載最新的release，解壓，打開`publish\win-x64\EasyGiving.exe`  

## 使用的運行庫
1. Microsoft.Windows.SDK.BuildTools (10.0.26100.1742)
2. Microsoft.WindowsAppSDK (1.7.250401001)
3. System.Data.SQLite (1.0.119)
4. Microsoft.NETCore.App (8.0.15)
5. Microsoft.Windows.SDK.NET.Ref.Windows (10.0.19041.57)

## 自訂工具和附魔（？可以做多語言適配？ 文言文版適配？ ）
sqlite資料庫存儲
> `Tools.db`： 工具清單
> `Enchantments.db`： 附魔表