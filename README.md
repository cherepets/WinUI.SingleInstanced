# WinUI.SingleInstanced

[![Build](https://github.com/cherepets/WinUI.SingleInstanced/actions/workflows/build.yml/badge.svg)](https://github.com/cherepets/WinUI.SingleInstanced/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/WinUI.SingleInstanced.svg)](https://www.nuget.org/packages/WinUI.SingleInstanced)

Zero-setup single instancing for WinUI 3 applications.
Based on article: [Making the app single-instanced](https://blogs.windows.com/windowsdeveloper/2022/01/28/making-the-app-single-instanced-part-3/) by Jingwei Zhang.

## Usage

Install the package in your WinUI 3 application project, the application immediately becomes single-instanced.
If you need to handle activation, subscribe to Program.Activated event:

```csharp
namespace MyApp;

public partial class App
{
    public App()
    {
        Program.Activated += OnActivated;
        InitializeComponent();
    }

    private void OnActivated(object? sender, AppActivationArguments args)
    {
        // Handle launch, protocol, file, or other activation arguments here.
    }
}
```

## How it works

Package installation adds .target file to disable generation of default Main() method.
Source generator creates Program.cs with Main() method that handles the activation.
