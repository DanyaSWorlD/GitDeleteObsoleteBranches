# The problem
When working with any git strategy which requires a new branch for every task/userstory/issue/bug/etc, this branches are deleted on remote when PR is closed. But to remove them on local machine, especially on windows ones, we need to do it manually. Some devs remo them one by one on each PR closed, and other, like me, are waiting for a certain amount to be collected and then remove them alltogether. 
> This tool finds and deletes local branches deleted on remote.

# The solution
Step one: Get all local branches
```
git branch -a
```
Step 2: Get branches deleted on a remote
```
git fetch --prune --dry-run
```
Step 3: Delete branches deleted on remote
```
git branch -D branchname
```

# Installation
Download executable or build it on your own
Replace a path with your path to executable and run
```
git config --global alias.dob '!C:/Git/GitDeleteObsoleteBranches.exe'
```
To use it open cmd, cd to your git repository folder and use
```
git dob
```
That's it!
