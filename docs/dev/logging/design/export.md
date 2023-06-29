# [Progsnap2](https://cssplice.github.io/progsnap2/) export tool
## Logging strategy
For efficiently processing and storing code snapshots, a strategy of storing file/directory diffs is currently employed by the PatternPal logging service. An overview of when actual CodeStates are logged can be found in the table below.

| **Event**       | **CodeState**     | **Remarks**                    |
|-----------------|-------------------|--------------------------------|
| `Project.Open`  | Full project      |                                |
| `Project.Close` | Full project      |                                |
| `File.Create`   | Created file only |                                |
| `File.Edit`     | Edited file only  |                                |
| `File.Rename`   | None              | Old and new filename is logged |
| `File.Delete`   | None              | Path of deleted file is logged |

All other events that are _currently_ logged by do not introduce any changes in the local CodeState, and therefore no included CodeState will be logged for their occurences. The export tool is able to determine what CodeState was actually relevant at the time a specific event was logged, based on previously stored CodeStates.



## Restore strategy
The role of the export tool now is two-fold:
1. Export the data stored in the PostgreSQL-database to a `.csv`-file.
1. Restore all full CodeStates. 

In order to do the latter, the following steps are performed. _Per individual session_, we...
1. Determine whether the event includes a Codestate
    - If it does, we include it in the export.
        - If the supplied CodeState was complete, no further work is done.
        - If the supplied CodeState was incomplete, we find the last full CodeState to obtain the missing source files for the partial CodeState.
    - If it doesn't, we check if there is a previous available CodeState for the session, and include that ID in the database.
2. For specific events (namely `File.Rename` and `File.Delete`) no CodeState is included, _but_ changes are still required in the exported CodeState. We leverage the data stored in the database to create a new CodeState reflecting those changes.


## Remarks
- The extension maintains a local mirror of the last successfully logged CodeState, in the form of file hashes for each project. For specific events, this mirror is used to determine whether a change actually occured compared to the previously logged CodeState. Whenever a look-up in this local mirror is unsuccessful, the complete CodeState is logged to make sure that we have an up to date copy in all relevant places. Whenever such an event unexpectedly occurs, the export tool will log a warning during export to inform the user of such an event.
- The export tool logs warning and errors whenever an unexpected event is encountered, such as an unavailable previous CodeState, an unavailable CodeState in the database, etc.. These errors are self-explanatory and include relevant metadata.
- CodeState-restoring for a specific session can only commence once a partial or full CodeState for that event is available. In other words: it might take a few events in a session before a CodeState actually comes available.