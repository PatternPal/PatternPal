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

The following video offers a walkthrough for the usage of PatternPal: 
https://www.youtube.com/watch?v=nl327X8Ixyk

### Recognizer

This module allows the user to select the desired design patterns from a list to recognize for. The user has the option to either recognize patterns from their contemporary active document, or one of their projects. 

When the user clicks on the 'Analyze' button, the extension attempts to recognize the selected design patterns in the active document or selected project and displays its analysis. The analysis depicts for each recognized pattern a general score bar depicting how confident PatternPal is in the accuracy of the implementation of the given design pattern. A score for the implementation of each of the selected patterns' components is also given. All the requirements for each of the pattern's components are also displayed, together with an indicator whether the given requirement has been fulfilled.

### Step-By-Step
This module offers the flexibility for users to choose between two options: starting the implementation of a new design pattern or continuing their work on a design pattern they have previously worked on.

When the user opts to initiate a new implementation, users are presented with the option to add a file to the current solution or not, in either case, they are subsequently provided with a dialog that asks for the place to save this new file. This new file will then open in Visual Studio. Alternatively, if the user prefers to continue their work they are presented with a dialog to open the file(s) they wish to resume their implementation on. These files(s) are then opened in Visual Studio.

In both cases the user is presented with a description of the requirement and explanation of the step. The user should implement the requirement in the opened file and click the 'Check' button. Only when the implementation is correct does the 'next instruction' button become available. Whenever the user has completed the final step, a 'Home' button is displayed which returns the user back to the main menu.

[releases page]: https://github.com/PatternPal/PatternPal/releases/latest
