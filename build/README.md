### Building and Installing the Ringrem Debian Package
This README explains how to build and install the Ringrem .deb package manually. The package requires .NET runtime 10.0 or higher. You can change it since I did not use any .NET 10-specific features, but you have to change DEBIAN/control file too.

## 1. Prepare the Build Directory
Clone the repository:
```sh
git clone https://github.com/ProcessingBeet/ringrem.git
cd ringrem
```

Build project: 
```sh
dotnet publish ringrem.cli/ringrem.cli.csproj   -c Release   -r linux-x64   --self-contained true   /p:PublishSingleFile=true   -o out
```
Output will be in ringrem/out with binary ringrem.cli. Keep all files, including .dlls and configuration assets.

## 2. Prepare the Debian Package Structure
Copy your build output mimcing root structure of your machine in, treating ringrem directory as root:
```sh
cp -r out/* build/ringrem/usr/lib/ringrem/
```
## 3. Build the Debian Package
Run:
```sh
dpkg-deb --build build/ringrem
```
This will produce:
```sh
build/ringrem.deb
```
## 4. Install the Package
```sh
apt install ./build/ringrem.deb
```

Files will be installed to /usr/lib/ringrem/
A symlink /usr/bin/ringrem will be created
Systemd timer will be enabled and started automatically

## 5. Verify Installation
Check the CLI: 
```sh
ringrem --help
ringrem notify
```

Check the timer:
```sh
systemctl status ringrem.timer
systemctl list-timers | grep ringrem
```

## 6. Remove the Package (if you want to)
This removes the package, symlink, systemd timer, and optionally configuration files:
```sh
sudo apt purge ringrem
sudo apt autoremove
```
