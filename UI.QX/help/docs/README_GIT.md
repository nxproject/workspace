# NX.Node - Git

NX.Node has built-in support for any Git-like product.  Let's first describe the
environment settings used:

Setting|Meaning
-------|-------
git_url|he URL for the Git repository (Default: https://github.com/)
git_product|The Git product ID (Default: NX.Node)
git_token|The Git access token
git_repo|The Git repository name.  This repository will be loaded at start time and any changes will be reloaded. (Example: MyRepo/SuperProject)

When a bee starts, the repository will be checked against any code loaded into the
**modules** folder and if there is a newer version, obtained, compiled and loaded
into the folder.  C# and Visual Basic code is supported.  When compiling both NX.Shared.dll
and NX.Engine.dll are available as using or Import.

Format|Meaning
------|-------
token@owner/project/module.ext|Full path.  The token is optional, as the git_token environment setting will be used
project/module.ext|The same owner as the git_repo environment setting, but a different project
/module.ext|The same owner and project as git_repo.  Not needed as module is loaded automatically
token@owner//module.ext|The same project as git_repo, but a different owner.  The token is optional, as the git_token environment setting will be used

[Back to top](/help/docs/README.md)
