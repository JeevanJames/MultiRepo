# Command-line interface

## Common CLI options

### For all commands
`--help, -h`

Displays help for the specified command.

### Root level options
`--version`

Displays the version of the tool.

### Repo options
Commands that operate on the repos of an existing project.

`--only-me`

Includes only the repo under the currentr diretory. If the current directory is not part of a repo, an error is displayed.

`--tags <tag1> <tag2> ...`

Includes only repos that have all the specified tags.

`--exclude-tags <tag1> <tag2> ...`

Excludes repos that have any of the specified tags.

## Initialize a multi-repo
This command initializes a new multi-repo. A multi-repo needs a JSON manifest definition file stored in either a separate repository or in an existing repository at a specific folder.

```sh
mr init <project-root-dir> <manifest dir> [--discover] [--branch <branch name>]
```

`<project-root-dir>` [Required]

The directory to setup the new multi-repo project in. This becomes the root directory of the project.

`<manifest dir>` [Required]

The relative directory from the project root directory to contain the manifest.

`--discover` [Optional]

If specified, attempts to locate repositories under the `<project-root-dir>` and add them to the project.

`--branch` [Optional]

The branch of the repository to checkout for the manifest details.

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
mr pull [repo options]
```
