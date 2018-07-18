## Example


If I have e.g. a new Kong server at http://kong.mycompany.com with no routes. It is a default setup with the admin port `8001` open to me.

I then make the following configuration file in a folder e.g. `c:\code\kongvergefiles\health.json`

````
{
  "Service": {
    "terminate": {
      "status_code": 200,
      "message": "All good"
    },
    "name": "healthcheck",
    "host": "www.example.com",
    "port": 80,
    "protocol": "http",
    "retries": 5,
    "connect_timeout": 60000,
    "write_timeout": 60000,
    "read_timeout": 60000,
    "Routes": [
      {
        "protocols": [ "http", "https" ],
        "paths": [ "/health/check" ],
        "regex_priority": 20,
        "strip_path": true
      }
    ]
  }
}
````

And I run Kongverge: `dotnet run --host kong.mycompany.com --input "c:\code\kongvergefiles"`
it will put the server into the state that the file specifies

The output is:
````
[14:43:49 INF] Getting existing services from kong.mycompany.com

[14:43:49 INF] Reading files from C:\Code\kongvergefiles
[14:43:49 INF]
Adding new service: "health"
[14:43:49 INF] Finished
````

If I run Kongverge a second time, Kongverge finds nothing to do, as the server is already in the desired state and the output is:
````
[12:02:20 INF] Getting existing services from kong.mycompany.com

[12:02:21 INF] Reading files from c:\code\kongvergefiles
[12:02:21 INF] Finished
````
