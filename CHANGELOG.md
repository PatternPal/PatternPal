# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres
to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [2.0.0] - 2021-12-15

### Added

-   Bride design pattern recognizer
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

[Unreleased]: https://github.com/super-idesign/designpatternrecognizer/compare/v2.0.0...HEAD

[2.0.0]: https://github.com/super-idesign/designpatternrecognizer/compare/b954bd6b9f7792c8854d57534c76143177a18a7e...v2.0.0
