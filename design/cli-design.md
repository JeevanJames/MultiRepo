# Command-line interface

## Initialize a multi-repo
This command initializes a new multi-repo. A multi-repo needs a JSON manifest definition file stored in either a separate repository or in an existing repository at a specific folder.

    TBD

## Clone an existing multi-repo
Clones an existing multi-repository project to the local machine.

```sh
mr clone <manifest repo url> [<manifest dir>] [--project-root-dir <dir>]
```

`<manifest repo url>` [Required]

The URL to the repository that contains the JSON manifest file.

`<manifest dir>` [Optional]

The relative directory in the repository where the JSON manifest file is located.

**Default**: The root directory in the repo.

`--project-root-dir, -r` [Optional]

The directory on the local system to setup the multi-repo project. This directory becomes the root directory for the project.

**Default**: The current directory.