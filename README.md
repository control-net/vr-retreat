![VR Retreat](docs/img/header.PNG)

<p align="center">
Challenge yourself to quit <a href="https://hello.vrchat.com/">VRChat</a> for a month while completing challenges and keeping track of yours and other's progress.
</p>

---

<p align="center">
  <a href="docs/LICENSE.md">
    <img src="https://img.shields.io/github/license/control-net/vr-retreat?style=for-the-badge">
  </a>
  <a href="#">
    <img src="https://img.shields.io/github/workflow/status/control-net/vr-retreat/.NET/main?style=for-the-badge">
  </a>
  <a href="#">
    <img src="https://img.shields.io/codacy/grade/d47c952003ed4824a19db230b5761b05/main?style=for-the-badge">
  </a>
  <a href="https://dot.net">
    <img src="https://img.shields.io/badge/made%20with-.NET%206-blueviolet?style=for-the-badge">
  </a>
</p>

---

## Build Dependencies

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

To verify your .NET 6 installation run the following in your terminal:

```bash
dotnet --version
```

## How to Build

Navigate into the `/src` directory of this project and run the following:

```bash
dotnet build
```

## Config

In order to run VRChat API tests or run the application against a real VRChat API you're going to need to create a configuration file with the appropriate credentials.

> :warning: You shouldn't use your real VRChat account, instead, create a new one

> :warning: This will not work if the account has 2FA enabled

### For Unit Tests

- Create a file and name it `TestConfig.json` in the `/src/VrRetreat.Tests/` directory.

### For Application Use

- Fill in your VRC credentials in `/src/VrRetreat.WebApp/VrChatConfig.json`

### Contents

The contents of your config file should be as follows:

```json
{
  "VrChatUsername": "Your-VrChat-Username-Here",
  "VrChatPassword": "Your-VrChat-Password-Here",
}
```

## Database & Migrations

If you're the [Visual Studio IDE](https://visualstudio.microsoft.com/vs/) you already have a local SQL Server ready.

On other platforms, you might need to install and configure an MSSQL server.

### Applying Migrations

Before you can start using the application, you should apply all migrations.

First install the dotnet EF tools:

```
dotnet tool install --global dotnet-ef
```

> 💡 The `--global` flag installs the tool for your whole system

Then change your directory into the WebApp and update the database:

```
cd src/VrRetreat.WebApp
dotnet ef database update
```

## hCaptcha

The registration view uses hCaptcha.

You might want to register your own [hCaptcha](https://www.hcaptcha.com/) account and fill in the `src/VrRetreat.WebApp/appsettings.json` hCaptcha section.
