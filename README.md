# Azure DevOps Build Light Monitor

[![.NET](https://github.com/JeffroeBodine/ado-build-light/actions/workflows/dotnet.yml/badge.svg)](https://github.com/JeffroeBodine/ado-build-light/actions/workflows/dotnet.yml)

A .NET 9 console application that monitors Azure DevOps pipeline builds and displays the status using colored LED lights on a Raspberry Pi. Perfect for creating a physical build status indicator for your development team.

## 🚦 What It Does

This application continuously monitors your Azure DevOps pipeline and provides visual feedback through colored LEDs:

- **🔴 Red Light**: Build failed
- **🟡 Yellow Light**: Build in progress
- **🟢 Green Light**: Build succeeded
- **🟠 Orange Light**: Build partially succeeded
- **💡 Lights Off**: Outside business hours or build cancelled

The monitor respects configurable business hours and only activates during your team's working schedule.

## ✨ Features

- **Real-time Monitoring**: Uses Azure DevOps REST API for immediate status updates
- **Business Hours Support**: Automatically turns off outside configured hours
- **Cross-Platform**: Runs on Raspberry Pi (with GPIO) or any system (with console output)
- **Configurable**: Easy JSON configuration for your organization and pipeline
- **Secure**: Uses Personal Access Tokens for authentication
- **Robust**: Automatic fallback to mock GPIO service on non-Pi systems

## 🛠️ Hardware Requirements

### For Raspberry Pi Setup

- Raspberry Pi (any model with GPIO pins)
- 3x LEDs (Red, Yellow, Green)
- 3x 220Ω resistors
- Breadboard and jumper wires
- Power supply for Raspberry Pi

### GPIO Pin Configuration

- **Red LED**: GPIO Pin 26
- **Yellow LED**: GPIO Pin 20
- **Green LED**: GPIO Pin 21

## 📋 Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Azure DevOps account with pipeline access
- Personal Access Token with Build (Read) permissions

## 🚀 Quick Start

### 1. Clone and Build

```bash
git clone https://github.com/JeffroeBodine/ado-build-light.git
cd ado-build-light
dotnet build
```

### 2. Configure Azure DevOps Access

1. **Create a Personal Access Token**:

   - Go to Azure DevOps → User Settings → Personal Access Tokens
   - Create new token with **Build (Read)** permissions
   - Copy the token value

2. **Find your Pipeline ID**:

   - Navigate to your pipeline in Azure DevOps
   - Copy the ID from the URL: `https://dev.azure.com/org/project/_build?definitionId=123`

3. **Configure the application**:

   ```bash
   cp appsettings.example.json appsettings.json
   ```

   Edit `appsettings.json` with your values:

   ```json
   {
     "AzureDevOps": {
       "Organization": "your-organization-name",
       "Project": "your-project-name",
       "PipelineId": "123",
       "PersonalAccessToken": "your-pat-token-here"
     },
     "BusinessHours": {
       "StartHour": 7,
       "EndHour": 18,
       "DaysOfWeek": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"]
     }
   }
   ```

### 3. Run the Application

```bash
dotnet run --project ADOBuildLight
```

### 4. Artifacts
   - You can also download the artifacts from the tag and run it using dotnet `ADOBuildLight.dll`

The application will:

- Detect if running on Raspberry Pi (uses real GPIO) or other systems (console output only)
- Check your pipeline status every minute
- Display colored lights based on build status
- Respect business hours configuration

## 🥧 Raspberry Pi Setup

### Hardware Wiring

Connect LEDs to your Raspberry Pi GPIO pins:

```
GPIO Pin 26 (Red)    → Red LED    → 220Ω Resistor → Ground
GPIO Pin 20 (Yellow) → Yellow LED → 220Ω Resistor → Ground
GPIO Pin 21 (Green)  → Green LED  → 220Ω Resistor → Ground
```

### Software Installation

1. **Install .NET 9 on Raspberry Pi**:

   ```bash
   curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0
   echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
   echo 'export PATH=$PATH:$HOME/.dotnet' >> ~/.bashrc
   source ~/.bashrc
   ```

2. **Clone and configure** (same as Quick Start steps 1-2)

3. **Run as a service** (optional):
   Create `/etc/systemd/system/ado-build-light.service`:

   ```ini
   [Unit]
   Description=Azure DevOps Build Light Monitor
   After=network.target

   [Service]
   Type=simple
   User=pi
   WorkingDirectory=/home/pi/ado-build-light
   ExecStart=/home/pi/.dotnet/dotnet run --project ADOBuildLight
   Restart=always
   RestartSec=10

   [Install]
   WantedBy=multi-user.target
   ```

   Enable and start:

   ```bash
   sudo systemctl enable ado-build-light
   sudo systemctl start ado-build-light
   ```

## ⚙️ Configuration Options

### Azure DevOps Settings

- `Organization`: Your Azure DevOps organization name
- `Project`: Project containing the pipeline
- `PipelineId`: Numeric pipeline ID to monitor
- `PersonalAccessToken`: PAT with Build (Read) permissions

### Business Hours Settings

- `StartHour`: Hour to start monitoring (24-hour format)
- `EndHour`: Hour to stop monitoring (24-hour format)
- `DaysOfWeek`: Array of days to monitor (e.g., `["Monday", "Tuesday", ...]`)

## 🔧 Development

### Project Structure

```
ado-build-light/
├── ADOBuildLight/           # Main application
│   ├── Interfaces/          # Service interfaces
│   ├── Models/             # Data models and configuration
│   ├── Services/           # Business logic services
│   └── Program.cs          # Application entry point
├── ADOBuildLight.Tests/    # Unit tests
├── appsettings.example.json # Configuration template
└── AZURE_DEVOPS_API.md    # API integration documentation
```

### Running Tests

```bash
dotnet test
```

### Building for Release

```bash
dotnet publish -c Release -r linux-arm --self-contained
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Troubleshooting

### Common Issues

**"Configuration file not found"**

- Ensure `appsettings.json` exists and is properly formatted
- Copy from `appsettings.example.json` and update values

**"Error creating GPIO service"**

- On non-Raspberry Pi systems, this is normal - the app uses console output instead
- On Raspberry Pi, ensure you have proper permissions to access GPIO

**"Error fetching data"**

- Verify your Personal Access Token has Build (Read) permissions
- Check that Organization, Project, and PipelineId are correct
- Ensure network connectivity to Azure DevOps

**LEDs not working on Raspberry Pi**

- Verify wiring connections match the GPIO pin configuration
- Check that resistors are properly connected
- Run with `sudo` if GPIO access is denied

### Getting Help

- Check the [Issues](https://github.com/JeffroeBodine/ado-build-light/issues) page
- Review [AZURE_DEVOPS_API.md](AZURE_DEVOPS_API.md) for API details
- Ensure your Azure DevOps setup matches the configuration requirements
