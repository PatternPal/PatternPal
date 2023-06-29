# Overview

To decouple the extension(s) from the recognizers, the background process was created. This process
communicates with the extension, and this is where the actual work happens. The extension is
intentionally kept as simple as possible, it should only sent requests to the background process and
display the results it receives.

When the extension creates a logging event, it also does not directly sent it to the remote logging
service. Instead, it sends the event to the background process, where some additional processing
happens before the event is forwarded to the remote logging service. This again reduces the amount
of work which needs to happen in the extension, making it easier to add support for other IDEs.
