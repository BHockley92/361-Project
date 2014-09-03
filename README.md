361-Project
===========

Where all of the information from our project will be stored.

###Workflow
The workflow we will be using is described in the image below.

![github workflow](http://nvie.com/img/2009/12/Screen-shot-2009-12-24-at-11.32.03.png)

To accompany this, we will be using semantic versioning with slight modifications. The outline of semantic versioning can be found [here](http://semver.org/). 

Since we are not using an API, it will be adjusted as follows for software.

1. Major releases (Overhauls of systems, Multiplayer, Etc) will move a version from 1.0.0 to 2.0.0
2. Feature additions (New Skins For Characters, Etc) will move a version from 1.1.0 to 1.2.0
3. Bug fixes (Game no longer crashes when clicking enter) will move a version from 1.1.1 to 1.1.2

Note: When submitting, a description of the submission is **REQUIRED**
Include a breif description of all work done and more specifically, include whether you have made a bug fix, a feature addition, or an overhaul/addition of a system.

If you would like further clarification, please email [me](mailto://bhockley92@gmail.com).

###Teamwork & Project Practices

Since this course involves doing most of the design, planning, architecture etc much in advance, we will implement only a subset of the [Agile][1] method of development, that is:

- **Sprints** (2 weeks) and **sprint planning** meetings (once a week, at the beginning of the sprint).
- Work units divided into **Tasks** lasting < 8 hour, tracked on some kind of board with statuses (to do, ongoing, testing, done, ...)
- **Test-driven development** (i.e. write the tests before the bulk of your code).

Additionally, everyone will share the same **style rules** (TBD), so as to not make git have an epileptic fit due to code formatting, and the **Dev** branch will undergo [Continuous Integration][2], making sure no tests break, style is respected, and other features which will depend on language and tools used.

###Source Control

Benjamin's Workflow is great, so follow that one. Here's a few details on how you do it:

- **Master** branch is (ideally) bug free, containing tested and approved changes. This one rarely gets updated until major feature completion.
- **Dev** branch containing approved changes, but not acceptance tested (prone to bugs from pull requests not thoroughly tested, which happens).
- Branches for each **Task** (ideally a task is not strongly linked to another, but often this happens, giving a branch for a cluster of similar Tasks).
- When your Task is **Done** (see Definition of Done, below), you may:
  - Merge **Dev** into your branch.
  - Resolve any conflicts and run the project's tests.
  - If all tests pass, create a **Pull Request** to Dev.
  - Someone else must then review the changes before accepting and merging the pull request.

###Definition of Done

- All tests pass.
- The new code is covered by new test cases.
- If the End User can verify this functionality, it must be acceptance tested by another team member.
- The code conforms to style guidelines.

###Coding Practices

TBD

[1]: http://en.wikipedia.org/wiki/Agile_software_development
[2]: http://en.wikipedia.org/wiki/Continuous_integration
