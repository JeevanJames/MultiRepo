# MultiRepo
Extensible command-line utility to manage multi-repository projects

## Usages

```
mr init
  Initializes a new MultiRepo project. Use the repo commands to manage repositories under this project.

mr repo add|remove|list
	Adds, removes or lists the repositories for a project
	
mr repo tag add|remove|list
	Adds, removes or lists the tags of a repository under a project.

mr plugin install|uninstall|update|list
  Installs, uninstalls, updates or lists plugins that add extra commands.
  These plugins are NuGet packages that follow a naming convention.

mr run
	Runs a shell command under all repositories.

Global repo filtering options (all are mutually exclusive):
--just-me      - If you're currently in a repository directory (anywhere inside), this command will execute only for that repo.
--include      - Include one or more repos to execute a command for.
--exclude      - Excludes one or more repos to execute a command for.
--include-tags - Execute a command for repositories that have all the specified tags
--exclude-tags - Execute a command for repositories that have none of the specified tags
```
