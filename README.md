<div align="center">
    <h1>Rogue</h1>
    <h3>The Experimental Web Browser (Written in C#) That Your Aunts & Uncles at <s>G00gle</s> & M*cr*<s>slo...</s> Warned You About</h3>
    <p>... and the one our friends @ Mozilla & Ladybird look at with great curiosity</p>
</div>

![Static Badge](https://img.shields.io/badge/License-MIT-blue?style=plastic) ![GitHub Repo stars](https://img.shields.io/github/stars/libdev-hacker/rogue?style=plastic&logo=github) <a href="https://github.com/libdev-hacker/rogue/issues"><img alt="GitHub Issues or Pull Requests" src="https://img.shields.io/github/issues-raw/libdev-hacker/rogue"></a> ![GitHub repo size](https://img.shields.io/github/repo-size/libdev-hacker/rogue)

## About Me
Believe it or not, this project started out as a simple coursework assignment for my CS course! I chose to create Rogue purely as a way to make my friends look at me as if I was The Joker's right-hand man. But now I see it as a way to bring the Internet back into the hands of everyone who may or may not find themselves <i>(begrudgingly)</i> reading all of this text right now.

<b> TL;DR: I'm insane! </b>

**Serious Part**: In all seriousness, I created this project to show off the true power of C# in order to create a truly cross-platform solution for <i>surfing the web</i>.

## Getting Started
To start your adventure into web browser development / general WWW-related shenanigans, clone a local copy on to your machine:
```sh
git clone https://github.com/libdev-hacker/rogue.git
```
### Building
To build Rogue, first enter the project's root directory via the command: 
```sh
cd rogue (or whichever directory you cloned into)
```
Then, to build the project run:
```sh
dotnet build
```

## Usage

> [!CAUTION]
> 🚨 DUE TO THE PROJECT'S INFANCY, PLEASE DO NOT DAILY DRIVE ROGUE! 🚨 (yet)

Due to the project purely being a W.I.P., the only way to access any webpages using Rogue is via the command line.
```sh
[path to bin]/Rogue https://example.com
```
If you leave the URL argument blank, you will be redirected to Rogue's default blank page, found [here](assets/blank.html)

## Reference
```assets/``` - Rogue's static asset cache

```src/``` - Root directory where all of Rogue's source code lives

```src/Rogue``` - Project that manages all UI logic (Tabs, UI Backend, WebPage Rendering...)

```src/Rogue.Graphics``` - Manages interoperation with the OpenGl / OpenGL ES / Metal (planned) backends

```src/Rogue.HTML``` - Parses HTML documents & constructs the DOM

```src/Rogue.JS``` - Handles JavaScript Execution, Isolation & Interoperation

```src/Rogue.JS.DOM``` - Acts as pivot for JS interactions with the DOM (implemented *literally* to fix a circular dependency problem)

```src/Rogue.Utils``` - Contains project-wide abstractions for 3<sup>rd</sup> party dependencies + other miscellaneous APIs

## Roadmap

- [ ] Overhaul Graphics Backend
- [ ] New Text Renderer
- [ ] Full HTML Spec Support
- [ ] Implement CSS Support

.. and more to come!

## License

This project is licensed under the [MIT License](LICENSE). This allows for Rogue or any of its constituent projects to be used, modified & re-distributed by anyone within the community. (Patches are still very much welcome though)

## Contributing

In the spirit of the open web, I **greatly appreciate** any & all contributions! However, since I proudly stand behind my opposing view to that of the author of this rather [infamous quote](https://x.com/FFmpeg/status/1762805900035686805), I invite you to please open a discussion & propose any new ideas you have. Not only does this help me decide Rogue's next steps, it also allows you to fully think out any idea for features / bug fixes you may have with the community.

But if you just want to code without wanting a thousand voices in your ear, feel free to fork this repo & open a Pull Request :)

For full guidance of contributing, please see our [CONTRIBUTING.MD](CONTRIBUTING.md) for more information & my policy on LLM support. Also, for the safety of this project & its community, please adhere to our [code of conduct](CODE_OF_CONDUCT.md).

#### Top Contributors
##### Graph courtesy of [contrib.rocks](https://contrib.rocks)
<a href="https://github.com/libdev-hacker/rogue/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=libdev-hacker/rogue" />
</a>
<p>I'm so alone 😭</p>
