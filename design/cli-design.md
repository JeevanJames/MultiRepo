# Command-line interface

## Initialize a multi-repo
This command initializes a new multi-repo. A multi-repo needs a JSON manifest definition file stored in either a separate repository or in an existing repository at a specific folder.

```sh
mr init 
```

## Clone an existing multi-repo
Clones an existing multi-repository project to the local machine.

```sh
mr clone <manifest repo url> [<manifest dir>] [--project-root-dir <dir>]
```

`<manifest repo url>` [Required]

The URL to the repository that contains the JSON manifest file. This could be a separate repository just to hold the manifest file and related files, or an existing repository where the manifest is stored under some directory. In the latter case, use the `<manifest dir>` argument to specify the directory location.

`<manifest dir>` [Optional]

The relative directory in the repository where the JSON manifest file is located.

**Default**: The root directory in the repo.

`--project-root-dir, -r` [Optional]

The directory on the local system to setup the multi-repo project. This directory becomes the root directory for the project.

**Default**: The current directory.

## Get latest from repositories
Gets the latest from the selected repositories.

```sh
mr pull [--tags=<tag1>,<tag2>... | --exclude-tags=<tag1>,<tag2>... | --only-me]
```

