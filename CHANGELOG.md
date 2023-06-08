# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres
to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [3.4.0] - 2023-06-08

## Added

- Log: Add a test suite for logging server by @JustNireon in https://github.com/PatternPal/PatternPal/pull/264
- Log: Implementation File.Edit by @WingToh in https://github.com/PatternPal/PatternPal/pull/267
- Log: Delete codestates folders if not existent in database by @JustNireon in https://github.com/PatternPal/PatternPal/pull/261
- Rec: Add priority sorting to RecognizerRunner by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/271
- Rec: Add prune all option to runner by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/273
- Ext: Error handling by @KnapSac in https://github.com/PatternPal/PatternPal/pull/265
- Rec: Added knockouts to priority sorting and added a test by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/276
- Docs: Add Priority sorting to recognizerrunner docs by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/277
- Feat: all object/method definitions for new step by step mode by @DanielvanDamme in https://github.com/PatternPal/PatternPal/pull/272
- ST: Add `Overrides` relation by @KnapSac in https://github.com/PatternPal/PatternPal/pull/278
- Log: Implemented diff-logging by @ovda96 in https://github.com/PatternPal/PatternPal/pull/274
- Rec: Implement strategy recognizer by @LvanWerven in https://github.com/PatternPal/PatternPal/pull/282
- Log: Refactoring + added FullCodeState-field by @ovda96 in https://github.com/PatternPal/PatternPal/pull/285
- Rec: Added priority notation by @LvanWerven in https://github.com/PatternPal/PatternPal/pull/286
- Log: Implemented File.Create by @WingToh in https://github.com/PatternPal/PatternPal/pull/283
- Rec: Added adapter by @rutgervincken in https://github.com/PatternPal/PatternPal/pull/288
- SBS: Implementation of new StepByStep module. by @DanielvanDamme in https://github.com/PatternPal/PatternPal/pull/284

## Changed

- Docs: Restructured documentation layout by @ovda96 in https://github.com/PatternPal/PatternPal/pull/258
- Rec: No longer hard-code Singleton Recognizer by @KnapSac in https://github.com/PatternPal/PatternPal/pull/266
- CLI: Remove .NET Framework dependency and use Spectre.Console by @KnapSac in https://github.com/PatternPal/PatternPal/pull/270
- Ext: Update to extensibility and add consent page by @JustNireon in https://github.com/PatternPal/PatternPal/pull/269
- Meta: Cleanup old code by @KnapSac in https://github.com/PatternPal/PatternPal/pull/275
- Rec: Fix adding result twice to to be pruned list by @KnapSac in https://github.com/PatternPal/PatternPal/pull/281
- Log: File.Edit only logging when enabled by @WingToh in https://github.com/PatternPal/PatternPal/pull/287

## [3.3.0] - 2023-05-24

## Added

- Rec: Implement `UsesCheck.Check` by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/183
- Rec: Implement `TypeCheck.Check` by @KnapSac in https://github.com/PatternPal/PatternPal/pull/182
- Rec: Added result of fieldCheck by @rutgervincken in https://github.com/PatternPal/PatternPal/pull/181
- Rec: methodcheck check function by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/187
- Rec: implemented ConstructorCheck.Check() by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/189
- Rec: implemented PropertyCheck.Check() by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/190
- Rec: added comments to the CheckResult file by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/192
- Feat/148/interfacecheck by @rutgervincken in https://github.com/PatternPal/PatternPal/pull/191
- Rec: Implement CheckCollection by @KnapSac in https://github.com/PatternPal/PatternPal/pull/194
- Rec: Expose Relations from SyntaxGraph by @KnapSac in https://github.com/PatternPal/PatternPal/pull/196
- Communication cycle and events by @WingToh in https://github.com/PatternPal/PatternPal/pull/180
- Rec: Implemented NotCheck and coherent Tests and helperMethods by @LvanWerven in https://github.com/PatternPal/PatternPal/pull/197
- Rec: Added typecheck to methodcheck and propertycheck by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/199
- Rec: Add notcheck to checks by @LvanWerven in https://github.com/PatternPal/PatternPal/pull/201
- Rec: Implemented relationchecks by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/198
- Rec: Implement ParameterCheck.Check() by @DanielvanDamme in https://github.com/PatternPal/PatternPal/pull/203
- Log: Implement compile error event by @WingToh in https://github.com/PatternPal/PatternPal/pull/207
- Rec: Move check implementation to NodeCheck by @KnapSac in https://github.com/PatternPal/PatternPal/pull/212
- Rec: Add overwrite of GetType4TypeCheck in FieldCheck by @CaptainJeroen in https://github.com/PatternPal/PatternPal/pull/220
- Log: Unpacking of Files on Logging server by @JustNireon in https://github.com/PatternPal/PatternPal/pull/214
- Rec: Run Class/Interface checks on all entities by @KnapSac in https://github.com/PatternPal/PatternPal/pull/222
- Feat/242/log-debug-program-event by @WingToh in https://github.com/PatternPal/PatternPal/pull/228
- Log: Add comments to entire logging server by @JustNireon in https://github.com/PatternPal/PatternPal/pull/204
- Rec: Implement computing results of recognizer by @KnapSac in https://github.com/PatternPal/PatternPal/pull/229
- Log: Updated Project logs (Project.Open, Project.Close) by @WingToh in https://github.com/PatternPal/PatternPal/pull/210
- BS+Ext: Use new Recognizer from extension by @KnapSac in https://github.com/PatternPal/PatternPal/pull/235
- Rec: Expanded the SyntaxGraph by creating relations between all IMembers and IEntities. by @CaptainJeroen in https://github.com/PatternPal/PatternPal/pull/237
- Log: Add Execution results to database column by @JustNireon in https://github.com/PatternPal/PatternPal/pull/231
- Log: Add recognize request to system by @JustNireon in https://github.com/PatternPal/PatternPal/pull/239
- Rec: Added pruning based on relations by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/243
- Log: Only log when DoLogData is True by @JustNireon in https://github.com/PatternPal/PatternPal/pull/253
- Log: Implemented project directory logging by @ovda96 in https://github.com/PatternPal/PatternPal/pull/246
- Rec: Fix Singleton recognizer by @KnapSac in https://github.com/PatternPal/PatternPal/pull/255
- Rec: Implemented type check pruning by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/257

## Changed

- Rec: Remove old recognizers by @KnapSac in https://github.com/PatternPal/PatternPal/pull/186
- Rec: Remove check builders and old checks by @KnapSac in https://github.com/PatternPal/PatternPal/pull/188
- Meta: Update packages by @JustNireon in https://github.com/PatternPal/PatternPal/pull/213
- Rec: Fixed the bug where constructors did not have all relations by @CaptainJeroen in https://github.com/PatternPal/PatternPal/pull/240
- Rec: Adding comments to syntaxgraph code by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/242
- Rec: Refactor Singleton by @rutgervincken in https://github.com/PatternPal/PatternPal/pull/245
- Meta: Rename Regonizers folder to Recognizers by @KnapSac in https://github.com/PatternPal/PatternPal/pull/254

## Meta

- Meta: Update installation instructions by @KnapSac in https://github.com/PatternPal/PatternPal/pull/193
- CI: Fix required checks blocking docs PRs by @KnapSac in https://github.com/PatternPal/PatternPal/pull/227
- Meta: Remove docker from msbuild production build by @JustNireon in https://github.com/PatternPal/PatternPal/pull/232
- Meta: Update README.md by @ovda96 in https://github.com/PatternPal/PatternPal/pull/233
- CI: Skip steps individually by @KnapSac in https://github.com/PatternPal/PatternPal/pull/234
- Meta: Delete Issue Templates by @ovda96 in https://github.com/PatternPal/PatternPal/pull/256
- Meta: Update .gitignore by @ovda96 in https://github.com/PatternPal/PatternPal/pull/259

## Docs

- Docs: Setup Github Pages with generated documentation by @KnapSac in https://github.com/PatternPal/PatternPal/pull/217
- Docs: Add recognizer explanation by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/223
- Docs: Add documentation syntax graph by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/225
- Meta: Add documentation strategy and singleton by @LvanWerven in https://github.com/PatternPal/PatternPal/pull/221
- Docs: Transfered recognizer documentation by @LvanWerven in https://github.com/PatternPal/PatternPal/pull/241
- Docs: Migrate Extension page from GH wiki by @KnapSac in https://github.com/PatternPal/PatternPal/pull/248
- Docs: Add Recognizer(Runner) documentation by @KnapSac in https://github.com/PatternPal/PatternPal/pull/247
- Log: Add information about logging to wiki by @JustNireon in https://github.com/PatternPal/PatternPal/pull/244

## [3.2.0] - 2023-04-20

### Added

- TestRunner: Support installing as dotnet tool by @KnapSac in https://github.com/PatternPal/PatternPal/pull/165
- Rec: Add new builders and checks approach by @KnapSac in https://github.com/PatternPal/PatternPal/pull/167
- Rec: Added interface check by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/169
- Rec: Add PropertyCheck(Builder) by @KnapSac in https://github.com/PatternPal/PatternPal/pull/168
- Tests: Added tests for communication with background service by @DanielvanDamme in https://github.com/PatternPal/PatternPal/pull/166
- Rec: Parameter check and builder by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/170
- Rec: Add `ICheckResult` by @KnapSac in https://github.com/PatternPal/PatternPal/pull/172
- Rec: Type check and builder by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/173
- Log: Initial setup of log server by @JustNireon in https://github.com/PatternPal/PatternPal/pull/156
- Rec: Implemented Modifier.Check by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/174
- Tests: modifier check tests by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/175
- Core: Start implementation of `ClassCheck` by @KnapSac in https://github.com/PatternPal/PatternPal/pull/176

### Improved

- Rec: Relations expanded with methods, syntaxtree expanded with semantic model (roslyn) by @CaptainJeroen in https://github.com/PatternPal/PatternPal/pull/178
- Rec: added priorities to builders and checks by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/177

### Meta

- Tests: Ignore failing tests for now by @KnapSac in https://github.com/PatternPal/PatternPal/pull/171

## [3.1.0] - 2023-04-04

### Added

- V23 h 89 collect test projects (singleton and strategy) by @ViciousDoormat in https://github.com/PatternPal/PatternPal/pull/155

### Improved

- Move step-by-step mode implementation to backend by @KnapSac in https://github.com/PatternPal/PatternPal/pull/154
- BS+Ext: Get supported recognizers from backend by @KnapSac in https://github.com/PatternPal/PatternPal/pull/161
- BS: Refactor and document RecognizerRunner and DesignPattern by @KnapSac in https://github.com/PatternPal/PatternPal/pull/162

### Fixed

- BS+Ext: Fix show all results button by @KnapSac in https://github.com/PatternPal/PatternPal/pull/160
- Ext: Fix and deny all warnings by @KnapSac in https://github.com/PatternPal/PatternPal/pull/164

### Meta

- Housekeeping: refactor/delete irrelevant projects by @ovda96 in https://github.com/PatternPal/PatternPal/pull/157
- Housekeeping: delete irrelevant directories by @ovda96 in https://github.com/PatternPal/PatternPal/pull/158
- Refactor: directory housekeeping by @ovda96 in https://github.com/PatternPal/PatternPal/pull/159

## [3.0.0] - 2023-03-22

### Added

- PatternPal.TestRunner: tool to run PatternPal on many projects at once.

### Improved

- Decouple recognizers from extension, run them in a background service instead.

## [2.2.2] - 2023-02-20

## [2.2.1] - 2022-01-21

### Fix

- Resource message error

## [2.2.0] - 2022-01-21

### Added

- New user control to display detected Design Patterns using expander controls
- The Logging Api and Test in the root project

### Improved

- Rewrote feedback text to one sentence that includes the file/node name
- Changed IDesign to PatternPal

### Removed

- TreeViewResults user control has been removed as it is has been replaced by ExpanderResults

### CI/CD

- Now tests multiple test projects
- SonarCloud

## [2.1.0] - 2022-01-12

### Added

- New view for selecting a design pattern to implement using step-by-step instructions
- New view for implementing a pattern using step-by-step instructions
- IDesign option page with option to turn logging data off
- Settings button on homescreen, which leads to the IDesign option page
- The possibility to define knock-out criteria in pattern recognizers
- Back button that is used for going back to the previous page
- OnBuildDone event to recognize build, rebuild, clean and deploy actions
- API client for the LoggingAPI

### Improved

- Github Ci/Cd, build extension also in develop and only test when code changed
- Design Pattern readability currently only improved Singleton

## [2.0.0] - 2021-12-15

### Added

- Bridge design pattern recognizer
- Home screen for the extension
- GitHub Ci/Cd
- Navigation between screen

### Improved

- Singleton Pattern Detector
  - Looks at generics so `Lazy<Singleton>` works now
- Refactored the SyntaxTree representation
  - Also moved this to a different module

### Changed

- Use GitHub actions instead of GitLab runner
- All screens have a common title

### Fixed

- Syntax Tree Generator used outdated syntax
- Compatibility with visual studio 2022

## [1.0.0] - 2021-12-01

The version developed by the previous team.

[Unreleased]: https://github.com/PatternPal/PatternPal/compare/v3.4.0...HEAD
[3.4.0]: https://github.com/PatternPal/PatternPal/compare/v3.3.0...v3.4.0
[3.3.0]: https://github.com/PatternPal/PatternPal/compare/v3.2.0...v3.3.0
[3.2.0]: https://github.com/PatternPal/PatternPal/compare/v3.1.0...v3.2.0
[3.1.0]: https://github.com/PatternPal/PatternPal/compare/v3.0.0...v3.1.0
[3.0.0]: https://github.com/PatternPal/PatternPal/compare/v2.2.2...v3.0.0
[2.2.2]: https://github.com/PatternPal/PatternPal/compare/v2.2.1...v2.2.2
[2.2.1]: https://github.com/PatternPal/PatternPal/compare/v2.2.0...v2.2.1
[2.2.0]: https://github.com/PatternPal/PatternPal/compare/v2.1.0...v2.2.0
[2.1.0]: https://github.com/PatternPal/PatternPal/compare/v2.0.0...v2.1.0
[2.0.0]: https://github.com/PatternPal/PatternPal/compare/v1.0.0...v2.0.0
[1.0.0]: https://github.com/PatternPal/PatternPal/releases/tag/v1.0.0
