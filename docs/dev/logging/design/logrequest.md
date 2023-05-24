# LogRequest

[Progsnap2](https://cssplice.github.io/progsnap2/) is an extensive model designed for capturing and analyzing programming events during the development process. It allows developers and researchers to collect fine-grained data about programming actions, such as code edits, builds, and debugging activities. Progsnap2 supports various event types, and the fields associated with each event type can vary.

## Basic fields

When logging events in a logging system, there are certain basic fields that are commonly needed in every log entry. These fields provide essential information about the event being logged and help in organizing and analyzing the logged data. The basic fields typically include:

- **client_timestamp**: This argument represents the timestamp when the event occurred. It helps in tracking the sequence of events and can be used for chronological analysis. The format used is ISO 8601, i.e.: `2023-05-22T12:35:15+0000 2023-05-22T12:35:15+0000`.

- **session_id**: The session ID identifies a specific user session or interaction. It allows grouping related events together, providing context for the logged event. It is a GUID of the following format: `43f19fab-1d16-43a0-bcfc-21697911551e`.

- **subject_id**: The subject ID refers to the ID of the entity or object associated with the event. It helps in identifying the primary entity affected by the event. It also is a GUID as exemplified above.

- **event_id**: Each event logged should have a unique event ID. It serves as a reference for identifying and retrieving specific events from the logs. It also is a GUID as exemplified above.

- **event_type**: Indicates the type of event.

- **tool_instances**: Information about the tool instances used during the event, in the following format: `VS IDE 2022; PP 2.6.2`.

## EventType Specific

Some events require extra columns and can be omitted in other events. All events currently in use will be mentioned in this paragraph

### Project.open (3)

The Project.open event is used to log when a project is opened in Visual Studio (VS). It captures the following extra information:

- **data**: Entire copy of the codestate of the current project. This will be passed along as a base64-encoded zipfile.
  
### Project.close (4)

The Project.close event is used to log when a project is closed in Visual Studio (VS). It has similar fields as the Project.open event.

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
