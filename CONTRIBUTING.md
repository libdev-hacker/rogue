# Contributing to Rogue

Welcome weary travelers!

## Table of Contents
- [Contributing Yourself](#contributing-yourself)
- [Raising an Issue](#raising-an-issue)
- [Contributing Code](#contributing-code)
- [Views of Vibe-Coded Contributions](#views-on-vibe-coded-contributions)
- [Any Quesions?](#any-questions)

## Contributing Yourself

By this, I mean you should feel free to contribute your own time & online self to support Rogue. These contributions could be minor, such as mentioning the project to a friend over lunch or a colleague during your break, or even spreading the news over social media about how much you loved it! *(or how much it needs help from other kind & generous souls)*

I want Rogue to become a browser that everyone who uses the World Wide Web everyday can be proud of.

## Raising an Issue

Have an issue with the code? Found a bug? **AMAZING!**

Issue reports SHOULD BE of the following format: (template TBC)
- What is the issue?
- If applicable, how to reproduce the bug?
- Do you have an idea where in the code this occurred?
- Error report
   - Stack trace produced by the .NET runtime / Exception message
- Platform-specific information
   - OS with version / architecture
   - Version / Install date of Rogue

## Contributing Code

Anyone with a computer, text editor & good understanding of the C# programming language is more than welcome to submit PRs or even critique others **(constructively)** 

Every commit SHOULD aim to be small (ideally < 50-100 loc) & keep the amount of files changed at a minimum. However, large commits are acceptable if the changes made are inconsequential enough or likely to not cause lasting harm to the codebase (such as potential to introduce bugs / security issues)

Rogue takes advantage of the [Conventional Commit Message](https://www.conventionalcommits.org/en/v1.0.0/) format. In short, all git commit messages should take the form of:

```
<type>[optional scope]: <description>

[optional message]
```

- ```<type>``` - One word summary of what the commit is trying to achieve. Suitable types are:
   - ```feat``` - Adding / Modifying one of Rogue's Features
   - ```fix``` - Bug Fix / Patch
   - ```refactor``` - Changing a part of the code's layout or file structure
   - ```docs``` - Adding documentation to code (comments etc.)
   - ```build``` - Changes to the build system (modifying build parameters)
   - ```chore``` - Miscellaneous change that SHOULD NOT affect the program during compilation **nor** execution
- ```scope``` - String identifying which part of the code the patch affects
   - MUST BE either the root namespace (i.e. Rogue.HTML) or a single word summary of the component (i.e. JS, HTML, Window...) if a scope is included
- ```description``` - One short sentence describing the change
   - Please keep this concise, yet informative
- ```message``` - Section where more detail CAN BE added
   - If it is a large commit with multiple changes, then each change should be detailed here. If there is only 1-2 changes, then more information SHOULD BE given here for clarity
   - All pertinent information included SHOULD BE in the form of a markdown unordered list (cascading points starting with a hyphen)


## Views on Vibe-Coded Contributions

Unlike an increasing amount of projects nowadays, I, [libdev-hacker](https://github.com/libdev-hacker), the lead maintainer of Rogue, do not condone the submission of vibe-coded commits / Pull Requests to the Rogue browser's codebase & will not for the foreseeable future.

I do not take this stance because I do not see the benefit of LLMs in programming but because of recent (as of the time of writing this document) conversations about code provenance & licensing issues that arise from it. I do not wish Rogue to become a project that rests on the shoulders of other developer's code with no due respect for their indirect contribution or claim over the code they have written. 

This being said, I have no problem with developers / contributors using an LLM model as a tool to aid in the process of contributing to the browser project. I feel this way because I am one of the people (of what feels like a vanishing minority recently) that still see LLMs as a tool (in more ways than one *wink wink*) & if used in this manner, can still be used as an educational aide for people learning how to properly contirbute to open source programs.

## Any Questions?

Honestly, I'd either be very confused or greatly impressed if you came here without any questions but if you do, then feel at liberty to open a discussion thread an ask; no question is a bad one, especially when it comes to a project as complex as a web browser.