# Introduction
The `mr` tool or `MultiRepo` is a tool to manage multiple repositories in a project.

## JSON manifest file
The tool is based on a JSON manifest file (`manifest.json`), which specifies the details of all repositories to manage under the project. The core details it specifies are:
* The location of the project's root directory relative to the manifest file directory. The root directory is a common base directory that contains all repository folders.
* The repository URL of each repo
* The relative path to the cloned directory of the repo.
* The type of each repo. This will allow multiple types of repos (Git, SVN, Mercurial, etc.) to be specified under a project and `mr` will be able to execute commands uniformly under all of them.

The tool can be run from any directory anywhere under the root directory.

The root directory contains a file called `.mr` which contains the relative location to the JSON manifest file. This file is not part of any repo, but is created when initializing a multi-repo setup or cloning an existing multi-repo setup.

## Extensibility
The tool provides an extensible system of commands that can be executed on one or more repositories.

Out of the box, the tool will have commands to execute VCS commands such as getting code (cloning), getting latest (pull), checking in (push), etc. These commands will be based on an extensible VCS provider system, which will allow these commands to be executed across multiple types of repos.

The extensibility will be based on NuGet packages. An extension is a class library that satisfies a specific contract and is distributed as a NuGet package. The package should be named with a specific convention to qualify as a valid `mr` extension.

```
MultiRepo.<Unique Package Name>
```

The JSON manifest file specifies the extensions that are installed for a particular project.