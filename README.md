361-Project
===========

Where all of the information for our project will be stored.


###TODO List for Final Acceptance Test
The acceptance test is MONDAY April 6. We need to get ourselves in gear. We have only a few weeks to get this done.

First it should be noted: EVERYONE FIX YOUR BROKEN CODE. Report bugs as you find them so those in charge can fix them. If you spot a bug while reading code, be kind and either point it out specifically where the bug is or fix it.

Benjamin :
- Make the entire user interface **WORK**
- ALL game actions should be **WORKING** by the end of next weekend (**Sunday March 22**). In other words: **backend testing is fully possible**.

Kevin :
- Finish the networking, lobbies and stats.
- A single game with 2 people (accounts/logins don't matter) should be completely playable over a network by the end of next weekend (**Sunday March 22**). This does not include stat tracking.

Nicholas :
- Finish MoveUnit
- TakeOverTile
- Tree Growth Phase
- MergeRegions

Emily :
- Destroy Village
- ConnectRegions
- Serialization

Alex :
- Implement changes (cannons, castles, HP on villages from cannon attack, cannon movement rules)



###Workflow
The workflow we will be using is described in the image below.

![github workflow](http://1.bp.blogspot.com/-ct9MmWf5gJk/U2Pe9V8A5GI/AAAAAAAAAT0/0Y-XvAb9RB8/s1600/gitflow-orig-diagram.png)

To accompany this, we will be using semantic versioning with slight modifications. The outline of semantic versioning can be found [here](http://semver.org/). 

Since we are not using an API, it will be adjusted as follows for software.

1. Major releases (Overhauls of systems, Multiplayer, Etc) will move a version from 1.0.0 to 2.0.0
2. Feature additions (New Skins For Characters, Etc) will move a version from 1.1.0 to 1.2.0
3. Bug fixes (Game no longer crashes when clicking enter) will move a version from 1.1.1 to 1.1.2

Note: When submitting, a description of the submission is **REQUIRED**
Include a brief description of all work done and more specifically, include whether you have made a bug fix, a feature addition, or an overhaul/addition of a system.

If you would like further clarification, please email [me](mailto://bhockley92@gmail.com).

###Teamwork & Project Practices

Since this course involves doing most of the design, planning, architecture etc much in advance, we will implement only a subset of the [Agile][1] method of development, that is:

- **Sprints** (2 weeks) and **sprint planning** meetings (at the beginning of the sprint).
- Work units divided into **Tasks** lasting < 8 hour, tracked on some kind of board with statuses (to do, ongoing, testing, done, ...)
- Weekly **Scrum meetings**, to talk about the work done and to do, and discuss any problems encountered or side-effects others should be aware of.
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

###Development Management

For ease of tracking development tasks and task assignment, we will be using Trello. Create an account [here](https://trello.com/signup) and [email](mailto://bhockley92@gmail.com) or message Ben with your Trello username (found by clicking on the nameplate in the top right, and then clicking on profile) to be added to the board.

[1]: http://en.wikipedia.org/wiki/Agile_software_development
[2]: http://en.wikipedia.org/wiki/Continuous_integration
