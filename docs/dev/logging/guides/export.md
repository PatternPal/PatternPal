# Exporting the dataset
The data logged by PatternPal can be exported in [Progsnap2](https://cssplice.github.io/progsnap2/)-format using the export tool. Platform-specific builds of this tool are included in the release; it is advised to run the export tool locally on the machine that actually stores the dataset, although it can also be used over a network share.

## Usage
1. Create a `config.json`-file with relevant details; an example can be found [here](https://github.com/PatternPal/PatternPal/blob/18b6262e3dd6a879b8afd653016539e3532d7947/PatternPal/PatternPal.ProgSnapExport/exampleConfig.json).
2. Run the export tool: available flags can be found below.

```console
USAGE:
    PatternPal.ProgSnapExport.dll [configFile] [exportDirectory] [OPTIONS]

ARGUMENTS:
    [configFile]         Path to config file; defaults to "./config.json"
    [exportDirectory]    Path to export directory; defaults to "./export"

OPTIONS:
                       DEFAULT                                                  
    -h, --help                    Prints help information                       
    -s, --start                   Specifies the starting point of the time      
                                  interval to be exported. Format: "YYYY-MM-DD  
                                  HH:MM:SS"                                     
    -e, --end                     Specifies the ending point of the time        
                                  interval to be exported. Format: "YYYY-MM-DD  
                                  HH:MM:SS"                                     
    -f, --force                   Overwrites the contents of the export         
                                  directory (if any)                            
        --csv-only                Does not include the CodeStates in the export 
        --log-level    Warning    Sets the log level of what is logged to the   
                                  logfile. Format: INFO|WARNING|ERROR (default: 
                                  "WARNING")                                    
    -v, --verbose                 Prints all messages to the console            
    -q, --quiet                   Surpresses all messages on the console  
```

## Remarks
Some tweaking is still necessary to create exports that can immediately be opened by [Progsnap2](https://cssplice.github.io/progsnap2/)-viewers such as [this project](https://github.com/Programming-Steps-Working-Group-2022/progsnap2-browser).