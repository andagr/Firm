My main programming language when I was studying was, as for many other developers, Java. There were other languages too, but Java was the main tool for learning basic software development. Later in my work I came in contact with C#, and since then I have come to prefer it over Java due to its features and ability to quickly evolve, and the extensive .NET API.

The basic aspect of C# is the object orientation which works similarly as in Java, although there are [subtle differences][1]. However, what makes it a bit more exciting are other features such as dynamic, async/await and functional. The latter is something that has gained quite a lot of popularity, although [it's not a new paradigm][2]. It was this aspect of C#, and in practice LINQ, that got me curious about functional programming in general. 

I decided to take a closer look at F#, partly because it's a member of the .NET platform but also because it seems to be developed from a pragmatic point of view. It is also very exciting that it is open source, seems to have a friendly community, and has support on several different platforms. Also noteworthy, at the time of writing F# has risen from position 45 to 16 in the [TIOBE index][3].

![F# Logo][4]

I am still very much a beginner, but in the spirit of recording things here are the resources I have used so far.

General
-------

 - [fsharp.org][5] - A hub for information about F# and the first place you should look.
 - [tryfsharp.org][6] - This site is amazing, it has a tutorial including an editor (with intellisense!) that works by using Silverlight to host the F# compiler in the browser.
 - [fsharpforfunandprofit.com][7] - A general site for learning F# that contains a lot of information in the form of tutorials and posts. 
 - [F# Language Reference][8] - F# on MSDN.
 - [Learn X in Y minutes, where X = F#][9] - To the point information about F# in the form of code examples.

Blogs
-----

These are blogs, related to F#, that I try to read as often as possible, in no particular order.

 - [Rachel Reese][10]
 - [Devjoy][11]
 - [Visual Studio F# Team][12]
 - [Tomas Petricek][13]
 - [Sergey Tihon][14]
 - [Michael Newton][15]
 - [Kit Eason][16]
 - [Don Syme][17]

Books
-----
There are several [book suggestions on fsharp.org][18], but the one I got and plan to read is [Functional Programming using F#][19].

![Functional Programming using F#][20]

Videos
------
When I was at [Öredev][21] I attended a couple of sessions on F#.

<h3>F# for C# developers by Phil Trelford</h3>

<iframe src="//player.vimeo.com/video/78908217" width="500" height="300" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>

<h3>Discovering Type Providers in F# 3.0 by Rachel Reese</h3>

<iframe src="//player.vimeo.com/video/79402548" width="500" height="300" webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe>

Podcasts
--------
[The .NET Rocks][22] show has several shows where they talk about F#.

Practicing
----------
[Project Euler][23] is a good resource to use for practicing problem solving using programming. Other than that I have used F# to solve a problem that interested me, and there will be a separate blog post about it later. I will keep using F# wherever it feels like a good fit, because for me personally trying things out is what works best when learning something. 


  [1]: http://stackoverflow.com/questions/1882692/c-sharp-constructor-execution-order
  [2]: http://en.wikipedia.org/wiki/Functional_programming#History
  [3]: http://www.tiobe.com/index.php/content/paperinfo/tpci/index.html
  [4]: /Content/images/uploaded%5Clogo.png
  [5]: http://fsharp.org
  [6]: http://www.tryfsharp.org
  [7]: http://fsharpforfunandprofit.com
  [8]: http://msdn.microsoft.com/en-us/library/dd233181.aspx
  [9]: http://learnxinyminutes.com/docs/fsharp/
  [10]: http://rachelree.se/
  [11]: http://www.devjoy.com/
  [12]: http://blogs.msdn.com/b/fsharpteam/
  [13]: http://tomasp.net/
  [14]: http://sergeytihon.wordpress.com/
  [15]: http://blog.mavnn.co.uk/
  [16]: http://www.kiteason.com/
  [17]: http://blogs.msdn.com/b/dsyme/
  [18]: http://fsharp.org/about/learning.html
  [19]: http://www2.imm.dtu.dk/~mire/FSharpBook/
  [20]: /Content/images/uploaded%5C9781107684065_200_functional-programming-using-f_haftad.jpg
  [21]: http://oredev.org
  [22]: http://www.dotnetrocks.com/
  [23]: http://projecteuler.net/