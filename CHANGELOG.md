# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres
to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.2.1] - 2022-01-21

### Fix

-   Resource message error

## [2.2.0] - 2022-01-21

### Added

-   New user control to display detected Design Patterns using expander controls 
-   The Logging Api and Test in the root project

### Improved

-   Rewrote feedback text to one sentence that includes the file/node name
-   Changed IDesign to PatternPal

### Removed

-   TreeViewResults user control has been removed as it is has been replaced by ExpanderResults

### CI/CD

-   Now tests multiple test projects
-   SonarCloud

## [2.1.0] - 2022-01-12

### Added

-   New view for selecting a design pattern to implement using step-by-step instructions
-   New view for implementing a pattern using step-by-step instructions
-   IDesign option page with option to turn logging data off
-   Settings button on homescreen, which leads to the IDesign option page
-   The possibility to define knock-out criteria in pattern recognizers
-   Back button that is used for going back to the previous page
-   OnBuildDone event to recognize build, rebuild, clean and deploy actions
-   API client for the LoggingAPI

### Improved

-   Github Ci/Cd, build extension also in develop and only test when code changed
-   Design Pattern readability currently only improved Singleton

## [2.0.0] - 2021-12-15

### Added

-   Bridge design pattern recognizer
-   Home screen for the extension
-   GitHub Ci/Cd
-   Navigation between screen

### Improved

-   Singleton Pattern Detector
    -   Looks at generics so `Lazy<Singleton>` works now
-   Refactored the SyntaxTree representation
    -   Also moved this to a different module

### Changed

-   Use GitHub actions instead of GitLab runner
-   All screens have a common title

### Fixed

-   Syntax Tree Generator used outdated syntax
-   Compatibility with visual studio 2022

## [1.0.0] - 2021-12-01

The version developed by the previous team.

[Unreleased]: https://github.com/PatternPal/PatternPal/compare/v2.2.1...HEAD

[2.2.1]: https://github.com/PatternPal/PatternPal/compare/v2.2.0...v2.2.1

[2.2.0]: https://github.com/PatternPal/PatternPal/compare/v2.1.0...v2.2.0

[2.1.0]: https://github.com/PatternPal/PatternPal/compare/v2.0.0...v2.1.0

[2.0.0]: https://github.com/PatternPal/PatternPal/compare/v1.0.0...v2.0.0

[1.0.0]: https://github.com/PatternPal/PatternPal/releases/tag/v1.0.0
