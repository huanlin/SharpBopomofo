# SharpBopomofo

![](https://img.shields.io/badge/.NET%20Standard-2.0-brightgreen.svg)

## 目的

提供一些用來把中文字轉成注音符號的 API。

## 動機

以往，我是透過 [IFELanguage](https://msdn.microsoft.com/en-us/library/windows/desktop/hh851778(v=vs.85).aspx) 來取得一串中文字的注音字根。然而，這種方法到了 Windows 10 似乎已經無法使用。

根據我的測試，即使 Windows 10 有安裝微軟注音輸入法，也無法使用 MSIME.Taiwan 來反查注音字根。奇怪的是，MSIME.China 依然可用，但 MSIME.China 只能取得拼音。

我在 GitHub 上面找不到我需要的工具，於是我決定造個輪子。

## 相依套件

 * [libchewing](https://github.com/chewing/libchewing)
 * [Serilog](https://serilog.net/)
 * [Jil](https://github.com/kevin-montrose/Jil)
 * [protobuf-net](https://github.com/mgravell/protobuf-net)
 * [NUnit](http://nunit.org/) 

## 授權

此專案是採用 LGPL 3.0 授權。

[libchewing](https://github.com/chewing/libchewing) 則是採用 GNU LGPL 2.1 授權。
