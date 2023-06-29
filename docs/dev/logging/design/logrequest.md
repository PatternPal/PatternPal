# LogRequest

[Progsnap2](https://cssplice.github.io/progsnap2/) is an extensive model designed for capturing and analyzing programming events during the development process. It allows developers and researchers to collect fine-grained data about programming actions, such as code edits, builds, and debugging activities. Progsnap2 supports various event types, and the fields associated with each event type can vary.

## Basic fields

When logging events in a logging system, there are certain basic fields that are commonly needed in every log entry. These fields provide essential information about the event being logged and help in organizing and analyzing the logged data. The basic fields typically include:

- **client_timestamp**: This argument represents the timestamp when the event occurred. It helps in tracking the sequence of events and can be used for chronological analysis. The format used is ISO 8601, i.e.: `2023-05-22T12:35:15+0000 2023-05-22T12:35:15+0000`.

- **session_id**: The session ID identifies a specific user session or interaction. It allows grouping related events together, providing context for the logged event. It is a GUID of the following format: `43f19fab-1d16-43a0-bcfc-21697911551e`.

- **subject_id**: The subject ID refers to the ID of the entity or object associated with the event. It helps in identifying the primary entity affected by the event. It also is a GUID as exemplified above.

- **event_id**: Each event logged should have a unique event ID. It serves as a reference for identifying and retrieving specific events from the logs. It also is a GUID as exemplified above.

- **project_id**: The project ID identifies the project associated with the event. It helps in grouping events by project and provides context for the logged event. This is the csproj file name.

- **event_type**: Indicates the type of event.
    1: `Session.Start`: Marks the start of a work session.
    2:  `Session.End`: Marks the end of a work session.
    3: `Project.Open`: Indicates that a project was opened.
    4: `Project.Close`: Indicates that a project was closed due to an explicit user or system action. Data consumers should be prepared to handle cases where `Project.Open` is not terminated by an explicit `Project.Close`.
    5: `File.Create`: Indicates that a file was created.
    6: `File.Delete`: Indicates that a file was deleted.
    7: `File.Open`: Indicates that a file was opened.
    8: `File.Close:` Indicates that a file was closed.
    10: `File.Rename`: Indicates that a file was renamed.
    12: `File.Edit`: Indicates that the contents of a file were edited.
    13: `Compile`: Indicates an attempt to compile all or part of the code.
    14: `Compile.Error`: Represents a compilation error and its associated diagnostic.
    15: `Compile.Warning`: Represents a compilation warning and its associated diagnostic.
    18: `Debug.Program`: Indicates a debug execution of the program and its associated input and/or output.
output.
was viewed.
    23: `X-RecognizerRun` Indicates that a recognizer was run.
    24: `X-StepByStepStep` Indicates that a step-by-step step was done.
<br></br>
- **tool_instances**: Information about the tool instances used during the event, in the following format: `VS IDE 2022; PP 2.6.2`.

---

<!---
TODO Proofread
-->

## EventType Specific

Some events require extra columns and can be omitted in other events. All events currently in use will be mentioned in this paragraph

### Project.Open (3)

The Project.open event is used to log when a project is opened in Visual Studio (VS). It captures the following extra information:

- **data**: Entire copy of the codestate of the current project. This will be passed along as a base64-encoded zipfile.
  
### Project.close (4)

The Project.close event is used to log when a project is closed in Visual Studio (VS). It has similar fields as the Project.open event.

### File.Create (5)

The File.create event is used to log when a file is created in Visual Studio (VS). It captures the following extra information:

- **code_state_section**: The name of the file that was created.
  - *example*: `"BridgePattern\\Program.cs"`
- **data**: The contents of the file that was created.
  - *example*: `"using System; namespace BridgePattern { class Program { static void Main(string[] args) { Console.WriteLine("Hello World!"); } } }"`

### File.Delete (6)

The File.delete event is used to log when a file is deleted in Visual Studio (VS). It captures the following extra information:

- **code_state_section**: The name of the file that was deleted.
  - *example*: `"BridgePattern\\Program.cs"`

### File.Rename (10)

The File.rename event is used to log when a file is renamed in Visual Studio (VS). It captures the following extra information:

- **code_state_section**: The name of the file that was renamed.
  - *example*: `"BridgePattern\\Program.cs"`
- **old_file_name**: The old name of the file that was renamed.
  - *example*: `"BridgePattern\\Program1.cs"`

### File.Edit (12)

The File.edit event is used to log when a file is edited in Visual Studio (VS). It captures the following extra information:

- **code_state_section**: The name of the file that was edited.
  - *example*: `"BridgePattern\\Program.cs"`
- **data**: The contents of the file that was edited.
  - *example*: `"using System; namespace BridgePattern { class Program { static void Main(string[] args) { Console.WriteLine("Hello World!"); } } }"`

### Compile (13)

The Compile event is used to log the compilation process of code within an application. The `event_id` uniquely identifies this compilation event, and it should be saved in case of any related Compile.error or Compile.warning events.

### Compile.warning (14)

The Compile.warning event is logged when a warning is generated during the compilation process. It includes information such as where the warning ocurred and what the warning was.

- **code_state_section**: Mentions in what file the error occurred.
  - *example*: `"BridgePattern\\Program.cs"`
- **compile_message_data**: Message the compiler sent as a warning.
  - *example*: `"Variable is unused"`

- **compile_message_type** Level of the warning
  - *example*: `"vsBuildErrorWarningHigh"`

- **source_location** : The location of the warning within the file.
  - *example*: `"Text:15"`

- **parent_event_id**: Event ID of the Compile event that is related this warning.
  - *example*: `43f19fab-1d16-43a0-bcfc-21697911551e`

### Compile.error (15)

The Compile.error event is logged when an error occurs during the compilation process.

- **code_state_section**: Mentions in what file the error occurred.
  - *example*: `"BridgePattern\\Program.cs"`

- **compile_message_data**: Message the compiler sent as a warning.
  - *example*: `"; expected"`

- **compile_message_type** Level of the warning
  - *example*: `"vsBuildErrorErrorHigh"`

- **source_location** : The location of the warning within the file.
  - *example*: `"Text:15"`

- **parent_event_id**: Event ID of the Compile event that is related this error.
  - *example*: `43f19fab-1d16-43a0-bcfc-21697911551e`

### Debug.Program (18)

The Debug.program event is used to log the execution of code through an application's debugging functionality.

- **ExecutionResult**: ExecutionResult is a field used in the Debug.program event. It represents the result of code execution during the debugging process.
  - *example*: `1`
  - Enum values:
    1. Unknown
    2. Success
    3. Timeout
    4. Error
    5. Test failed

### X-RecognizerRun (23)

The X-RecognizerRun event is used to log the execution of a recognizer. It includes information such as the recognizer configuration and the recognizer result.

- **RecognizerConfig**: RecognizerConfig is a field used in the X-RecognizerRun event. It currently only represents the name of the recognizer that was run.
  - *example*: `[ "RECOGNIZER_ADAPTER", "RECOGNIZER_BRIDGE", "RECOGNIZER_DECORATOR", "RECOGNIZER_FACTORY_METHOD", "RECOGNIZER_OBSERVER", "RECOGNIZER_SINGLETON", "RECOGNIZER_STATE", "RECOGNIZER_STRATEGY" ]`

- **RecognizerResult**: RecognizerResult is a field used in the X-RecognizerRun event. It represents the result of the recognizer that was run.
  - *example*:

    ```json
    {
        "recognizer": "RECOGNIZER_SINGLETON",
        "className": "no matched node",
        "results": [
            {
                "feedbackType": "FEEDBACK_CORRECT",
                "subCheckResults": [
                    {
                        "feedbackType": "FEEDBACK_CORRECT",
                        "feedbackMessage": "Found class 'SingleTonTestCase01'",
                        "subCheckResults": [
                            {
                                "feedbackType": "FEEDBACK_CORRECT",
                                "feedbackMessage": "Found the required checks for: SingleTonTestCase01.",
                                "subCheckResults": [
                                    {
                                        "feedbackType": "FEEDBACK_CORRECT",
                                        "subCheckResults": [
                                            {
                                                "feedbackType": "FEEDBACK_CORRECT",
                                                "feedbackMessage": "Found constructor: SingleTonTestCase01().",
                                                "subCheckResults": [
                                                    {
                                                        "feedbackType": "FEEDBACK_CORRECT",
                                                        "feedbackMessage": "Found Uses relations for SingleTonTestCase01()."
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    {
                                        "feedbackType": "FEEDBACK_CORRECT"
                                    }
                                ]
                            }
                        ]
                    }
                ]
            }
        ]
    }
    ```
