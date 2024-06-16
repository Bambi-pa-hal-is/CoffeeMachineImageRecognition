# CoffeeMachineImageRecognition

sudo systemctl daemon-reload
sudo systemctl enable coffeemachine.service


sudo systemctl start coffeemachine.service
sudo systemctl stop coffeemachine.service
sudo systemctl restart coffeemachine.service
sudo systemctl status coffeemachine.service


# Coffee Machine Image Recognition

This project is designed to perform image recognition for a coffee machine using a .NET application.

## Setting Up the Application to Run on Startup

To ensure the Coffee Machine Image Recognition application runs on startup, we use a systemd service. Follow these steps to manage the service.

### Creating the Systemd Service File

1. **Create the Service File:**

```sh
sudo nano /etc/systemd/system/coffeemachine.service


[Unit]
Description=Coffee Machine Image Recognition Service
After=network.target
StartLimitIntervalSec=500
StartLimitBurst=5

[Service]
WorkingDirectory=/home/pi/CoffeeMachineImageRecognition/CoffeeMachineImageRecognition
ExecStartPre=/bin/sh -c '/usr/bin/git pull >> /home/pi/coffeemachine.log 2>&1 || exit 0'
ExecStartPre=/bin/sh -c '/usr/local/bin/dotnet build >> /home/pi/coffeemachine.log 2>&1 || exit 1'
ExecStart=/usr/local/bin/dotnet run
Restart=always
User=pi
Environment=DOTNET_CLI_TELEMETRY_OPTOUT=1
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
