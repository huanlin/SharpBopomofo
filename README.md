# SharpBopomofo

## 目的

提供一些用來把中文字轉成注音符號的 API。

## 動機

我的另一個開源專案原本是透過 [IFELanguage](https://msdn.microsoft.com/en-us/library/windows/desktop/hh851778(v=vs.85).aspx) 來取得一串中文字的注音字根。然而，這種方法到了 Windows 10 似乎已經無法使用。

根據我的測試，在有安裝微軟注音的 Windows 10 環境上，已經無法使用 MSIME.Taiwan 了，而 MSIME.China 依然可用。然而 MSIME.China 只能取得拼音。

在我建立此專案時，並未在 GitHub 上面找到類似的 .NET 函式庫。於是我決定造這個輪子。我打算使用新酷音的資料檔來反查中文的注音字根。

## 授權

此專案是採用 LGPL 3.0 授權。

[libchewing](https://github.com/chewing/libchewing) 則是採用 GNU LGPL 2.1 授權。
