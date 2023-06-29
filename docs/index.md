# PatternPal

A Visual Studio extension that detects design patterns and helps users implement them.

## Requirements

- Visual Studio 2022

## Installation

You can download the extension installer from the [releases page]. Unzip the `PatternPal` folder and
run the included `PatternPal.Extension.vsix` installer. This will open a prompt to install
PatternPal into your Visual Studio 2022 instance. After the installation has succeeded, you can
access PatternPal from `View > Other Windows > PatternPal Extension`.

## Usage

You can open the main view of the extension through `View > Other Windows > PatternPal Extension`.

### Demo

The following video offers a walkthrough for the usage of PatternPal. 
https://www.youtube.com/watch?v=nl327X8Ixyk
[![PatternPal Demo]((https://img.youtube.com/vi/v=nl327X8Ixyk/0.jpg))](https://www.youtube.com/watch?v=nl327X8Ixyk?t=35s "PatternPal Demo")

### Step-By-Step
This module offers the flexibility for users to choose between two options: starting the implementation of a new design pattern or continuing their work on a design pattern they have previously worked on.

When the user opts to initiate a new implementation, users are presented with the option to add a file to the current solution or not, in either case, they are subsequently provided with a dialog that asks for the place to save this new file. This new file will then open in Visual Studio. Alternatively, if the user prefers to continue their work they are presented with a dialog to open the file(s) they wish to resume their implementation on. These files(s) are then opened in Visual Studio.

In both cases the user is presented with a description of the requirement and explanation of the step. The user should implement the requirement in the opened file and click the 'Check' button. Only when the implementation is correct does the 'next instruction' button become available.

[releases page]: https://github.com/PatternPal/PatternPal/releases/latest
