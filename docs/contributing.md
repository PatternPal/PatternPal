# Code conventions

We use [EditorConfig][2] to force the same basic code style within this project.

We use a few programming principles:

- DRY: Don’t Repeat Yourself. This prevents multiple pieces of code that have the same purpose.
- [SOLID](https://www.freecodecamp.org/news/solid-principles-explained-in-plain-english/): The code in this project
  should be easy to maintain and expand.
- KISS: Keep It Short, Stupid. The code is easy to understand and uses no unnecessary complexities.

The class names, method names and documentation will be written in English so that international parties can easily use
and participate in the development of the project. When writing documentation the [Microsoft Documentation Comments][3]
standard must be used. Inline comments have no conventions.

The code must conform to the [C# Coding Conventions][4].

Project versions use [Semantic Versioning][5]. To update the changelog we use [keepachangelog][6].

The CI/CD uses certain [commit syntax][7] for version bumping, using this syntax isn't required but is considered
helpful to correctly increase the project version.

Branch naming uses the following ebnf syntax:

```ebnf
branch_name    ::= default_branch | (branch_type, "/", [github_issue, "-"], description); 
default_branch ::= "master" | "develop"; 
branch_type    ::= "feature" | "fix" | "improvement"; 
github_issue   ::= "#", postive_number;
description    ::= letter, {letter | "-"};
```

Version control will be handled by GitHub. When adding an issue or pull request use the template that is provided and
that can be found within the repository.

## Testing

We mainly use NUnit to test our code. When NUnit can't be used we describe test cases and their desirable result. Only
critical functionalities are required to be tested. Meaning that the parts of the program that when broken would break
the rest of the program.

# Definition of Done

Before a new functionality can be added to the product it must satisfy these requirements:

- Unit tests will be written for all back-end components. All of the tests must succeed. The only exception is when a
  valid explanation is provided as to why the test fails.
- The code has comments (refer to # Code conventions)
- The code is committed to the correct branch.
- The changelog is adjusted with changes that have been made.
- A pull request has been made to development and it has been approved by two project members.
- The pull request has been merged to development.
- The Technical design has been updated if needed.
- The code must meet the code conventions. (refer to # Code conventions).
- The code must meet the acceptation criteria that have been assigned to the issue.
- When functional testing is needed, a test report will be written. The results will be registered in the test report.
- An issue on ready-to-review will be reviewed by someone other than the assignee(s). The provided feedback will be
  revised and incorporated.

[1]: https://www.sonarqube.org/

[2]: https://editorconfig.org/

[3]: https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/documentation-comments

[4]: https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

[5]: https://semver.org/spec/v2.0.0.html

[6]: https://keepachangelog.com/en/1.0.0/

[7]: https://github.com/conventional-changelog/conventional-changelog/tree/master/packages/conventional-changelog-angular
