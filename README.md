<h1 align="center">
  <br>
  <img src="https://user-images.githubusercontent.com/17230330/212778015-3e7516bc-9092-4dd9-bbc0-2f41de377418.png" alt="Delfinovin" width="150">
  <br>
  <b>Delfinovin (Dell-fee-no-vin)</b>
</h1>

<p align="center">
 Delfinovin is an application written in C# to allow for the use of Gamecube controllers on the Windows platform. It creates virtual Xbox controllers to be used in most, if not all modern games and is written with compatibility in mind, supporting most if not all standard Gamecube adapters. Delfinovin is extremely low latency and provides a number of settings to adjust and modify the user experience. Started in January 2021, Delfinovin began as a command line project and has since evolved into a .NET WPF GUI-based application.
</p>

<p align="center">
    <a href="https://www.paypal.me/Struggleton/">
        <img src="https://cdn.rawgit.com/twolfson/paypal-github-button/1.0.0/dist/button.svg" width="155" alt="">
    </a>
    <a href="https://www.patreon.com/Struggleton">
        <img src="https://c5.patreon.com/external/logo/become_a_patron_button@2x.png" width="150" alt="">
    </a>
 <a href="https://ko-fi.com/Struggleton">
        <img src="https://uploads-ssl.webflow.com/5c14e387dab576fe667689cf/61e11d430afb112ea33c3aa5_Button-1-p-500.png" width="235" alt="">
    </a>
</p>


<p align="center">
    <a href="https://somsubhra.github.io/github-release-stats/?username=Struggleton&repository=Delfinovin">
        <img src="https://img.shields.io/github/downloads/Struggleton/Delfinovin/total" alt="">
    </a>
    <a href="https://www.gnu.org/licenses/gpl-3.0">
        <img src="https://img.shields.io/badge/License-GPLv3-blue.svg" alt="">
    </a>
    <img src="https://user-images.githubusercontent.com/17230330/212779824-ecfaab07-c912-4944-a68e-d1f51908ef22.png" alt="">
</p>

## Usage
Delfinovin is an application that utilizes the **.NET Core framework.** In order to use the application you will need to install the **.NET 7 Desktop Runtime.** Delfinovin also uses **ViGEmBus and the WinUSB driver** to create virtual Xbox controllers and interact with the Gamecube adapters respectively. 

Please see the [setup guide](https://github.com/Struggleton/delfinovin-docs/wiki/Setup-Guide) on Delfinovin's wiki on how to setup up the application, along with utilizing Delfinovin's various features.

Features that Delfinovin has includes: 
- Button Remapping
- Hotkey Support
- Input Display
- Controller profiles
- Input Recording / Playback
- ... along with a number of settings to improve the user experience.

Please check the [wiki for a more comprehensive list of features and how to use them!](https://github.com/Struggleton/delfinovin-docs/wiki)

## Credits
### Delfinovin was mainly programmed by myself (Struggleton!), but could not have come to fruition without help from these people:

- [Benjamin "Nefarius" Höglinger-Stelzer](https://github.com/nefarius) for their work on [Utilties.DeviceManagement](https://github.com/nefarius/Nefarius.Utilities.DeviceManagement), [Utilties.GithubUpdater](https://github.com/nefarius/Nefarius.Utilities.GitHubUpdater), and most notably [ViGEmBus](https://github.com/ViGEm/ViGEmBus) and [ViGEm.NET](https://github.com/ViGEm/ViGEm.NET), and giving me a lot of advice on controllers
- [Narr the Reg](https://github.com/german77) from the Yuzu team for their continued support + advice on Gamecube controller input and adapters
- [Adrian "Federerer" Gaś](https://github.com/Federerer) for their work on the [Notification.Wpf](https://github.com/Federerer/Notifications.Wpf) library
- [Ryan "ryanvs" Slooten](https://github.com/ryanvs) for their work on [DropDownButtonBehavior](https://gist.github.com/ryanvs/8059757)
- [Matt "ms4v" Cunningham](https://bitbucket.org/elmassivo/) for their original foray into Gamecube adapter work with [GCNUSBFeeder](https://bitbucket.org/elmassivo/gcn-usb-adapter/src/master/) and vJoy
- [Gabriel "gemarcano" Marcano](https://github.com/gemarcano) for providing [essential documents on how the Gamecube adapter sends data](https://github.com/gemarcano/GCN_Adapter-Driver/tree/master/docs)
- [Ben "BarkingFrog" K.](https://twitter.com/Barking_Frogs) for their assistance writing the original calibration function
- [Pete "pbatard" Batard](https://github.com/pbatard) for their work on [Zadig](https://zadig.akeo.ie/)/[libwdi](https://github.com/pbatard/libwdi) which makes setting up WinUSB much more convenient
- [The LibUsbDotNet Team](https://github.com/LibUsbDotNet/) for their work on the [LibUsbDotNet](https://github.com/LibUsbDotNet/LibUsbDotNet) library
- [The WinUSB Team at Microsoft](https://github.com/microsoft) for their work providing the WinUSB drivers
- [The Dolphin Team](https://github.com/dolphin-emu) for their work on [Dolphin](https://github.com/dolphin-emu/dolphin) and research on Gamecube adapters
- [The Ryujinx Team](https://github.com/Ryujinx) for their work on [Ryujinx](https://github.com/Ryujinx/Ryujinx) and inspiration for Delfinovin's input system
- [The MaterialDesignInXAML Team](https://github.com/MaterialDesignInXAML) for their work on the [Material Design in XAML toolkit](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit), providing a great framework for UI design
- [The Octokit Team](https://github.com/octokit) for their work creating [Octokit](https://github.com/octokit/octokit.net), a great library for interacting with Github
- [OpenClipArt](https://openclipart.org/) for providing the original vector for the design behind the GamecubeDialog
- [Zacksly](https://twitter.com/_Zacksly) for their work creating the button icons used in the application
- Finally, [Fivee](https://twitter.com/5ivee_), [Noir](https://twitter.com/nowiyre) & [NurseJoy](https://twitter.com/nursejoychan) for their continued support behind the project.

If you feel that you've contributed to the project and would like to be added to the list of credits, please contact me [@Struggleton on Twitter](https://twitter.com/Struggleton). 

## Contributing / Event Info
**If you'd like to contribute to the Delfinovin project or use it at large events, [please navigate to the Delfinovin wiki page for more information on getting in contact!](https://github.com/Struggleton/delfinovin-docs/wiki/Contributing-&-Event-Info)**